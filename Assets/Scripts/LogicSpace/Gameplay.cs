using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LogicSpace.Cells;
using LogicSpace.Fields;
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

        private readonly Map _map;
        private readonly InputAction _moveAction;
        private readonly int _delayTime;

        private List<Cell> _movingCells;
        private Predictor _predictor;
        private State _state;
        private MovementController _movementController;
        private Camera _camera;

        public Gameplay(Map map, int delayTime)
        {
            _map = map;
            _moveAction = InputSystem.actions["Move"];
            _delayTime = delayTime;
        }

        public void Start()
        {
            _state = State.WaitingDecision;
            _movingCells = ExtractMovingCells(_map);
            _predictor = new();
            _moveAction.performed += ctx => StartTurn(ctx).Forget();
            _camera = Camera.main;
            FitCameraToBounds(_map.WorldMin, _map.WorldMax);
        }

        private static List<Cell> ExtractMovingCells(Map map)
        {
            var cells = new List<Cell>(map.Width * map.Height);
            foreach (var (_, field) in map.Fields) cells.AddRange(field.Cells);
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
            foreach (var cell in _movingCells)
            {
                Direction direction;
                direction = cell.GetComponent<Player>() != null ? playerDirection : DirectionUtils.GetRandom();
                cell.LookDirection = direction;

                var movingComponent = cell.GetComponent<Moving>();
                _movementController = movingComponent.MovementController;
                while (true)
                {
                    var step = _movementController.GetNextStep();
                    if (step.stepDirection == Direction.Ambiguous)
                        break;
                    var future = _predictor.Predict(cell, step);
                    Debug.Log($"{cell.gameObject.name}: {future}");
                    await DoFuture(future);
                }

                await UniTask.Delay(_delayTime);
            }

            EndTurn();
        }

        private async UniTask DoFuture(IEnumerable<IRequest> future)
        {
            Debug.Log("Doing future");
            foreach (var request in future)
            {
                switch (request)
                {
                    case StopRequest stopRequest:
                        return;
                    case RotateRequest rotateRequest:
                        rotateRequest.target.LookDirection = rotateRequest.lookDirection;
                        _movementController.LookDirection = rotateRequest.lookDirection;
                        break;
                    case MoveRequest moveRequest:
                        var cell = moveRequest.target;
                        var direction = moveRequest.direction;
                        //TODO could fail, temp dirt hack
                        var movingComponent = cell.GetComponent<Moving>();
                        await MovementSystem.Move(cell.GetCancellationTokenOnDestroy(), cell, direction,
                            movingComponent.Speed);
                        break;
                }
            }
        }

        private void EndTurn() => _state = State.WaitingDecision;

        private void FitCameraToBounds(Vector2 bottomLeft, Vector2 topRight, float padding = 0f)
        {
            if (_camera == null) return;

            // 1. Вычисляем центр прямоугольника
            Vector2 center = (bottomLeft + topRight) * 0.5f;

            // 2. Перемещаем камеру в центр
            _camera.transform.position = new Vector3(center.x, center.y, _camera.transform.position.z);

            // 3. Вычисляем ширину и высоту прямоугольника
            float width = topRight.x - bottomLeft.x + padding * 2;
            float height = topRight.y - bottomLeft.y + padding * 2;

            // 4. Настраиваем ортографический размер
            // Для ортографической камеры: видимая высота = 2 * orthographicSize
            // Видимая ширина = 2 * orthographicSize * aspect

            float aspect = _camera.aspect; // Соотношение сторон (ширина/высота)

            // Нужно выбрать orthographicSize так, чтобы в поле зрения поместилось и по ширине, и по высоте
            float sizeByHeight = height * 0.5f; // Половина высоты
            float sizeByWidth = (width * 0.5f) / aspect; // С учетом соотношения сторон

            // Выбираем большее значение, чтобы все поместилось
            _camera.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
        }
    }
}