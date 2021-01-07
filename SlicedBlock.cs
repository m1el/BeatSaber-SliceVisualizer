using UnityEngine;
using System;
using ScoreScalingMode = SliceVisualizer.PluginConfig.ScoreScalingMode;
using System.Collections.Generic;

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
        private Color otherColor;
        private Color missedAreaColor;
        private Color sliceColor;
        private Color arrowColor;
        private bool needsUpdate = false;
        public bool isActive { get; private set; }

        public static SlicedBlock Build(Transform parent, Material material)
        {
            /*
             * The hierarchy is the following:
             * (BlockGroup
             *   RoundRect
             *   Cirle
             *   Arrow
             *   (SliceGroup
             *     MissedArea
             *     Slice))
             */
            var config = PluginConfig.Instance;

            // var SortingLayerID = SliceVisualizerController.SortingLayerID + index;
            var slicedBlock = new SlicedBlock();

            var blockGroupGO = new GameObject("BlockGroup");
            var blockTransform = blockGroupGO.AddComponent<RectTransform>();

            blockTransform.SetParent(parent);
            blockTransform.localScale = Vector3.one * config.CubeScale;
            blockTransform.localRotation = Quaternion.identity;
            blockTransform.localPosition = Vector3.zero;
            slicedBlock.gameObject = blockGroupGO;
            slicedBlock.blockTransform = blockTransform;

            /*
            {
                var blockMaskGO = new GameObject("BlockMask");
                var maskTransform = blockMaskGO.AddComponent<RectTransform>();
                maskTransform.SetParent(blockTransform);
                maskTransform.localScale = Vector3.one;
                maskTransform.localRotation = Quaternion.identity;
                var halfWidth = Assets.RRect.rect.width / Assets.RRect.pixelsPerUnit / 2.0f;
                var halfHeight = Assets.RRect.rect.height / Assets.RRect.pixelsPerUnit / 2.0f;
                maskTransform.localPosition = new Vector3(-halfWidth, -halfHeight, 0f);

                var blockMask = blockMaskGO.AddComponent<SpriteMask>();
                blockMask.sprite = Assets.RRect;
                blockMask.backSortingOrder = -4;
                blockMask.frontSortingOrder = 0;
            }
            */
            //Log.Info(string.Format("transform.rotation: {0}", blockMaskGO.transform.localRotation));
            //Log.Info(string.Format("transform.position: {0}", blockMaskGO.transform.localPosition));

            {
                // Construct the note body
                var backgroundGO = new GameObject("RoundRect");
                var background = backgroundGO.AddComponent<SpriteRenderer>();
                var backgroundTransform = backgroundGO.AddComponent<RectTransform>();
                background.material = material;
                background.sprite = Assets.RRect;
                // background.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                // background.sortingLayerID = SortingLayerID;
                background.sortingOrder = -4;
                backgroundTransform.SetParent(blockTransform);
                backgroundTransform.localScale = Vector3.one;
                backgroundTransform.localRotation = Quaternion.identity;
                var halfWidth = Assets.RRect.rect.width / Assets.RRect.pixelsPerUnit / 2.0f;
                var halfHeight = Assets.RRect.rect.height / Assets.RRect.pixelsPerUnit / 2.0f;
                backgroundTransform.localPosition = new Vector3(-halfWidth, -halfHeight, 0f);
                slicedBlock.background = background;
            }

            {
                // Construct the small circle in the center
                var circleGO = new GameObject("Circle");
                var circle = circleGO.AddComponent<SpriteRenderer>();
                var circleTransform = circleGO.AddComponent<RectTransform>();
                circle.material = material;
                circle.sprite = Assets.Circle;
                circle.color = config.CenterColor;
                // circle.sortingLayerID = SortingLayerID;
                circle.sortingOrder = -3;
                circleTransform.SetParent(blockTransform);
                circleTransform.localScale = Vector3.one * config.CenterScale;
                circleTransform.localRotation = Quaternion.identity;
                var halfWidth = config.CenterScale * Assets.Circle.rect.width / Assets.Circle.pixelsPerUnit / 2.0f;
                var halfHeight = config.CenterScale * Assets.Circle.rect.height / Assets.Circle.pixelsPerUnit / 2.0f;
                circleTransform.localPosition = new Vector3(-halfWidth, -halfHeight, 0f);
                slicedBlock.circle = circle;
            }

            {
                // Construct the directional arrow
                var arrowGO = new GameObject("Arrow");
                var arrow = arrowGO.AddComponent<SpriteRenderer>();
                var arrowTransform = arrowGO.AddComponent<RectTransform>();
                arrow.material = material;
                arrow.sprite = Assets.Arrow;
                arrow.color = config.ArrowColor;
                // arrow.sortingLayerID = SortingLayerID;
                arrow.sortingOrder = -2;
                arrowTransform.SetParent(blockTransform);
                arrowTransform.localScale = Vector3.one * config.ArrowScale;
                arrowTransform.localRotation = Quaternion.identity;
                var halfWidth = config.ArrowScale * Assets.Arrow.rect.width / Assets.Arrow.pixelsPerUnit / 2.0f;
                var centerOffset = halfWidth - config.ArrowScale * Assets.Arrow.rect.height / Assets.Arrow.pixelsPerUnit;
                arrowTransform.localPosition = new Vector3(-halfWidth, centerOffset, 0f);
                slicedBlock.arrow = arrow;
            }

            {
                // Construct the slice UI
                var sliceGroupGO = new GameObject("SliceGroup");
                var sliceGroupTransform = sliceGroupGO.AddComponent<RectTransform>();
                sliceGroupTransform.SetParent(blockTransform);
                sliceGroupTransform.localScale = Vector3.one;
                sliceGroupTransform.localRotation = Quaternion.identity;
                sliceGroupTransform.localPosition = Vector3.zero;
                slicedBlock.sliceGroupTransform = sliceGroupTransform;

                {
                    // missed area background
                    var missedAreaGO = new GameObject("MissedArea");
                    var missedArea = missedAreaGO.AddComponent<SpriteRenderer>();
                    var missedAreaTransform = missedAreaGO.AddComponent<RectTransform>();
                    missedArea.material = material;
                    missedArea.sprite = Assets.White;
                    missedArea.color = config.MissedAreaColor;
                    // missedArea.sortingLayerID = SortingLayerID;
                    missedArea.sortingOrder = -1;
                    // missedArea.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    missedAreaTransform.SetParent(sliceGroupTransform);
                    missedAreaTransform.localScale = new Vector3(0f, 1f, 1f);
                    missedAreaTransform.localRotation = Quaternion.identity;
                    missedAreaTransform.localPosition = new Vector3(0f, -0.5f, 0f);
                    slicedBlock.missedAreaTransform = missedAreaTransform;
                    slicedBlock.missedArea = missedArea;
                }

                {
                    // slice line
                    var sliceGO = new GameObject("Slice");
                    var slice = sliceGO.AddComponent<SpriteRenderer>();
                    var sliceTransform = sliceGO.AddComponent<RectTransform>();
                    slice.material = material;
                    slice.sprite = Assets.White;
                    slice.color = config.SliceColor;
                    // slice.sortingLayerID = SortingLayerID;
                    slice.sortingOrder = 0;
                    // slice.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    sliceTransform.SetParent(sliceGroupTransform);
                    sliceTransform.localScale = new Vector3(config.SliceWidth, 1f, 1f);
                    sliceTransform.localRotation = Quaternion.identity;
                    sliceTransform.localPosition = new Vector3(-config.SliceWidth / 2f, -0.5f, 0f);
                    slicedBlock.sliceTransform = sliceTransform;
                    slicedBlock.slice = slice;
                }
            }

            slicedBlock.SetActive(false);

            return slicedBlock;
        }

        public void SetActive(bool isActive)
        {
            this.isActive = isActive;
            var config = PluginConfig.Instance;
            blockTransform.localScale = Vector3.one * (isActive ? config.CubeScale : 0f);
            background.gameObject.SetActive(isActive);
            arrow.gameObject.SetActive(isActive);
            circle.gameObject.SetActive(isActive);
            circle.gameObject.SetActive(isActive);
            missedArea.gameObject.SetActive(isActive);
            slice.gameObject.SetActive(isActive);
        }
        public void SetState(NoteController noteController, NoteCutInfo info, Color color, Color otherColor)
        {
            var config = PluginConfig.Instance;

            var combinedDirection = new Vector3(-info.cutNormal.y, info.cutNormal.x, 0f);
            float sliceAngle = Mathf.Atan2(combinedDirection.y, combinedDirection.x) * Mathf.Rad2Deg;
            // The default cube rotation is "arrow down", so we rotate slices 90 degrees counter-clockwise
            sliceAngle += 90f;
            var sliceOffset = info.cutDistanceToCenter;
            // why is this different?
            // var sliceOffset = Vector3.Dot(info.cutNormal, info.cutPoint

            // Cuts to the left of center have a negative offset
            var center = noteController.noteTransform.position;
            if (Vector3.Dot(info.cutNormal, info.cutPoint - center) > 0f)
            {
                sliceOffset = -sliceOffset;
            }

            var noteData = noteController.noteData;
            // Extract cube rotation from actual cube rotation
            float cubeRotation = 0f;
            if (config.RotationFromCubeTransform)
            {
                cubeRotation = noteController.noteTransform.localRotation.eulerAngles.z;
            }
            else
            {
                DirectionToRotation.TryGetValue(noteData.cutDirection, out cubeRotation);
            }
            float cubeX;
            float cubeY;
            if (config.PositionFromCubeTransform)
            {
                var positionScaling = 2.0f;
                var position = noteController.noteTransform.position * positionScaling;
                cubeX = position.x;
                cubeY = position.y;
            }
            else
            {
                var positionOffset = new Vector2(-1.5f, 1.5f);
                cubeX = noteData.lineIndex + positionOffset.x;
                cubeY = (int)noteData.noteLineLayer + positionOffset.y;
            }

            var isDirectional = DirectionToRotation.ContainsKey(noteData.cutDirection);

            this.SetCubeState(color, otherColor, cubeX, cubeY, cubeRotation, isDirectional);
            this.SetSliceState(sliceOffset, sliceAngle, cubeRotation, info.directionOK, info.saberTypeOK);
            this.SetActive(true);
        }

        private static readonly Dictionary<NoteCutDirection, float> DirectionToRotation = new Dictionary<NoteCutDirection, float>()
        {
            [NoteCutDirection.Down] = 0f,
            [NoteCutDirection.DownRight] = 45f,
            [NoteCutDirection.Right] = 90f,
            [NoteCutDirection.UpRight] = 135f,
            [NoteCutDirection.Up] = 180f,
            [NoteCutDirection.UpLeft] = 225f,
            [NoteCutDirection.Left] = 270f,
            [NoteCutDirection.DownLeft] = 315f,
        };

        private void SetCubeState(Color color, Color otherColor, float cubeX, float cubeY, float cubeRotation, bool isDirectional)
        {
            isActive = true;
            aliveTime = 0f;
            this.color = color;
            this.otherColor = otherColor;
            this.isDirectional = isDirectional;

            var config = PluginConfig.Instance;
            background.color = color;
            blockTransform.localRotation = Quaternion.Euler(0f, 0f, cubeRotation);
            blockTransform.localPosition = new Vector3(cubeX, cubeY, 0f);

            var arrowAlpha = isDirectional ? 1f : 0f;
            arrow.color = Fade(config.ArrowColor, arrowAlpha);
        }

        private void SetSliceState(float sliceOffset, float sliceAngle, float cubeRotation, bool directionOk, bool saberTypeOk)
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

            sliceOffset = Mathf.Clamp(sliceOffset, -0.5f, 0.5f);

            if (saberTypeOk)
            {
                missedAreaColor = config.MissedAreaColor;
                sliceColor = config.SliceColor;
            }
            else
            {
                missedAreaColor = otherColor;
                sliceColor = otherColor;
            }

            if (directionOk)
            {
                arrowColor = config.ArrowColor;
            }
            else
            {
                arrowColor = config.BadDirectionColor;
            }

            missedArea.color = missedAreaColor;
            slice.color = sliceColor;
            arrow.color = arrowColor;

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
                SetActive(false);
                return;
            }

            var t = aliveTime / config.CubeLifetime;
            var blockPosition = blockTransform.localPosition;
            var arrowAlpha = (isDirectional ? 1f : 0f) * config.UIOpacity;
            blockPosition.z = Mathf.Lerp(-config.PopDistance, config.PopDistance, t);
            blockTransform.localPosition = blockPosition;

            // calculate pop strength
            var popStrength = 0f;
            if (aliveTime < config.PopEnd)
            {
                popStrength = InvLerp(config.PopEnd, 0.0f, t);
                needsUpdate = true;
            }

            // calculate fade out opacity
            var alpha = config.UIOpacity;
            if (aliveTime > config.FadeStart)
            {
                var fadeT = InvLerp(config.FadeStart, 1.0f, t);
                alpha *= Mathf.Lerp(1f, 0f, fadeT);
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                background.color = Fade(Pop(color, popStrength), alpha);
                arrow.color = Fade(arrowColor, arrowAlpha * alpha);
                circle.color = Fade(config.CenterColor, alpha);
                missedArea.color = Fade(missedAreaColor, alpha);
                slice.color = Fade(sliceColor, alpha);
                needsUpdate = false;
            }
        }
    }
}
