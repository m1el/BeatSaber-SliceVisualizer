using UnityEngine;

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

            background.color = color;
            blockTransform.localRotation = Quaternion.Euler(0f, 0f, cubeRotation);
            blockTransform.localPosition = new Vector3(cubeX, cubeY, 0f);

            var alpha = isDirectional ? 1f : 0f;
            arrow.color = new Color(1f, 1f, 1f, alpha);
        }
        public void SetSliceState(float sliceOffset, float sliceAngle, float cubeRotation)
        {
            var sliceWidth = 0.05f;
            var useLogScaling = true;
            var missedColor = new Color(0f, 0f, 0f, 0.5f);
            var sliceColor = new Color(1f, 1f, 1f, 1f);

            if (useLogScaling)
            {
                sliceOffset = LogScaling(sliceOffset);
            }

            missedArea.color = missedColor;
            slice.color = sliceColor;

            sliceGroupTransform.localRotation = Quaternion.Euler(0f, 0f, sliceAngle - cubeRotation);
            sliceGroupTransform.localPosition = Vector3.zero;

            missedAreaTransform.localRotation = Quaternion.identity;
            missedAreaTransform.localScale = new Vector3(sliceOffset, 1f, 1f);

            sliceTransform.localRotation = Quaternion.identity;
            sliceTransform.localPosition = new Vector3(sliceOffset - sliceWidth / 2f, -0.5f, 0f);
        }


        private static float LogScaling(float distance)
        {
            float min = 0.005f;
            float max= 0.5f;
            float sign = Mathf.Sign(distance);
            distance = Mathf.Abs(distance);

            if (distance < min) { return 0.0f; }
            float logMin = Mathf.Log(min);
            float logMax = Mathf.Log(max);
            distance = (Mathf.Log(distance) - logMin) / (logMax - logMin);

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
            float lifetime = 1.0f;
            float popDistance = 0.5f;
            float fadeStart = 0.5f;
            float popEnd = 0.1f;
            var arrowColor = new Color(1f, 1f, 1f, 1f);
            var circleColor = new Color(1f, 1f, 1f, 1f);
            var missedColor = new Color(0f, 0f, 0f, 0.5f);
            var sliceColor = new Color(1f, 1f, 1f, 0.5f);

            if (!isActive) { return; }
            aliveTime += delta;
            if (aliveTime > lifetime)
            {
                SetInactive();
                return;
            }

            var t = aliveTime / lifetime;
            var blockPosition = blockTransform.localPosition;
            blockPosition.z = Mathf.Lerp(-popDistance, popDistance, t);
            blockTransform.localPosition = blockPosition;

            if (t < popEnd)
            {
                var popStrength = InvLerp(fadeStart, 0.0f, t);
                background.color = Pop(color, popStrength);
                needsUpdate = true;
            }
            else if (t > fadeStart)
            {
                var fadeT = InvLerp(fadeStart, 1.0f, t);
                var fadeAlpha = Mathf.Lerp(1f, 0f, fadeT);
                background.color = Fade(color, fadeAlpha);
                arrow.color = Fade(arrowColor, fadeAlpha);
                circle.color = Fade(circleColor, fadeAlpha);
                missedArea.color = Fade(missedColor, fadeAlpha);
                slice.color = Fade(sliceColor, fadeAlpha);
                needsUpdate = true;
            }
            else if (needsUpdate)
            {
                background.color = color;
                arrow.color = arrowColor;
                circle.color = circleColor;
                missedArea.color = missedColor;
                slice.color = sliceColor;
                needsUpdate = false;
            }
        }
    }
}
