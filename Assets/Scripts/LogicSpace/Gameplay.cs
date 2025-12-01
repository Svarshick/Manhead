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

        private List<Cell> _movingCells;
        private Predictor _predictor; 
        private State _state;

        public Gameplay(Map map)
        {
            _map = map;
            _moveAction = InputSystem.actions["Move"];
        }

        public void Start()
        {
            _state = State.WaitingDecision;
            _movingCells = ExtractMovingCells(_map);
            _predictor = new();
            _moveAction.performed += ctx => StartTurn(ctx).Forget();
        }

        private static List<Cell> ExtractMovingCells(Map map)
        {
            var cells = new List<Cell>(map.Width * map.Height);
            foreach (var (_, field) in map.Fields) cells.AddRange(field.Cells);
            return cells.FindAll(cell => cell.Components.ContainsKey(typeof(Moving)));
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
                direction = cell.Components.ContainsKey(typeof(Player)) ? playerDirection : DirectionUtils.GetRandom();
                cell.LookDirection = direction;

                var movingComponent = (Moving)cell.Components[typeof(Moving)];
                var movementController = movingComponent.MovementController;
                while (true)
                {
                    var step = movementController.GetNextStep();
                    if (step.stepDirection == Direction.Ambiguous)
                        break;
                    var future = _predictor.Predict(cell, step);
                    Debug.Log($"{cell.gameObject.name}: {future}");
                    await DoFuture(future);
                }

                await UniTask.Delay(1000);
            }

            EndTurn();
        }

        private async UniTask DoFuture(IEnumerable<IRequest> future)
        {
            if (future.Any(request => request is StopRequest)) return;
            foreach (var request in future)
            {
                 switch (request)
                 {
                     case RotateRequest rotateRequest:
                         rotateRequest.target.LookDirection = rotateRequest.lookDirection;
                         break;
                     case MoveRequest moveRequest:
                         var cell = moveRequest.target;
                         var direction = moveRequest.direction;
                         //TODO could fail, temp dirt hack
                         var movingComponent = (Moving)cell.Components[typeof(Moving)];
                         await MovementSystem.Move(cell.GetCancellationTokenOnDestroy(), cell, direction, movingComponent.Speed);
                         break;
                 }
            }
        }

        private void EndTurn() => _state = State.WaitingDecision;
    }
}