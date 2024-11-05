using DG.Tweening;
using UnityEngine;
using StellarMass.Systems.Data.Persistent;

namespace StellarMass.Game.VFX
{
    public class SpriteRendererTrail : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer.DOFade(0, PersistentData.Player.TrailFadeTime)
                .From(0.75f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}