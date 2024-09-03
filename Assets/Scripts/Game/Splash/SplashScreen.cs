using FMODUnity;
using StellarMass.Utilities.FMODUtilities;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private EventReference logoAppearEvent;

    public void PlayLogoAppearSound()
    {
        logoAppearEvent.PlayOneShot();
    }
}
