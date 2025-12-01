using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using LogicSpace.Cell;
using LogicSpace.Movement;
using LogicSpace.Predictor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
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

        private State _state;
        private Map _map;
        private List<Cell.Cell> _movingCells;
        private InputAction _moveAction;
        private Predictor.Predictor _predictor = new();
        
        public Gameplay(Map map)
        {
            _state = State.WaitingDecision;
            
            _map = map;
            _movingCells = ExtractMovingCells(_map);
            
            _moveAction = InputSystem.actions["Move"];
            _moveAction.performed += ctx => StartTurn(ctx).Forget();
        }
        
        public void Start() {}
        
        private static List<Cell.Cell> ExtractMovingCells(Map map)
        {
            var cells = new List<Cell.Cell>(map.Width * map.Height);
            foreach (var (_, field) in map.Fields)
            {
                cells.AddRange(field.Cells);
            }
            return cells.FindAll(cell => cell.GetComponent<Moving>() is not null);
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
                if (cell.GetComponent<Player>() != null)
                    direction = playerDirection;
                else
                    direction = DirectionUtils.GetRandom();
                
                var movingComponent = cell.GetComponent<Moving>();
                var movement = MovementTypeFabric.Create(movingComponent.MovementType, cell.Field.Grid, cell.Field.GridPosition, direction);
                while (true)
                {
                    var step = movement.GetNextStep();
                    if (step == null)
                        break;
                    var future = _predictor.Predict(cell, (Direction)step);
                    Debug.Log($"{cell.gameObject.name}: {future}");
                    await DoFuture(future);
                }
                await UniTask.Delay(1000);
            }
            EndTurn();
        }

        private async UniTask DoFuture(IEnumerable<IRequest> future)
        {
            if (future.Any(request => request is StopRequest))
            {
                return;
            }
            foreach (var moveRequest in future.OfType<MoveRequest>())
            {
                var targetCell = moveRequest.target;
                var direction = moveRequest.direction;
                //TODO could fail, temp dirt hack
                var movingComponent = targetCell.GetComponent<Moving>();
                targetCell.LookDirection = direction;
                await MovementSystem.Move(targetCell.GetCancellationTokenOnDestroy(), targetCell, direction, movingComponent.Speed);
            }
        }
        
        private void EndTurn()
        {
            _state = State.WaitingDecision;
        }
    }
}