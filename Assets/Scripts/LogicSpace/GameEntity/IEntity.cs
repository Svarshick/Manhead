namespace LogicSpace.GameEntity
{
    public interface IEntity
    {
        public IEntitySide LeftSide { get; }
        public IEntitySide RightSide { get; }
        public IEntitySide FrontSide { get; }
        public IEntitySide BackSide { get; }
        public T GetComponent<T>() where T : EntityComponent;
    }

    public interface IEntitySide
    {
        public T GetComponent<T>() where T : EntitySideComponent;
    }
}