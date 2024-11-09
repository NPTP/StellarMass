using Summoner.Utilities.Attributes;
using UnityEngine;
using UnityEngine.U2D;

namespace Summoner.Game.Ship
{
    [ExecuteAlways]
    public class ShipScaler : MonoBehaviour
    {
        [SerializeField][GUIDisabled] private float spriteLineHeight = 0.5f;
        [Space]
        [SerializeField] private SpriteShapeController[] spriteShapeControllers;

        private void Update()
        {
            SetSpriteHeight();
        }
        
        private void SetSpriteHeight()
        {
            float scale = transform.localScale.x;
            spriteLineHeight = Mathf.Clamp(-(0.2f * scale) + 0.2f, 0.01f, 99f);
            
            for (int i = 0; i < spriteShapeControllers.Length; i++)
            {
                SpriteShapeController spriteShapeController = spriteShapeControllers[i];
                int count = spriteShapeController.spline.GetPointCount();
                for (int j = 0; j < count; j++)
                {
                    spriteShapeController.spline.SetHeight(i, spriteLineHeight);
                }
                spriteShapeController.UpdateSpriteShapeParameters();
            }
        }
    }
}