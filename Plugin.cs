using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace SliceVisualizer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static SliceVisualizerController Controller;
        internal static GameObject ControllerObj;
        internal static IPALogger Log { get; private set; }

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Assets.Init(logger);
            SliceVisualizerController.Init(logger);
            Log.Info("SliceVisualizer initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        private void GameSceneLoaded()
        {
            Log.Info("Game scene loaded, probably game start");
            Controller.DoSomething();
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
