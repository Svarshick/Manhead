using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpace.GameEntity;
using UnityEngine;

namespace LogicSpace.EditorData
{
    public class EntityData : MonoBehaviour, IEntity
    {
        [SerializeReference] public List<EntityComponent> Components = new();

        public EntitySideData FrontSide;
        public EntitySideData LeftSide;
        public EntitySideData BackSide;
        public EntitySideData RightSide;
        public Color Color = Color.white;

        public T GetComponent<T>() where T : EntityComponent
        {
            return Components.OfType<T>().FirstOrDefault();
        }

        IEntitySide IEntity.FrontSide => FrontSide;
        IEntitySide IEntity.LeftSide => LeftSide;
        IEntitySide IEntity.BackSide => BackSide;
        IEntitySide IEntity.RightSide => RightSide;
        Color IEntity.Color => Color;
    }

    [Serializable]
    public class EntitySideData : IEntitySide
    {
        [SerializeReference] public List<EntitySideComponent> Components = new();

        public T GetComponent<T>() where T : EntitySideComponent
        {
            return Components.OfType<T>().FirstOrDefault();
        }
    }
}