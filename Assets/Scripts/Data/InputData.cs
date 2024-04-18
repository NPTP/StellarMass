using StellarMass.Input;
using UnityEngine;

namespace StellarMass.Data
{
    [CreateAssetMenu]
    public class InputData : DataScriptable
    {
        [SerializeField] private InputInfo[] inputInfos;
        public InputInfo[] InputInfos => inputInfos;
    }
}