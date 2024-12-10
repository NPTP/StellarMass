using System;
using UnityEngine;

namespace Summoner.Utilities.CurveUtilities
{
    /// <summary>
    /// Instance of evaluate-able curve.
    /// Enum CurveType is implicitly convertible to a new Curve struct.
    /// </summary>
    public readonly struct Curve
    {
        public static Curve Default => new Curve(CurveType.EaseOutQuad);
        
        public static implicit operator Curve(CurveType curveType) => new Curve(curveType);
        
        private readonly Func<float, float> curveEvaluator;
        
        private Curve(CurveType curveType) => curveEvaluator = GetCurveEvaluator(curveType);

        public float Evaluate(float x) => Mathf.Clamp01(curveEvaluator(x));

        private static Func<float, float> GetCurveEvaluator(CurveType curveType)
        {
            return curveType switch
            {
                CurveType.EaseOutExp => Curves.EaseOutExp,
                CurveType.EaseOutQuad => Curves.EaseOutQuad,
                CurveType.EaseOutCubic => Curves.EaseOutCubic,
                CurveType.EaseOutQuart => Curves.EaseOutQuart,
                CurveType.Quad => Curves.Quad,
                CurveType.SmoothStep => Curves.SmoothStep,
                _ => throw new ArgumentOutOfRangeException(nameof(curveType), curveType, null)
            };
        }
    }
}