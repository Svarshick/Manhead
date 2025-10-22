using LogicSpace;
using Scellecs.Morpeh;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Test : MonoBehaviour
    {
        [SerializeField] public FieldProvider fieldProvider;
        
        private World _world;
        private void Awake()
        {
            _world = World.Default;
            var group = _world.CreateSystemsGroup();
            group.AddSystem(new MovingSystem());
            _world.AddSystemsGroup(1, group);
        }

        private void Start()
        {
            var cellProvider = GetComponent<MovableProvider>();
            ref var moving = ref cellProvider.Entity.AddComponent<Moving>();
            moving.TargetField = fieldProvider.Entity;
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("trigger");
        }
    }
}