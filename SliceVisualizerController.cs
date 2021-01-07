using BS_Utils.Utilities;
using SliceVisualizer.Configuration;
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
        private static IPALogger Log = null!;
        private static readonly int maxItems = 12;
        private SlicedBlock[]? BlockBuffer;
        private BeatmapObjectManager? SpawnController;
        private ColorManager MyColorManager = null!;
        public SliceVisualizerController(IPALogger logger)
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
            Log.Info("Built visualizer scene");
        }
        public void Stahp()
        {
            if (BlockBuffer != null)
            {
                foreach (var slicedBlock in BlockBuffer)
                {
                    slicedBlock.Dispose();
                }
                BlockBuffer = null;
            }

            if (SpawnController != null)
            {
                SpawnController.noteWasCutEvent -= OnNoteCut;
                SpawnController = null;
            }
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
                BlockBuffer[ii] = new SlicedBlock(transform, material);
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


        private static SaberType OtherSaber(SaberType saber) {
            switch (saber)
            {
                case SaberType.SaberA: return SaberType.SaberB;
                case SaberType.SaberB: return SaberType.SaberA;
            }

            throw new System.ArgumentException(string.Format("Invalid saber type: {0}!", saber));
        }

        private void OnNoteCut(NoteController noteController, NoteCutInfo info)
        {
            Plugin.Log.Info("on note cut 1?");
            if (BlockBuffer == null || BlockBuffer.Length == 0) { return; }

            // Re-use cubes at the same column&layer to avoid UI cluttering
            var blockIndex = noteController.noteData.lineIndex + 4 * (int)noteController.noteData.noteLineLayer;
            var color = MyColorManager.ColorForSaberType(info.saberType);
            var otherCoor = MyColorManager.ColorForSaberType(OtherSaber(info.saberType));
            var slicedBlock = BlockBuffer[blockIndex];
            slicedBlock.SetState(noteController, info, color, otherCoor);
        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update()
        {
            var delta = Time.deltaTime;
            if (BlockBuffer == null) {
                return;
            }
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
