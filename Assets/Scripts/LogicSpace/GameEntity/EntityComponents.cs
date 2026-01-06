using System;
using LogicSpace.Movement;
using UnityEngine;

namespace LogicSpace.GameEntity
{
    [Serializable]
    public abstract class EntityComponent
    {
    }

    //Types

    [Serializable]
    public class Player : EntityComponent
    {
    }

    [Serializable]
    public class Moving : EntityComponent
    {
        [Min(1f)] public float Speed;

        public MovementType MovementType;
        public int Priority;
    }
}