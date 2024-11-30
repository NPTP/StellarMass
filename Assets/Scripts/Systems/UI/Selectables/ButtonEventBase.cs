using UnityEngine.UI;

namespace Summoner.Systems.UI.Selectables
{
    public class ButtonEventBase : Button
    {
        protected override void OnValidate()
        {
            base.OnValidate();
            transition = Transition.None;
        }
    }
}
