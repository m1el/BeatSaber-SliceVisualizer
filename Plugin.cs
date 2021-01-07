using IPA;
using IPA.Config;
using IPA.Config.Stores;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace SliceVisualizer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; } = null!;
        internal static SliceVisualizerController Controller = null!;
        internal static GameObject ControllerObj = null!;
        internal static IPALogger Log { get; private set; } = null!;

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(Config conf, IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Assets.Init(logger);
            Controller = new SliceVisualizerController(logger);

            Log.Info("SliceVisualizer initialized.");

            PluginConfig.Instance = conf.Generated<PluginConfig>();

            Log.Debug("Config loaded");
        }

        private void GameSceneLoaded()
        {
            Log.Info("Game scene loaded, probably game start");
            if (PluginConfig.Instance.Enabled)
            {
                Controller.DoSomething();
            }
        }

        private void MenuSceneLoaded()
        {
            Log.Info("Menu scene loaded, probably game end");
            Controller.Stahp();
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            BS_Utils.Utilities.BSEvents.gameSceneLoaded += GameSceneLoaded;
            BS_Utils.Utilities.BSEvents.menuSceneLoaded += MenuSceneLoaded;
            ControllerObj = new GameObject("SliceVisualizerController");
            Controller = ControllerObj.AddComponent<SliceVisualizerController>();
            ControllerObj.SetActive(true);
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            ControllerObj.SetActive(false);
            BS_Utils.Utilities.BSEvents.menuSceneLoaded -= MenuSceneLoaded;
            BS_Utils.Utilities.BSEvents.gameSceneLoaded -= GameSceneLoaded;
            Log.Debug("OnApplicationQuit");
        }
    }
}
