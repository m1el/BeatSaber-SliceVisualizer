using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private static int SortingLayerID = 777;
        private static IPALogger Log;
        private SlicedBlock[] buffer;
        private int maxItems = 1;
        private System.Random rng;
        public static void Init(IPALogger logger)
        {
            Log = logger;
        }
        public void ShowSomething()
        {
            Build();
            Log.Info("created something?");
        }

        private GameObject Build()
        {
            var gameObject = BuildCanvas();
            var transform = gameObject.GetComponent<RectTransform>();
            var material = new Material(Shader.Find("Custom/Sprite"));
            ///var material = Resources.FindObjectsOfTypeAll<Material>().FirstOrDefault(x => x.name == "UINoGlow");
            buffer = new SlicedBlock[maxItems];
            for (var i = 0; i < maxItems; i++)
            {
                buffer[i] = BuildBlock(transform, material);
                Log.Info(string.Format("built another object: {0}", i));
            }

            return gameObject;
        }

        private GameObject BuildCanvas()
        {
            rng = new System.Random();
            var gameObject = new GameObject("TestCanvas");
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();
            gameObject.transform.position = new Vector3(0f, 0f, 10f);
            gameObject.SetActive(true);
            return gameObject;
        }

        private SlicedBlock BuildBlock(Transform parent, Material material)
        {
            /*
             * The hierarchy is the following:
             * (BlockMask
             *   RoundRect
             *   Cirle
             *   Arrow
             *   (SliceGroup
             *     MissedArea
             *     Slice)))
             */

            var cubeRotation = rng.Next(0, 360);
            var isDirectional = true;
            var cubeX = rng.Next(-2, 2);
            var cubeY = rng.Next(0, 3);
            var sliceAngle = rng.Next(360);
            var sliceOfset = rng.Next(-30, 31) / 31f;

            var cubeScale = 0.9f;
            var circleScale = 0.2f;
            var arrowScale = 0.6f;
            var sliceWidth = 0.05f;

            var slicedBlock = new SlicedBlock();

            var blockMaskGO = new GameObject("BlockMask");
            var blockTransform = blockMaskGO.AddComponent<RectTransform>();
            var blockMask = blockMaskGO.AddComponent<Mask>();
            blockMask.enabled = true;
            
            var maskImage = blockMaskGO.AddComponent<SpriteRenderer>();
            maskImage.material = null;
            maskImage.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            maskImage.sprite = Assets.RRect;

            blockTransform.SetParent(parent);
            blockTransform.localRotation = Quaternion.identity;
            blockTransform.localPosition = Vector3.zero;
            slicedBlock.gameObject = blockMaskGO;
            slicedBlock.blockTransform = blockTransform;
            blockMaskGO.SetActive(true);
            //Log.Info(string.Format("transform.rotation: {0}", blockMaskGO.transform.localRotation));
            //Log.Info(string.Format("transform.position: {0}", blockMaskGO.transform.localPosition));

            {
                // Construct note body background
                var backgroundGO = new GameObject("RoundRect");
                var background = backgroundGO.AddComponent<SpriteRenderer>();
                var backgroundTransform = backgroundGO.AddComponent<RectTransform>();
                background.material = material;
                background.sprite = Assets.RRect;
                background.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                background.sortingLayerID = SortingLayerID;
                background.sortingOrder = 0;
                backgroundTransform.SetParent(blockTransform);

                backgroundTransform.localScale = Vector3.one * cubeScale;
                backgroundTransform.localRotation = Quaternion.identity;
                var halfWidth = cubeScale * Assets.RRect.rect.width / Assets.RRect.pixelsPerUnit / 2.0f;
                var halfHeight= cubeScale * Assets.RRect.rect.height / Assets.RRect.pixelsPerUnit / 2.0f;
                backgroundTransform.localPosition = new Vector3(-halfWidth, -halfHeight, 0f);
                backgroundGO.SetActive(true);
                slicedBlock.background = background;
            }

            {
                // Construct the small circle
                var circleGO = new GameObject("Circle");
                var circle = circleGO.AddComponent<SpriteRenderer>();
                var circleTransform = circleGO.AddComponent<RectTransform>();
                circle.material = material;
                circle.sprite = Assets.Circle;
                circle.color = new Color(1f, 1f, 1f, 1f);
                circle.sortingLayerID = SortingLayerID;
                circle.sortingOrder = 1;
                circleTransform.SetParent(blockTransform);
                circleTransform.localScale = Vector3.one * circleScale;
                circleTransform.localRotation = Quaternion.identity;
                var halfWidth = circleScale * Assets.Circle.rect.width / Assets.Circle.pixelsPerUnit / 2.0f;
                var halfHeight= circleScale * Assets.Circle.rect.height / Assets.Circle.pixelsPerUnit / 2.0f;
                circleTransform.localPosition = new Vector3(-halfWidth, -halfHeight, 0f);
                circleGO.SetActive(true);
                slicedBlock.circle = circle;
            }

            {
                // Construct the directional arrow
                var arrowGO = new GameObject("Arrow");
                var arrow = arrowGO.AddComponent<SpriteRenderer>();
                var arrowTransform = arrowGO.AddComponent<RectTransform>();
                arrow.material = material;
                arrow.sprite = Assets.Arrow;
                arrow.color = new Color(1f, 1f, 1f, 1f);
                arrow.sortingLayerID = SortingLayerID;
                arrow.sortingOrder = 2;
                arrowTransform.SetParent(blockTransform);
                arrowTransform.localScale = Vector3.one * arrowScale;
                arrowTransform.localRotation = Quaternion.identity;
                var halfWidth = arrowScale * Assets.Arrow.rect.width / Assets.Arrow.pixelsPerUnit / 2.0f;
                var centerOffset = halfWidth - arrowScale * Assets.Arrow.rect.height / Assets.Arrow.pixelsPerUnit;
                arrowTransform.localPosition = new Vector3(-halfWidth, centerOffset, 0f);
                arrowGO.SetActive(true);
                slicedBlock.arrow = arrow;
            }

            {
                // Construct the slice UI
                var sliceGroupGO = new GameObject("SliceGroup");
                var sliceGroupTransform = sliceGroupGO.AddComponent<RectTransform>();
                sliceGroupTransform.SetParent(blockTransform);
                sliceGroupTransform.anchoredPosition3D = new Vector3(0.5f, 0.5f, 0f);
                sliceGroupTransform.localRotation = Quaternion.identity;
                sliceGroupTransform.localPosition = Vector3.zero;
                slicedBlock.sliceGroupTransform = sliceGroupTransform;

                {
                    // missed are background
                    var missedAreaGO = new GameObject("MissedArea");
                    var missedArea = missedAreaGO.AddComponent<SpriteRenderer>();
                    var missedAreaTransform = missedAreaGO.AddComponent<RectTransform>();
                    missedArea.material = material;
                    missedArea.sprite = Assets.White;
                    missedArea.color = new Color(0f, 0f, 0f, 1f);
                    missedArea.sortingLayerID = SortingLayerID;
                    missedArea.sortingOrder = 3;
                    missedAreaTransform.SetParent(sliceGroupTransform);
                    missedAreaTransform.localRotation = Quaternion.identity;
                    missedAreaTransform.localScale = new Vector3(0f, 1f, 1f);
                    missedAreaTransform.localPosition = new Vector3(0f, -0.5f, 0f);
                    slicedBlock.missedAreaTransform = missedAreaTransform;
                }

                {
                    // slice line
                    var sliceGO = new GameObject("Slice");
                    var slice = sliceGO.AddComponent<SpriteRenderer>();
                    var sliceTransform = sliceGO.AddComponent<RectTransform>();
                    slice.material = material;
                    slice.sprite = Assets.White;
                    slice.color = new Color(1f, 1f, 1f, 1f);
                    slice.sortingLayerID = SortingLayerID;
                    slice.sortingOrder = 4;
                    sliceTransform.SetParent(sliceGroupTransform);
                    sliceTransform.localScale = new Vector3(sliceWidth, 1f, 1f);
                    sliceTransform.localRotation = Quaternion.identity;
                    sliceTransform.localPosition = new Vector3(-sliceWidth / 2f, -0.5f, 0f);
                    slicedBlock.sliceTransform = sliceTransform;
                }
            }

            slicedBlock.SetCubeState(cubeX, cubeY, cubeRotation, isDirectional);
            slicedBlock.SetSliceState(sliceOfset, sliceAngle, cubeRotation);

            return slicedBlock;
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
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update()
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
