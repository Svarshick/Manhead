using CustomMath;
using LogicSpace.GameEntity;
using UnityEngine;

namespace LogicSpace.EntityAppearance.Rules
{
    public class ArrowEntityVisualizationRule : IEntityVisualizationRule
    {
        public int Priority => 0;

        public void Apply(IEntity entity, EntityVisualizationContext context)
        {
            ProcessSide(entity.FrontSide, Direction.Up, context);
            ProcessSide(entity.LeftSide, Direction.Left, context);
            ProcessSide(entity.BackSide, Direction.Down, context);
            ProcessSide(entity.RightSide, Direction.Right, context);
        }

        private void ProcessSide(IEntitySide sideData, Direction sidePosition,
            EntityVisualizationContext ctx)
        {
            var arrow = sideData.GetComponent<Crossroad>();
            if (arrow != null && !ctx.IsConsumed(arrow))
            {
                ctx.Consume(arrow);
                var go = new GameObject("arrow");
                go.transform.SetParent(ctx.RootGO.transform, false);

                var arrowSprite = Resources.Load<Sprite>("right-arrow");
                var spriteRenderer = go.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = arrowSprite;

                go.transform.position = sidePosition.ToVector2() * 0.3f;
                go.transform.localScale = new Vector3(0.07f, 0.1f, 0.1f);
                var lookDirection = arrow.rotationDirection.ToVector2();
                var angleDegrees = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                go.transform.rotation = Quaternion.Euler(0, 0, angleDegrees);
            }
        }
    }
}