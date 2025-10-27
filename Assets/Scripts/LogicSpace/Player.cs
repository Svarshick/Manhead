using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LogicSpace
{
    [RequireComponent(typeof(Cell))]
    public class Player : MonoBehaviour
    {
        private Cell _cell;
        private UniTask _moveTask;
        
        void Awake()
        {
            _cell = GetComponent<Cell>();
            var moveAction = InputSystem.actions["Move"];
            moveAction.performed += OnMove;
        }
        
        private void OnMove(InputAction.CallbackContext ctx)
        {
            if (_moveTask.Status == UniTaskStatus.Pending)
                return;
            
            var direction = ctx.ReadValue<Vector2>();
            if (direction == Vector2.up || 
                direction == Vector2.down || 
                direction == Vector2.left ||
                direction == Vector2.right)
            {
                _moveTask = MovementSystem.Move(this.GetCancellationTokenOnDestroy(), _cell, direction);
            }
        }
    }
}