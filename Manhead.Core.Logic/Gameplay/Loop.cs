/*using System.Collections.Generic;

namespace Manhead.Core.Gameplay;

public class Loop : IStartable
{
    public enum State
    {
        Unloaded,
        WaitingDecision,
        ProcessingTurn
    }

    private readonly MovementService _movementService;
    private readonly List<Entity> _movingCells;

    private readonly int _delayTime;

    private readonly InputAction _moveAction;
    private readonly PredictionService _predictionService;

    private State _state = State.Unloaded;

    public Loop(Storage storage, MovementService movementService, InputSystemActions input, int delayTime = 0)
    {
        _movementService = movementService;
        _movingCells = ExtractMovingCells(storage);

        _delayTime = delayTime;

        _predictionService = new PredictionService();

        input.Game.Enable();
        _moveAction = input.Game.Move;
    }

    public void Start()
    {
        _state = State.WaitingDecision;
        _moveAction.performed += ctx => StartTurn(ctx).Forget();
    }

    private async UniTask StartTurn(InputAction.CallbackContext context)
    {
        switch (_state)
        {
            case State.Unloaded:
                Debug.LogWarning("Attempt to start in unloaded state");
                return;
            case State.ProcessingTurn:
                return;
        }

        var playerDirection = context.ReadValue<Vector2>().ToDirection();
        if (playerDirection == Direction.Ambiguous)
            return;

        _state = State.ProcessingTurn;
        foreach (var entity in _movingCells)
        {
            var turnDirection = entity.GetComponent<Player>() != null ? playerDirection : DirectionUtils.Random();

            var movingComponent = entity.GetComponent<Moving>();
            var movementController = MovementTypeFabric.Create(
                movingComponent.MovementType.Value,
                entity.Cell.Value.Position,
                turnDirection);

            var continueTurn = true;
            while (continueTurn)
            {
                var step = movementController.NextStep();
                var future = _predictionService.Predict(entity, step);
                Debug.Log($"{nameof(entity)}: {future}");
                continueTurn = await DoFuture(future, movementController);
            }

            await UniTask.Delay(_delayTime);
        }

        EndTurn();
    }

    private async UniTask<bool> DoFuture(IEnumerable<IRequest> future, MovementController movementController)
    {
        Debug.Log($"Doing future:\n{future.ToLogString()}");
        foreach (var request in future)
            switch (request)
            {
                case StopRequest stopRequest:
                    return false;
                case RotateRequest rotateRequest:
                    rotateRequest.target.LookDirection.Value = rotateRequest.lookDirection;
                    movementController.LookDirection = rotateRequest.lookDirection;
                    break;
                case MoveRequest moveRequest:
                    var entity = moveRequest.target;
                    var direction = moveRequest.direction;
                    //TODO could fail
                    var movingComponent = entity.GetComponent<Moving>();
                    await _movementService.Move(entity, direction, movingComponent.Speed.Value);
                    movementController.CurrentPosition = entity.Cell.Value.Position;
                    break;
            }

        return true;
    }

    private void EndTurn()
    {
        _state = State.WaitingDecision;
    }


    private static List<Entity> ExtractMovingCells(Storage storage)
    {
        return storage.Entities.Where(entity => entity.GetComponent<Moving>() != null)
            .OrderBy(entity => entity.GetComponent<Moving>().Priority).ToList();
    }
}*/