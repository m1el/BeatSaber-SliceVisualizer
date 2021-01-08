using UnityEngine;
using System;

namespace SliceVisualizer.Models
{
    public static class ScoreScalingModeExt
    {
        public static float ApplyScaling(this ScoreScalingMode mode, float offset, float min, float max)
        {
            Func<float, float> transform = mode switch
            {
                ScoreScalingMode.Linear => (x => x),
                ScoreScalingMode.Log => Mathf.Log,
                ScoreScalingMode.Sqrt => Mathf.Sqrt,
                _ => (x => x),
            };

            var sign = Mathf.Sign(offset);
            offset = Mathf.Abs(offset);

            if (offset < min)
            {
                return 0.0f;
            }

            var tMin = transform(min);
            var tMax = transform(max);
            offset = (transform(offset) - tMin) / (tMax - tMin);

            return Mathf.Clamp(sign * offset, -0.5f, 0.5f);
        }
    }
}