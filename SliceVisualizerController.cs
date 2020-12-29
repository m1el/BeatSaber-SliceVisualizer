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

        private static IPALogger Log;
        private SlicedBlock[] buffer;
        private int maxItems = 12;
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
             *   Arrow
             *   (SliceGroup
             *     MissedArea
             *     Slice)))
             */

            var cubeScale = 0.9f;
            var circleScale = 0.2f;
            var arrowScale = 1.0f;
            var decalOffset = 0.005f;

            var blockMaskGO = new GameObject("BlockMask");
            var blockTransform = blockMaskGO.AddComponent<RectTransform>();
            var blockMask = blockMaskGO.AddComponent<Mask>();
            blockMask.enabled = true;
            
            var maskImage = blockMaskGO.AddComponent<SpriteRenderer>();
            maskImage.material = null;
            maskImage.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            maskImage.sprite = Assets.RRect;

            blockTransform.SetParent(parent);
            blockMaskGO.SetActive(true);
            var position = new Vector3(rng.Next(-2, 2), rng.Next(0, 3), 0f);
            var pivot = new Vector3(0.5f, 0.5f, 0f) + position;
            blockTransform.localPosition = position;
            blockTransform.RotateAround(pivot, new Vector3(0f, 0f, 1f), rng.Next(0, 360));
            //Log.Info(string.Format("transform.rotation: {0}", blockMaskGO.transform.localRotation));
            //Log.Info(string.Format("transform.position: {0}", blockMaskGO.transform.localPosition));

            var backgroundGO = new GameObject("RoundRect");
            var background = backgroundGO.AddComponent<SpriteRenderer>();
            var backgroundTransform = backgroundGO.AddComponent<RectTransform>();
            background.material = material;
            background.sprite = Assets.RRect;
            background.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
            backgroundTransform.SetParent(blockTransform);

            backgroundTransform.localScale =  Vector3.one * cubeScale;
            backgroundTransform.localRotation = Quaternion.identity;
            var cubeHMargin = (1.0f - cubeScale * Assets.RRect.rect.width / Assets.RRect.pixelsPerUnit) / 2.0f;
            var cubeVMargin = (1.0f - cubeScale * Assets.RRect.rect.height / Assets.RRect.pixelsPerUnit) / 2.0f;
            backgroundTransform.localPosition = new Vector3(cubeHMargin, cubeVMargin, 0f);
            backgroundGO.SetActive(true);

            var arrowGO = new GameObject("Arrow");
            var arrow = arrowGO.AddComponent<SpriteRenderer>();
            var arrowTransform = arrowGO.AddComponent<RectTransform>();
            arrow.material = material;
            arrow.sprite = Assets.Arrow;
            arrow.color = new Color(1f, 1f, 1f, 1f);
            arrowTransform.SetParent(blockTransform);
            arrowTransform.localScale = Vector3.one * arrowScale;
            arrowTransform.localRotation = Quaternion.identity;
            var arrowHMargin = (1.0f - arrowScale * Assets.Arrow.rect.width / Assets.Arrow.pixelsPerUnit) / 2.0f;
            var arrowVMargin = 1.0f - arrowHMargin - Assets.Arrow.rect.height / Assets.Arrow.pixelsPerUnit;
            arrowTransform.localPosition = new Vector3(arrowHMargin, arrowVMargin, -decalOffset);
            arrowGO.SetActive(true);

            var circleGO = new GameObject("Circle");
            var circle = circleGO.AddComponent<SpriteRenderer>();
            var circleTransform = circleGO.AddComponent<RectTransform>();
            circle.material = material;
            circle.sprite = Assets.Circle;
            circle.color = new Color(1f, 1f, 1f, 1f);
            circleTransform.SetParent(blockTransform);
            circleTransform.localScale = Vector3.one * circleScale;
            circleTransform.localRotation = Quaternion.identity;
            var circleHMargin = (1.0f - circleScale * Assets.Circle.rect.width / Assets.Circle.pixelsPerUnit) / 2.0f;
            var circleVMargin = (1.0f - circleScale * Assets.Circle.rect.height / Assets.Circle.pixelsPerUnit) / 2.0f;
            circleTransform.localPosition = new Vector3(circleHMargin, circleVMargin, -decalOffset);
            circleGO.SetActive(true);

            return new SlicedBlock
            {
                //gameObject = blockMaskGO,
                background = background,
                //arrow = arrow,
            };
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
