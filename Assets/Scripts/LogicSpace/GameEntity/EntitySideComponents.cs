using System;
using CustomMath;
using UnityEngine.Serialization;

namespace LogicSpace.GameEntity
{
    [Serializable]
    public class EntitySideComponent
    {
    }

    [Serializable]
    public class Crossroad : EntitySideComponent
    {
        [FormerlySerializedAs("RotationDirection")]
        public Direction rotationDirection;
    }

    [Serializable]
    public class Stop : EntitySideComponent
    {
    }
}