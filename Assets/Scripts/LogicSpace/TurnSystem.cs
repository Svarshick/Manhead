using UnityEngine;
using UnityEngine.InputSystem;

namespace LogicSpace
{
    public class TurnSystem
    {
        GameObject _player;
        InputAction _moveAction;
        
        public TurnSystem(GameObject player)
        {
            _moveAction = InputSystem.actions["Move"];
            //_moveAction.performed += 
        }
    }
}