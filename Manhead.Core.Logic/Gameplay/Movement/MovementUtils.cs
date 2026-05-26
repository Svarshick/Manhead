using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;

namespace Manhead.Core.Logic.Gameplay.Movement;

public static class MovementUtils
{
    public static async Task MoveTowards(
        Transform transform, 
        Vector2 target,
        float speed)
    {
        while (true)
        {
            var deltaTime = (float)Time.ElapsedGameTime.TotalSeconds;
            var diff = target - transform.Position;
            var shift = Vector2.Normalize(diff) * speed * deltaTime;
            if (shift.LengthSquared() < diff.LengthSquared())
            {
                transform.Position += shift;
                await MonoTask.NextFrame();
            }
            else
            {
                transform.Position = target;
                return;
            }
        }
    }
}