using SliceVisualizer.Configuration;
using SliceVisualizer.Models;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SliceVisualizer.Core
{
    internal class NsvSlicedBlock : MonoBehaviour
    {
        private PluginConfig _config = null!;
        private ColorManager _colorManager = null!;

        private Transform _blockTransform = null!;
        private Transform _sliceTransform = null!;
        private Transform _missedAreaTransform = null!;
        private Transform _sliceGroupTransform = null!;
        private SpriteRenderer _background = null!;
        private SpriteRenderer _arrow = null!;
        private SpriteRenderer _circle = null!;
        private SpriteRenderer _missedArea = null!;
        private SpriteRenderer _slice = null!;

        private float _aliveTime;
        private bool _isDirectional;
        private Color _color;
        private Color _saberColor;
        private Color _arrowColor;
        private Color _missedAreaColor;
        private Color _sliceColor;
        private bool _needsUpdate;

        [Inject]
        internal void Construct(PluginConfig config, NsvAssetLoader assetLoader, ColorManager colorManager)
        {
            _colorManager = colorManager;
            _config = config;

            BuildNote(assetLoader);
        }

        private void BuildNote(NsvAssetLoader assetLoader)
        {
            /*
             * The hierarchy is the following:
             * (RootBlockGroup
             *   RoundRect
             *   Circle
             *   Arrow
             *	  (SliceGroup
             *		MissedArea
             *		Slice))
             */

            _blockTransform = gameObject.AddComponent<RectTransform>();
            gameObject.AddComponent<RectMask2D>();

            _blockTransform.localScale = Vector3.one * _config.CubeScale;
            _blockTransform.localRotation = Quaternion.identity;
            _blockTransform.localPosition = Vector3.zero;

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
                background.material = assetLoader.UINoGlowMaterial;
                // background.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                // background.sortingLayerID = SortingLayerID;
                background.sortingOrder = -4;
                backgroundTransform.SetParent(_blockTransform);
                backgroundTransform.localScale = Vector3.one;
                backgroundTransform.localRotation = Quaternion.identity;
                if (assetLoader.RRect != null)
                {
                    background.sprite = assetLoader.RRect;
                    var halfWidth = assetLoader.RRect.rect.width / assetLoader.RRect.pixelsPerUnit / 2.0f;
                    var halfHeight = assetLoader.RRect.rect.height / assetLoader.RRect.pixelsPerUnit / 2.0f;
                    backgroundTransform.localPosition = new Vector3(-halfWidth, -halfHeight, 0f);
                }

                _background = background;
            }

            {
                // Construct the small circle in the center
                var circleGO = new GameObject("Circle");
                var circle = circleGO.AddComponent<SpriteRenderer>();
                var circleTransform = circleGO.AddComponent<RectTransform>();
                circle.material = assetLoader.UINoGlowMaterial;
                circle.color = _config.CenterColor;
                // circle.sortingLayerID = SortingLayerID;
                circle.sortingOrder = -3;
                circleTransform.SetParent(_blockTransform);
                circleTransform.localScale = Vector3.one * _config.CenterScale;
                circleTransform.localRotation = Quaternion.identity;
                if (assetLoader.Circle != null)
                {
                    var sprite = assetLoader.Circle;
                    circle.sprite = sprite;
                    var halfWidth = _config.ArrowScale * sprite.rect.width / sprite.pixelsPerUnit / 2.0f;
                    var centerOffset = halfWidth - _config.ArrowScale * sprite.rect.height / sprite.pixelsPerUnit;
                    circleTransform.localPosition = new Vector3(-halfWidth, centerOffset, 0f);
                }

                _circle = circle;
            }

            {
                // Construct the directional arrow
                var arrowGO = new GameObject("Arrow");
                var arrow = arrowGO.AddComponent<SpriteRenderer>();
                var arrowTransform = arrowGO.AddComponent<RectTransform>();
                arrow.material = assetLoader.UINoGlowMaterial;
                arrow.color = _config.ArrowColor;
                // arrow.sortingLayerID = SortingLayerID;
                arrow.sortingOrder = -2;
                arrowTransform.SetParent(_blockTransform);
                arrowTransform.localScale = Vector3.one * _config.ArrowScale;
                arrowTransform.localRotation = Quaternion.identity;
                if (assetLoader.Arrow != null)
                {
                    var sprite = assetLoader.Arrow;
                    arrow.sprite = sprite;
                    var halfWidth = _config.ArrowScale * sprite.rect.width / sprite.pixelsPerUnit / 2.0f;
                    var centerOffset = halfWidth - _config.ArrowScale * sprite.rect.height / sprite.pixelsPerUnit;
                    arrowTransform.localPosition = new Vector3(-halfWidth, centerOffset, 0f);
                }

                _arrow = arrow;
            }

            {
                // Construct the slice UI
                var sliceGroupGO = new GameObject("SliceGroup");
                var sliceGroupTransform = sliceGroupGO.AddComponent<RectTransform>();
                sliceGroupTransform.SetParent(_blockTransform);
                sliceGroupTransform.localScale = Vector3.one;
                sliceGroupTransform.localRotation = Quaternion.identity;
                sliceGroupTransform.localPosition = Vector3.zero;
                _sliceGroupTransform = sliceGroupTransform;

                {
                    // missed area background
                    var missedAreaGO = new GameObject("MissedArea");
                    var missedArea = missedAreaGO.AddComponent<SpriteRenderer>();
                    var missedAreaTransform = missedAreaGO.AddComponent<RectTransform>();
                    missedArea.material = assetLoader.UINoGlowMaterial;
                    missedArea.sprite = assetLoader.White;
                    missedArea.color = _config.MissedAreaColor;
                    // missedArea.sortingLayerID = SortingLayerID;
                    missedArea.sortingOrder = -1;
                    // missedArea.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    missedAreaTransform.SetParent(sliceGroupTransform);
                    missedAreaTransform.localScale = new Vector3(0f, 1f, 1f);
                    missedAreaTransform.localRotation = Quaternion.identity;
                    missedAreaTransform.localPosition = new Vector3(0f, -0.5f, 0f);
                    _missedAreaTransform = missedAreaTransform;
                    _missedArea = missedArea;
                }

                {
                    // slice line
                    var sliceGO = new GameObject("Slice");
                    var slice = sliceGO.AddComponent<SpriteRenderer>();
                    var sliceTransform = sliceGO.AddComponent<RectTransform>();
                    slice.material = assetLoader.UINoGlowMaterial;
                    slice.sprite = assetLoader.White;
                    slice.color = _config.SliceColor;
                    // slice.sortingLayerID = SortingLayerID;
                    slice.sortingOrder = 0;
                    // slice.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    sliceTransform.SetParent(sliceGroupTransform);
                    sliceTransform.localScale = new Vector3(_config.SliceWidth, 1f, 1f);
                    sliceTransform.localRotation = Quaternion.identity;
                    sliceTransform.localPosition = new Vector3(-_config.SliceWidth / 2f, -0.5f, 0f);
                    _sliceTransform = sliceTransform;
                    _slice = slice;
                }
            }
        }

        internal void Init(RectTransform parent)
        {
            gameObject.GetComponent<RectTransform>().SetParent(parent);
        }

        internal void SetData(NoteController noteController, NoteCutInfo noteCutInfo, NoteData noteData)
        {
            // Extract cube rotation from actual cube rotation
            var cubeRotation = _config.RotationFromCubeTransform
                ? noteController.noteTransform.localRotation.eulerAngles.z
                : noteData.cutDirection.RotationAngle();
            // Using built-in rotationAngle conversion.
            // Please note that it's range is from [-180° <=> 180°[ compared to [0° <=> 360°[, but that shouldn't make a difference.

            SetCubeState(noteController, noteCutInfo, noteData, cubeRotation);

            SetSliceState(noteController, noteCutInfo, cubeRotation);
        }

        private void SetCubeState(NoteController noteController, NoteCutInfo noteCutInfo, NoteData noteData, float cubeRotation)
        {
            float cubeX;
            float cubeY;
            if (_config.PositionFromCubeTransform)
            {
                const float positionScaling = 2.0f;
                var position = noteController.noteTransform.position * positionScaling;
                cubeX = position.x;
                cubeY = position.y;
            }
            else
            {
                var positionOffset = new Vector2(-1.5f, 1.5f);
                cubeX = noteData.lineIndex + positionOffset.x;
                cubeY = (int) noteData.noteLineLayer + positionOffset.y;
            }

            _aliveTime = 0f;

            _isDirectional = noteData.cutDirection != NoteCutDirection.Any && noteData.cutDirection != NoteCutDirection.None;

            if (_config.UseCustomColors)
            {
                _color = noteData.colorType == ColorType.ColorA ? _config.LeftColor : _config.RightColor;
                _saberColor = noteData.colorType != ColorType.ColorA ? _config.LeftColor : _config.RightColor;
            }
            else
            {
                // This should work due to colors of both type A and B match for their respective saber and blocks
                _color = _colorManager.ColorForType(noteData.colorType);
                _saberColor = _colorManager.ColorForSaberType(noteCutInfo.saberType);
            }

            _background.color = _color;
            _blockTransform.localRotation = Quaternion.Euler(0f, 0f, cubeRotation);
            _blockTransform.localPosition = new Vector3(cubeX, cubeY, 0f);

            var arrowAlpha = _isDirectional ? 1f : 0f;
            _arrow.color = Fade(_config.ArrowColor, arrowAlpha);
        }

        private void SetSliceState(NoteController noteController, NoteCutInfo noteCutInfo, float cubeRotation)
        {
            var combinedDirection = new Vector3(-noteCutInfo.cutNormal.y, noteCutInfo.cutNormal.x, 0f);
            var sliceAngle = Mathf.Atan2(combinedDirection.y, combinedDirection.x) * Mathf.Rad2Deg;
            // The default cube rotation is "arrow down", so we rotate slices 90 degrees counter-clockwise
            sliceAngle += 90f;
            var sliceOffset = noteCutInfo.cutDistanceToCenter;
            // why is this different?
            // var sliceOffset = Vector3.Dot(info.cutNormal, info.cutPoint

            // Cuts to the left of center have a negative offset
            var center = noteController.noteTransform.position;
            if (Vector3.Dot(noteCutInfo.cutNormal, noteCutInfo.cutPoint - center) > 0f)
            {
                sliceOffset = -sliceOffset;
            }

            sliceOffset = _config.ScoreScaling.ApplyScaling(sliceOffset, _config.ScoreScaleMin, _config.ScoreScaleMax);

            if (noteCutInfo.saberTypeOK)
            {
                _missedAreaColor = _config.MissedAreaColor;
                _sliceColor = _config.SliceColor;
            }
            else
            {
                _missedAreaColor = _saberColor;
                _sliceColor = _saberColor;
            }

            _arrowColor = noteCutInfo.directionOK ? _config.ArrowColor : _config.BadDirectionColor;

            _arrow.color = _arrowColor;
            _missedArea.color = _missedAreaColor;
            _slice.color = _slice.color;

            _sliceGroupTransform.localRotation = Quaternion.Euler(0f, 0f, sliceAngle - cubeRotation);
            _sliceGroupTransform.localPosition = Vector3.zero;

            _missedAreaTransform.localRotation = Quaternion.identity;
            _missedAreaTransform.localScale = new Vector3(sliceOffset, 1f, 1f);

            _sliceTransform.localRotation = Quaternion.identity;
            _sliceTransform.localPosition = new Vector3(sliceOffset - _config.SliceWidth * 0.5f, -0.5f, 0f);
        }

        /// <returns>
        ///	Returns a boolean indicating whether it should be removed from the active tracking pool or not.
        /// </returns>
        internal bool ExternalUpdate(float delta = 0f)
        {
            _aliveTime += delta;
            if (_aliveTime > _config.CubeLifetime)
            {
                return true;
            }

            var t = _aliveTime / _config.CubeLifetime;
            var blockPosition = _blockTransform.localPosition;
            var arrowAlpha = (_isDirectional ? 1f : 0f) * _config.UIOpacity;
            blockPosition.z = Mathf.Lerp(-_config.PopDistance, _config.PopDistance, t);
            _blockTransform.localPosition = blockPosition;

            // calculate pop strength
            var popStrength = 0f;
            if (_aliveTime < _config.PopEnd)
            {
                popStrength = InvLerp(_config.PopEnd, 0.0f, t);
                _needsUpdate = true;
            }

            // calculate fade out opacity
            var alpha = _config.UIOpacity;
            if (_aliveTime > _config.FadeStart)
            {
                var fadeT = InvLerp(_config.FadeStart, 1.0f, t);
                alpha *= Mathf.Lerp(1f, 0f, fadeT);
                _needsUpdate = true;
            }

            if (_needsUpdate)
            {
                _background.color = Fade(Pop(_color, popStrength), alpha);
                _arrow.color = Fade(_arrowColor, arrowAlpha * alpha);
                _circle.color = Fade(_config.CenterColor, alpha);
                _missedArea.color = Fade(_missedAreaColor, alpha);
                _slice.color = Fade(_sliceColor, alpha);
                _needsUpdate = false;
            }

            return false;
        }

        private static Color Fade(Color color, float alpha)
        {
            color.a *= alpha;
            return color;
        }

        private static float InvLerp(float start, float end, float x)
        {
            return Mathf.Clamp((x - start) / (end - start), 0f, 1f);
        }

        private static Color Pop(Color color, float amount)
        {
            return color * (1.0f - amount) + amount * Color.white;
        }

        internal class Pool : MemoryPool<NsvSlicedBlock>
        {
            protected override void OnCreated(NsvSlicedBlock item) => item.gameObject.SetActive(false);

            protected override void OnSpawned(NsvSlicedBlock item) => item.gameObject.SetActive(true);

            protected override void OnDespawned(NsvSlicedBlock item) => item.gameObject.SetActive(false);
        }
    }
}