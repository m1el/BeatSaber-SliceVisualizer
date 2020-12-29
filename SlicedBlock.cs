using UnityEngine;

namespace SliceVisualizer
{
    internal class SlicedBlock
    {
        public GameObject gameObject { get; set; }
        public RectTransform blockTransform { get; set; }
        public SpriteRenderer background { get; set; }
        public SpriteRenderer arrow { get; set; }
        public SpriteRenderer circle { get; set; }
        public void SetCubeState(float cubeX, float cubeY, float rotation, bool isDirectional)
        {
            var position = new Vector3(cubeX, cubeY, 0f);
            var pivot = new Vector3(0.5f, 0.5f, 0f) + position;
            blockTransform.localPosition = position;
            blockTransform.rotation = Quaternion.identity;
            blockTransform.RotateAround(pivot, new Vector3(0f, 0f, 1f), rotation);
            var alpha = isDirectional ? 1f : 0f;
            arrow.color = new Color(1f, 1f, 1f, alpha);
        }
    }
}
