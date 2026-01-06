using LogicSpace.GameEntity;
using UnityEditor;
using UnityEngine;

namespace LogicSpace.EntityAppearance.Rules
{
    public class BaseEntityVisualizationRule : IEntityVisualizationRule
    {
        public int Priority => 0;

        public void Apply(IEntity entity, EntityVisualizationContext context)
        {
            var go = new GameObject("background");
            go.transform.SetParent(context.RootGO.transform, false);

            var squareSprite = GetStandardSquareSprite();
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.color = entity.Color;
            spriteRenderer.sprite = squareSprite;
            spriteRenderer.sortingOrder = -1;
        }

        public static Sprite GetStandardSquareSprite()
        {
            // 1. Try the known path first (Fastest)
            var knownPath = "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/Square.png";
            var s = AssetDatabase.LoadAssetAtPath<Sprite>(knownPath);
            if (s != null) return s;

            // 2. Fallback: Search the database for the file (Robust)
            // We look for a sprite named "Square" inside the 2d.sprite package
            var guids = AssetDatabase.FindAssets("Square t:sprite");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                // This ensures we get the high-res 1x1 Unity square, 
                // not some random square icon from a plugin.
                if (path.Contains("com.unity.2d.sprite")) return AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            return null;
        }
    }
}