using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Gameplay;
using Manhead.Core.Logic.Gameplay.Data;
using Manhead.Core.Logic.Gameplay.Data.Components;
using Manhead.Core.Logic.Gameplay.Movement;
using Manhead.Core.Logic.Gameplay.Prediction;
using Manhead.Core.Logic.WorldSpace;

public class GameLoop
{
    private List<Entity> _entities = new();
    private Field<Entity> _entityField;
    private EntityView _entityView;

    private GameInput _input;
    private MovementService _movementService;
    private Predictor _predictor;
    
    private State _state = State.Unloaded;
    private List<Entity> _movingEntities = new();
    private int _delayTime;
    
    public enum State
    {
        Unloaded,
        WaitingDecision,
        ProcessingTurn
    }
    
    public GameLoop(LevelBlueprint level, GameInput input, GridLayout gridLayout)
    {
        _entityView = new EntityView(gridLayout);
        _entityField = new Field<Entity>(level.Field.Width, level.Field.Height);
        for (int x = 0; x < level.Field.Width; x++)
        {
            for (int y = 0; y < level.Field.Height; y++)
            {
                if (level.Field[x][y].Count < 1)
                {
                    continue;
                }

                var placement = level.Field[x][y][0];
                var entity = placement.Template.Entity.Clone();
                entity.Position = placement.Position;
                
                _entityField[x][y].Add(entity);
                _entities.Add(entity);
                var view = _entityView.AddView(entity);
                view.RelativePosition = gridLayout.GridToWorld(entity.Position);
            }
        }
        
        _movingEntities = _entities.Where(entity => entity.GetComponent<Moving>() != null).ToList();

        _predictor = new Predictor(_entityField);
        _movementService = new MovementService(_entityField, gridLayout);
        _input = input;
        
        _state = State.WaitingDecision;
        _input.Move += direction => StartTurn(direction);
    }

    public void Draw()
    {
        _entityView.Draw();
    }
    
    private async Task StartTurn(Direction direction)
    {
        switch (_state)
        {
            case State.Unloaded:
                Console.WriteLine("Attempt to start in unloaded state");
                return;
            case State.ProcessingTurn:
                return;
        }

        if (direction == Direction.Ambiguous)
            return;

        _state = State.ProcessingTurn;
        foreach (var entity in _movingEntities)
        {
            var turnDirection = entity.GetComponent<Player>() != null ? direction : DirectionUtils.Random();

            var movingComponent = entity.GetComponent<Moving>();
            var movementController = MovementTypeFabric.Create(
                MovementType.AllTheWayMovement,
                entity.Position,
                turnDirection);

            var continueTurn = true;
            while (continueTurn)
            {
                var step = movementController.NextStep();
                var future = _predictor.Predict(entity, step);
                Console.WriteLine($"{nameof(entity)}: {future}");
                continueTurn = await DoFuture(future, movementController);
            }

            await Task.Delay(_delayTime);
        }

        EndTurn();
    }
    
    private async Task<bool> DoFuture(IEnumerable<IRequest> future, MovementController movementController)
    {
        Console.WriteLine($"Doing future:\n{future.ToLogString()}");
        foreach (var request in future)
            switch (request)
            {
                case StopRequest stopRequest:
                    return false;
                case RotateRequest rotateRequest:
                    rotateRequest.target.LookDirection= rotateRequest.lookDirection;
                    movementController.LookDirection = rotateRequest.lookDirection;
                    break;
                case MoveRequest moveRequest:
                    var entity = moveRequest.target;
                    var direction = moveRequest.direction;
                    //TODO could fail
                    var movingComponent = entity.GetComponent<Moving>();
                    var entityView = _entityView.GetView(entity);
                    await _movementService.Move(entity, entityView, direction, movingComponent.Speed);
                    movementController.CurrentPosition = entity.Position;
                    break;
            }

        return true;
    }
    
    private void EndTurn()
    {
        _state = State.WaitingDecision;
    }
}