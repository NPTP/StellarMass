using System;
using UnityEngine;

namespace Summoner.Utilities
{
    public class Curve
    {
        public enum Type
        {
            Square = 0,
            SmoothStep,
            EaseOutExp,
            EaseOutSquare,
            EaseOutCubic,
            EaseOutQuart
        }

        private readonly Func<float, float> curveEvaluator;
        private Curve(Curve.Type curveType) => curveEvaluator = GetCurve(curveType);
        public float Evaluate(float x) => curveEvaluator(x);
        public static implicit operator Curve(Curve.Type curveType) => new Curve(curveType);

        private static Func<float, float> GetCurve(Curve.Type curveType)
        {
            return curveType switch
            {
                Type.Square => Square,
                Type.SmoothStep => SmoothStep,
                Type.EaseOutExp => EaseOutExp,
                Type.EaseOutSquare => EaseOutSquare,
                Type.EaseOutCubic => EaseOutCubic,
                Type.EaseOutQuart => EaseOutQuart,
                _ => throw new ArgumentOutOfRangeException(nameof(curveType), curveType, null)
            };
        }
        
        public static float Square(float x)
        {
            if (x >= 1) return 1;
            return x * x;
        }

        public static float SmoothStep(float x)
        {
            if (x >= 1) return 1;
            return x * x * (3.0f - 2.0f * x);
        }

        public static float EaseOutExp(float x)
        {
            return x >= 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
        }

        public static float EaseOutSquare(float x) => EaseOutToPower(x, 2);
        public static float EaseOutCubic(float x) => EaseOutToPower(x, 3);
        public static float EaseOutQuart(float x) => EaseOutToPower(x, 4);
        private static float EaseOutToPower(float x, float p)
        {
            return 1 - Mathf.Pow(1 - x, p);
        }
    }
}