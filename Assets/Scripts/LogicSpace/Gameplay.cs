using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Cysharp.Threading.Tasks;
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
        private FieldsGrid _fieldsGrid;
        private List<Cell.Cell> _cells;
        private InputAction _moveAction;
        private Predictor.Predictor _predictor = new();
        
        public Gameplay(Tilemap tilemap)
        {
            _state = State.WaitingDecision;
            
            _fieldsGrid = CreateFieldsGrid(tilemap);
            _cells = ExtractCells(_fieldsGrid);
            
            _moveAction = InputSystem.actions["Move"];
            _moveAction.performed += ctx => StartTurn(ctx).Forget();
        }
        
        public void Start() {}

        private FieldsGrid CreateFieldsGrid(Tilemap tilemap)
        {
            tilemap.CompressBounds();
            int padding = 3;
            int xMin = tilemap.cellBounds.xMin - padding;
            int yMin = tilemap.cellBounds.yMin - padding;
            int xMax = tilemap.cellBounds.xMax + padding;
            int yMax = tilemap.cellBounds.yMax + padding;
            int width = xMax - xMin + 1;
            int height = yMax - yMin + 1;
            Dictionary<Vector2Int, Field> fields = new(width * height);
            var fieldsGrid = new FieldsGrid(tilemap, fields);
            for (int x = xMin; x <= xMax; ++x)
            {
                for (int y = yMin; y <= yMax; ++y)
                {
                    var position = new Vector2Int(x, y);
                    fieldsGrid.Fields[position] = new Field(fieldsGrid, position);
                }
            }

            foreach (var cell in tilemap.GetComponentsInChildren<Cell.Cell>())
            {
                var cellPosition = (Vector2Int) fieldsGrid.Tilemap.WorldToCell(cell.transform.position);
                fieldsGrid.Fields[cellPosition].Cells.Add(cell);
                cell.ChangeField(fieldsGrid.Fields[cellPosition]);
            }
            
            return fieldsGrid;
        }
        
        private List<Cell.Cell> ExtractCells(FieldsGrid fieldsGrid)
        {
            var cells = new List<Cell.Cell>(fieldsGrid.Width * fieldsGrid.Height);
            foreach (var (_, field) in fieldsGrid.Fields)
            {
                cells.AddRange(field.Cells);
            }
            return cells;
        }

        private async UniTask StartTurn(InputAction.CallbackContext context)
        {
            if (_state == State.ProcessingTurn)
                return;
            var playerDirection = context.ReadValue<Vector2>().ToDirection();
            if (playerDirection == Direction.Ambiguous)
                return;
            
            _state = State.ProcessingTurn;
            foreach (var cell in _cells)
            {
                IMovementType movement;
                if (cell.GetComponent<Player>())
                {
                    movement = new AllTheWayMovement(cell.Field.Grid, cell.Field.GridPosition, playerDirection);
                }
                else
                {
                    var direction = DirectionUtils.GetRandom();
                    movement = new AllTheWayMovement(cell.Field.Grid, cell.Field.GridPosition, direction);
                }

                while (true)
                {
                    var step = movement.GetNextStep();
                    if (step == null)
                        break;
                    var future = _predictor.Predict(cell, (Direction)step);
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
                await MovementSystem.Move(targetCell.GetCancellationTokenOnDestroy(), targetCell, moveRequest.direction);
            }
        }
        
        private void EndTurn()
        {
            _state = State.WaitingDecision;
        }
    }
}