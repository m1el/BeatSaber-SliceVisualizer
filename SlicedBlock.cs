using UnityEngine;
using System;
using ScoreScalingMode = SliceVisualizer.PluginConfig.ScoreScalingMode;

namespace SliceVisualizer
{

    internal class SlicedBlock
    {
        public GameObject gameObject { get; set; }
        public Transform blockTransform { get; set; }
        public Transform sliceTransform { get; set; }
        public Transform missedAreaTransform { get; set; }
        public Transform sliceGroupTransform { get; set; }
        public SpriteRenderer background { get; set; }
        public SpriteRenderer arrow { get; set; }
        public SpriteRenderer circle { get; set; }
        public SpriteRenderer missedArea { get; set; }
        public SpriteRenderer slice { get; set; }
        private bool isDirectional;
        private float aliveTime;
        private Color color;
        private bool needsUpdate = false;
        public bool isActive { get; private set; }
        public void SetInactive()
        {
            isActive = false;
            var transparent = new Color(0f, 0f, 0f, 0f);
            background.color = transparent;
            arrow.color = transparent;
            circle.color = transparent;
            missedArea.color = transparent;
            slice.color = transparent;
        }
        public void SetCubeState(Color color, float cubeX, float cubeY, float cubeRotation, bool isDirectional)
        {
            isActive = true;
            aliveTime = 0f;
            this.color = color;
            this.isDirectional = isDirectional;

            var config = PluginConfig.Instance;
            background.color = color;
            blockTransform.localRotation = Quaternion.Euler(0f, 0f, cubeRotation);
            blockTransform.localPosition = new Vector3(cubeX, cubeY, 0f);

            var arrowAlpha = isDirectional ? 1f : 0f;
            arrow.color = Fade(config.ArrowColor, arrowAlpha);
        }
        public void SetSliceState(float sliceOffset, float sliceAngle, float cubeRotation)
        {
            var config = PluginConfig.Instance;
            switch (config.ScoreScaling)
            {
                case ScoreScalingMode.Linear:
                    break;
                case ScoreScalingMode.Log:
                    sliceOffset = ApplyScaling(sliceOffset, x => Mathf.Log(x));
                    break;
                case ScoreScalingMode.Sqrt:
                    sliceOffset = ApplyScaling(sliceOffset, x => Mathf.Sqrt(x));
                    break;
            }

            missedArea.color = config.MissedAreaColor;
            slice.color = config.SliceColor;

            sliceGroupTransform.localRotation = Quaternion.Euler(0f, 0f, sliceAngle - cubeRotation);
            sliceGroupTransform.localPosition = Vector3.zero;

            missedAreaTransform.localRotation = Quaternion.identity;
            missedAreaTransform.localScale = new Vector3(sliceOffset, 1f, 1f);

            sliceTransform.localRotation = Quaternion.identity;
            sliceTransform.localPosition = new Vector3(sliceOffset - config.SliceWidth * 0.5f, -0.5f, 0f);
        }


        private static float ApplyScaling(float distance, Func<float, float> transform)
        {
            var config = PluginConfig.Instance;
            float sign = Mathf.Sign(distance);
            distance = Mathf.Abs(distance);

            if (distance < config.ScoreScaleMin) { return 0.0f; }
            float tMin = transform(config.ScoreScaleMin);
            float tMax = transform(config.ScoreScaleMax);
            distance = (transform(distance) - tMin) / (tMax - tMin);

            return distance * sign;
        }
        public void Cleanup()
        {
            isActive = false;
            gameObject = null;
            blockTransform = null;
            sliceTransform = null;
            missedAreaTransform = null;
            background = null;
            arrow = null;
            circle = null;
            missedArea = null;
            slice = null;
        }
        private static float InvLerp(float start, float end, float x)
        {
            return Mathf.Clamp((x - start) / (end - start), 0f, 1f);
        }
        private static Color Fade(Color color, float alpha)
        {
            color.a *= alpha;
            return color;
        }
        private static Color Pop(Color color, float amount)
        {
            var white = new Color(1f, 1f, 1f, 1f);
            return color * (1.0f - amount) + amount * white;
        }
        public void Update(float delta = 0f)
        {
            var config = PluginConfig.Instance;

            if (!isActive) { return; }
            aliveTime += delta;
            if (aliveTime > config.CubeLifetime)
            {
                SetInactive();
                return;
            }

            var t = aliveTime / config.CubeLifetime;
            var blockPosition = blockTransform.localPosition;
            var arrowAlpha = isDirectional ? 1f : 0f;
            blockPosition.z = Mathf.Lerp(-config.PopDistance, config.PopDistance, t);
            blockTransform.localPosition = blockPosition;

            if (t < config.PopEnd)
            {
                var popStrength = InvLerp(config.PopEnd, 0.0f, t);
                background.color = Pop(color, popStrength);
                needsUpdate = true;
            }
            else if (t > config.FadeStart)
            {
                var fadeT = InvLerp(config.FadeStart, 1.0f, t);
                var fadeAlpha = Mathf.Lerp(1f, 0f, fadeT);
                background.color = Fade(color, fadeAlpha);
                arrow.color = Fade(config.ArrowColor, fadeAlpha * arrowAlpha);
                circle.color = Fade(config.CenterColor, fadeAlpha);
                missedArea.color = Fade(config.MissedAreaColor, fadeAlpha);
                slice.color = Fade(config.SliceColor, fadeAlpha);
                needsUpdate = true;
            }
            else if (needsUpdate)
            {
                background.color = color;
                arrow.color = Fade(config.ArrowColor, arrowAlpha);
                circle.color = config.CenterColor;
                missedArea.color = config.MissedAreaColor;
                slice.color = config.SliceColor;
                needsUpdate = false;
            }
        }
    }
}
