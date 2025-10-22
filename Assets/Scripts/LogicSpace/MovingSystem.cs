using Scellecs.Morpeh;

namespace LogicSpace
{
    
    public class MovingSystem : ISystem
    {
        public World World { get; set; }

        private Filter _movingFilter;
        private Stash<Cell> _cellStash;
        private Stash<Moving> _movingStash;
        private Stash<Field> _fieldStash;

        public void OnAwake()
        {
            _movingFilter = World.Filter.With<Moving>().With<Cell>().Build();
            _cellStash = World.GetStash<Cell>();
            _movingStash = World.GetStash<Moving>();
            _fieldStash = World.GetStash<Field>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var movable in _movingFilter)
            {
                ref var cell = ref _cellStash.Get(movable);
                ref var moving = ref _movingStash.Get(movable);
                ref var targetField = ref _fieldStash.Get(moving.TargetField);
                
                var way = targetField.Transform.position - cell.Transform.position;
                var shift = way.normalized * deltaTime;
                if (shift.magnitude < way.magnitude)
                {
                    cell.Transform.position += shift;
                }
                else
                {
                    cell.Transform.position = targetField.Transform.position;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}