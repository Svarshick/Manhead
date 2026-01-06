using System;
using System.Collections.Generic;
using CustomMath;
using LogicSpace.EditorData;
using LogicSpace.GameField;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LogicSpace.GameEntity
{
    public class Entity : IEntity
    {
        private readonly Dictionary<Type, EntityComponent> _components;
        public readonly EntitySide BackSide;
        public readonly EntitySide FrontSide;
        public readonly EntitySide LeftSide;
        public readonly EntitySide RightSide;

        private GameObject _gameObject;

        private Direction _lookDirection = Direction.Up;

        public Entity()
        {
            _components = new Dictionary<Type, EntityComponent>();
            FrontSide = new EntitySide(this, Direction.Up);
            LeftSide = new EntitySide(this, Direction.Left);
            RightSide = new EntitySide(this, Direction.Right);
            BackSide = new EntitySide(this, Direction.Down);
        }

        public Entity(EntityData data)
        {
            _components = new Dictionary<Type, EntityComponent>();
            foreach (var component in data.Components)
            {
                var type = component.GetType();
                if (_components.ContainsKey(type))
                {
                    Debug.LogAssertion($"{this} get more than one component of type {type}");
                    _components[type] = component;
                }
                else
                {
                    _components.Add(component.GetType(), component);
                }
            }

            FrontSide = new EntitySide(this, Direction.Up, data.FrontSide.Components);
            LeftSide = new EntitySide(this, Direction.Left, data.LeftSide.Components);
            BackSide = new EntitySide(this, Direction.Down, data.BackSide.Components);
            RightSide = new EntitySide(this, Direction.Right, data.RightSide.Components);

            //TODO: dirty
            Appearance = data.transform.GetChild(0).gameObject;
        }

        public Cell Cell { get; private set; }

        //TODO: should it be public??
        public GameObject Appearance
        {
            get => _gameObject;
            set
            {
                Object.DestroyImmediate(_gameObject);
                if (value != null && Cell != null)
                    value.transform.position = Cell.WorldPosition;
                _gameObject = value;
            }
        }

        public Direction LookDirection
        {
            get => _lookDirection;
            set
            {
                _lookDirection = value;
                switch (_lookDirection)
                {
                    case Direction.Up: Appearance.transform.localRotation = Quaternion.Euler(0, 0, 0); break;
                    case Direction.Left: Appearance.transform.localRotation = Quaternion.Euler(0, 0, 90); break;
                    case Direction.Down: Appearance.transform.localRotation = Quaternion.Euler(0, 0, 180); break;
                    case Direction.Right: Appearance.transform.localRotation = Quaternion.Euler(0, 0, 270); break;
                }
            }
        }

        IEntitySide IEntity.FrontSide => FrontSide;
        IEntitySide IEntity.LeftSide => LeftSide;
        IEntitySide IEntity.BackSide => BackSide;
        IEntitySide IEntity.RightSide => RightSide;

        public T GetComponent<T>() where T : EntityComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
                return (T)component;
            return null;
        }

        public void ChangeCell(Cell cell)
        {
            Cell?.Entities.Remove(this);
            Cell = cell ?? throw new ArgumentNullException(nameof(cell));
            cell.Entities.Add(this);
        }

        public EntitySide GetSide(Direction direction)
        {
            return direction switch
            {
                Direction.Up => FrontSide,
                Direction.Left => LeftSide,
                Direction.Down => BackSide,
                Direction.Right => RightSide,
                _ => null
            };
        }

        public EntitySide GetSideVisibleFrom(Direction fromDirection)
        {
            var sideDirection = SpaceUtility.GetFacingSideDirection(fromDirection, _lookDirection);
            return GetSide(sideDirection);
        }
    }
}