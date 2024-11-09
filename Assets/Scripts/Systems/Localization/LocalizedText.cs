using Summoner.Utilities.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Summoner.Systems.Localization
{
    [RequireComponent(typeof(TextMeshPro))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField][Required] private TextMeshPro tmpText;
        [SerializeField] private LocalizedString localizedString;

        private void Awake()
        {
            if (localizedString != null)
            {
                tmpText.text = localizedString.GetLocalizedString();
            }
        }
    }
}