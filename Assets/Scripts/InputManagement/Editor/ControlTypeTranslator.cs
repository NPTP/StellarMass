using System;
using System.Numerics;

namespace StellarMass.InputManagement.Editor
{
    public static class ControlTypeTranslator
    {
        /// <summary>
        /// Translates from Unity's controlType string name to the name of the relevant type that an input action uses.
        /// </summary>
        public static string Translate(string controlTypeName)
        {
            return controlTypeName switch
            {
                // NP TODO: Fill out null entries with correct types
                "Analog" => null,
                "Axis" => "float",
                "Bone" => null,
                "Delta" => null,
                "Digital" => null,
                "Double" => "double",
                "Dpad" => null,
                "Eyes" => null,
                "Integer" => "int",
                "Pose" => null,
                "Quaternion" => nameof(Quaternion),
                "Stick" => null,
                "Touch" => null,
                "Vector2" => nameof(Vector2),
                "Vector3" => nameof(Vector3),
                _ => null
            };
        }
    }
}