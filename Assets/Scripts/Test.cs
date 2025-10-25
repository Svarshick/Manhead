using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private GameObject targetObject;
        
        private InputAction _moveAction;
        private float _speed = 0.5f;
        
        private void Awake()
        {
            _moveAction = InputSystem.actions["Move"];
        }

        private void Start()
        {
            _moveAction.performed += context => StepByStepMoving(targetObject, context.ReadValue<Vector2>(), 3);
        }

        void Update()
        {
            Debug.Log("update");
        }

        private async UniTask StepByStepMoving(GameObject obj, Vector3 dimension, int steps)
        {
            for (int i = 0; i < steps; ++i)
            {
                Debug.Log($"Step {i}");
                await MoveTowards(obj, obj.transform.position + dimension * 3);
                await UniTask.Delay(TimeSpan.FromSeconds(2));
            }
        }

        private async UniTask MoveTowards(GameObject obj, Vector3 target)
        {
            while (true)
            {
                var diff = target - obj.transform.position;
                var shift = diff.normalized * _speed * Time.deltaTime;
                if (shift.magnitude < diff.magnitude)
                {
                    obj.transform.position += shift;
                    await UniTask.NextFrame();
                }
                else
                {
                    obj.transform.position = target;
                    return;
                }
            }
        }
    }
}