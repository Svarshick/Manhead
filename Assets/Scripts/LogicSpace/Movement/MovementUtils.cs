using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LogicSpace
{
    public static class MovementUtils
    {
        public static async UniTask MoveTowards(CancellationToken cancellationToken, GameObject obj, Vector3 target, float speed)
        {
            while (true)
            {
                var diff = target - obj.transform.position;
                var shift = diff.normalized * speed * Time.deltaTime;
                if (shift.magnitude < diff.magnitude)
                {
                    obj.transform.position += shift;
                    await UniTask.NextFrame(cancellationToken);
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