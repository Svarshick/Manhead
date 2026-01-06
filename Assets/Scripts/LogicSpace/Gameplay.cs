using System.Collections.Generic;
using System.Linq;
using CustomMath;
using Cysharp.Threading.Tasks;
using LogicSpace.GameEntity;
using LogicSpace.GameField;
using LogicSpace.Movement;
using LogicSpace.Prediction;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace LogicSpace
{
    public class Gameplay : IStartable
    {
        public enum State
        {
            WaitingDecision,
            ProcessingTurn
        }

        private readonly int _delayTime;

        private readonly Field _field;
        private readonly InputAction _moveAction;
        private Camera _camera;
        private MovementController _movementController;

        private List<Entity> _movingCells;
        private Predictor _predictor;
        private State _state;

        public Gameplay(Field field, int delayTime)
        {
            _field = field;
            _moveAction = InputSystem.actions["Move"];
            _delayTime = delayTime;
        }

        public void Start()
        {
            _state = State.WaitingDecision;
            _movingCells = ExtractMovingCells(_field);
            _predictor = new Predictor();
            _moveAction.performed += ctx => StartTurn(ctx).Forget();
            _camera = Camera.main;
            FitCameraToBounds(_field.WorldMin, _field.WorldMax);
        }

        private static List<Entity> ExtractMovingCells(Field field)
        {
            var cells = new List<Entity>(field.Width * field.Height);
            foreach (var cell in field.Cells) cells.AddRange(cell.Entities);
            return cells.FindAll(cell => cell.GetComponent<Moving>() != null)
                .OrderBy(cell => cell.GetComponent<Moving>().Priority).ToList();
        }

        private async UniTask StartTurn(InputAction.CallbackContext context)
        {
            if (_state == State.ProcessingTurn)
                return;
            var playerDirection = context.ReadValue<Vector2>().ToDirection();
            if (playerDirection == Direction.Ambiguous)
                return;

            _state = State.ProcessingTurn;
            foreach (var entity in _movingCells)
            {
                Direction turnDirection;
                turnDirection = entity.GetComponent<Player>() != null ? playerDirection : DirectionUtils.GetRandom();
                var continueTurn = true;

                var movingComponent = entity.GetComponent<Moving>();
                _movementController = MovementTypeFabric.Create(movingComponent.MovementType,
                    entity.Cell.GridPosition, turnDirection);
                while (continueTurn)
                {
                    var step = _movementController.GetNextStep();
                    var future = _predictor.Predict(entity, step);
                    Debug.Log($"{nameof(entity)}: {future}");
                    continueTurn = await DoFuture(future);
                }

                await UniTask.Delay(_delayTime);
            }

            EndTurn();
        }

        private async UniTask<bool> DoFuture(IEnumerable<IRequest> future)
        {
            Debug.Log($"Doing future:\n{future.ToLogString()}");
            foreach (var request in future)
                switch (request)
                {
                    case StopRequest stopRequest:
                        return false;
                    case RotateRequest rotateRequest:
                        rotateRequest.target.LookDirection = rotateRequest.lookDirection;
                        //TODO target depends on rotateRequest, but _movementController not???
                        _movementController.LookDirection = rotateRequest.lookDirection;
                        break;
                    case MoveRequest moveRequest:
                        var entity = moveRequest.target;
                        var direction = moveRequest.direction;
                        //TODO could fail, temp dirt hack
                        var movingComponent = entity.GetComponent<Moving>();
                        await MovementSystem.Move(entity.Appearance.GetCancellationTokenOnDestroy(), entity, direction,
                            movingComponent.Speed);
                        //TODO target depends on rotateRequest, but _movementController not???
                        _movementController.CurrentPosition = entity.Cell.GridPosition;
                        break;
                }

            return true;
        }

        private void EndTurn()
        {
            _state = State.WaitingDecision;
        }

        private void FitCameraToBounds(Vector2 bottomLeft, Vector2 topRight, float padding = 0f)
        {
            if (_camera == null) return;

            // 1. Вычисляем центр прямоугольника
            var center = (bottomLeft + topRight) * 0.5f;

            // 2. Перемещаем камеру в центр
            _camera.transform.position = new Vector3(center.x, center.y, _camera.transform.position.z);

            // 3. Вычисляем ширину и высоту прямоугольника
            var width = topRight.x - bottomLeft.x + padding * 2;
            var height = topRight.y - bottomLeft.y + padding * 2;

            // 4. Настраиваем ортографический размер
            // Для ортографической камеры: видимая высота = 2 * orthographicSize
            // Видимая ширина = 2 * orthographicSize * aspect

            var aspect = _camera.aspect; // Соотношение сторон (ширина/высота)

            // Нужно выбрать orthographicSize так, чтобы в поле зрения поместилось и по ширине, и по высоте
            var sizeByHeight = height * 0.5f; // Половина высоты
            var sizeByWidth = width * 0.5f / aspect; // С учетом соотношения сторон

            // Выбираем большее значение, чтобы все поместилось
            _camera.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
        }
    }
}