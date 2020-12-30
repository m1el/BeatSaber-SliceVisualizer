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
        public void SetCubeState(float cubeX, float cubeY, float cubeRotation, bool isDirectional)
        {
            blockTransform.localRotation = Quaternion.Euler(0f, 0f, cubeRotation);
            blockTransform.localPosition = new Vector3(cubeX, cubeY, 0f) + new Vector3(0.5f, 0.5f, 0f);

            var alpha = isDirectional ? 1f : 0f;
            arrow.color = new Color(1f, 1f, 1f, alpha);
        }
        public void SetSliceState(float sliceOffset, float sliceAngle, float cubeRotation)
        {
            var sliceWidth = 0.05f;
            var position = blockTransform.localPosition;
            var pivot = new Vector3(0.5f, 0.5f, 0f) + position;

            sliceGroupTransform.localRotation = Quaternion.Euler(0f, 0f, sliceAngle - cubeRotation);
            sliceGroupTransform.localPosition = Vector3.zero;

            missedAreaTransform.localRotation = Quaternion.identity;
            missedAreaTransform.localScale = new Vector3(sliceOffset, 1f, 1f);

            sliceTransform.localRotation = Quaternion.identity;
            sliceTransform.localPosition = new Vector3(sliceOffset - sliceWidth / 2f, -0.5f, 0f);
        }
    }
}
