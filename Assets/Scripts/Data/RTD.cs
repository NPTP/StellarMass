using UnityEngine;
using Utilities;

namespace StellarMass.Data
{
    /// <summary>
    /// RTD = "Run Time Data". Abbreviated for more concise code in the project.
    /// </summary>
    public class RTD : ClosedSingleton<RTD>
    {
        private const string PREFAB_RESOURCES_PATH = "RunTimeData";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateInstanceFromPrefab()
        {
            GameObject runTimeDataPrefab = Resources.Load<GameObject>(PREFAB_RESOURCES_PATH);
            GameObject instance = Instantiate(runTimeDataPrefab);
            DontDestroyOnLoad(instance);
            instance.name = PREFAB_RESOURCES_PATH;
        }

        [SerializeField] private AudioData audioData;
        public static AudioData Audio => PrivateInstance.audioData;

        [SerializeField] private InputData inputData;
        public static InputData Input => PrivateInstance.inputData;
    }
}