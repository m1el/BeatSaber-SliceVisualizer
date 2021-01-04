using BS_Utils.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using IPALogger = IPA.Logging.Logger;

namespace SliceVisualizer
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class SliceVisualizerController : MonoBehaviour
    {
        // private static int SortingLayerID = 777;
        private static IPALogger Log;
        private SlicedBlock[] BlockBuffer;
        private int maxItems = 12;
        private BeatmapObjectManager SpawnController;
        private ColorManager MyColorManager;
        public static void Init(IPALogger logger)
        {
            Log = logger;
        }
        public void DoSomething()
        {
            SpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectExecutionRatingsRecorder>().LastOrDefault()
                .GetPrivateField<BeatmapObjectManager>("_beatmapObjectManager");
            if (SpawnController == null)
            {
                Log.Info("spawn controller not found. is this multiplayer mode?");
                return;
            }
            SpawnController.noteWasCutEvent += OnNoteCut;
            MyColorManager = GameObject.FindObjectOfType<ColorManager>();
            Build();
            Log.Info("created something?");
        }
        public void Stahp()
        {
            foreach (var slicedBlock in BlockBuffer)
            {
                slicedBlock.Cleanup();
            }
            SpawnController.noteWasCutEvent -= OnNoteCut;
            SpawnController = null;
        }

        private GameObject Build()
        {
            var gameObject = BuildCanvas();
            var transform = gameObject.GetComponent<RectTransform>();
            var material = new Material(Shader.Find("Custom/Sprite"));
            ///var material = Resources.FindObjectsOfTypeAll<Material>().FirstOrDefault(x => x.name == "UINoGlow");
            BlockBuffer = new SlicedBlock[maxItems];
            for (var ii = 0; ii < maxItems; ii++)
            {
                BlockBuffer[ii] = BuildBlock(transform, material, ii);
                Log.Info(string.Format("built another object: {0}", ii));
            }
            return gameObject;
        }

        private GameObject BuildCanvas()
        {
            var config = PluginConfig.Instance;

            var gameObject = new GameObject("SliceVisualizerCanvas");
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();
            gameObject.transform.localScale = Vector3.one * config.CanvasScale;
            gameObject.transform.localPosition = config.CanvasOffset;
            return gameObject;
        }

        private SlicedBlock BuildBlock(Transform parent, Material material, int index)
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

            slicedBlock.SetInactive();

            return slicedBlock;
        }

        static SaberType OtherSaber(SaberType saber) {
            switch (saber)
            {
                case SaberType.SaberA: return SaberType.SaberB;
                case SaberType.SaberB: return SaberType.SaberA;
            }

            throw new System.ArgumentException(string.Format("Invalid saber type: {0}!", saber));
        }

        private void OnNoteCut(NoteController noteController, NoteCutInfo info)
        {
            var config = PluginConfig.Instance;
            if (BlockBuffer.Length == 0) { return; }

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
            if (config.RotationFromCubeTransform) {
                cubeRotation = noteController.noteTransform.localRotation.eulerAngles.z;
            } else {
                DirectionToRotation.TryGetValue(noteData.cutDirection, out cubeRotation);
            }
            float cubeX;
            float cubeY;
            if (config.PositionFromCubeTransform) {
                var positionScaling = 2.0f;
                var position = noteController.noteTransform.position * positionScaling;
                cubeX = position.x;
                cubeY = position.y;
            } else {
                var positionOffset = new Vector2(-1.5f, 1.5f);
                cubeX = noteData.lineIndex + positionOffset.x;
                cubeY = (int)noteData.noteLineLayer + positionOffset.y;
            }

            var isDirectional = DirectionToRotation.ContainsKey(noteData.cutDirection);
            Color color = MyColorManager.ColorForSaberType(info.saberType);
            Color otherCoor = MyColorManager.ColorForSaberType(OtherSaber(info.saberType));

            // Re-use cubes at the same column&layer to avoid UI cluttering
            var blockIndex = noteData.lineIndex + 4 * (int)noteData.noteLineLayer;
            var slicedBlock = BlockBuffer[blockIndex];

            slicedBlock.SetCubeState(color, otherCoor, cubeX, cubeY, cubeRotation, isDirectional);
            slicedBlock.SetSliceState(sliceOffset, sliceAngle, cubeRotation, info.directionOK, info.saberTypeOK);
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

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update()
        {
            var delta = Time.deltaTime;
            foreach (var slicedBlock in BlockBuffer)
            {
                slicedBlock.Update(delta);
            }
        }

        // These methods are automatically called by Unity, you should remove any you aren't using.
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            Plugin.Log?.Debug($"{name}: Awake()");
        }
        /// <summary>
        /// Only ever called once on the first frame the script is Enabled. Start is called after any other script's Awake() and before Update().
        /// </summary>
        private void Start()
        {

        }
 
        /// <summary>
        /// Called every frame after every other enabled script's Update().
        /// </summary>
        private void LateUpdate()
        {

        }

        /// <summary>
        /// Called when the script becomes enabled and active
        /// </summary>
        private void OnEnable()
        {

        }

        /// <summary>
        /// Called when the script becomes disabled or when it is being destroyed.
        /// </summary>
        private void OnDisable()
        {

        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");

        }
        #endregion
    }
}
