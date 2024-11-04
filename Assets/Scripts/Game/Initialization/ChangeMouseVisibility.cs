using StellarMass.Systems.Initialization;
using UnityEngine;

namespace StellarMass.Game.Initialization
{
    [CreateAssetMenu]
    public class ChangeMouseVisibility : ExecutableStep
    {
        [SerializeField] private bool hideCursor = true;
        
        public override void Execute()
        {
            Cursor.visible = hideCursor;
        }
    }
}