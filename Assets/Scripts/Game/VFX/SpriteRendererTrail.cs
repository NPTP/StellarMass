using DG.Tweening;
using StellarMass.Game.Data;
using UnityEngine;

namespace StellarMass.Game.VFX
{
    public class SpriteRendererTrail : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer.DOFade(0, PlayerData.TrailFadeTime)
                .From(0.75f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}