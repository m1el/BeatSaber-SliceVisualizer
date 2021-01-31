using System;
using BeatSaberMarkupLanguage;
using HMUI;
using SliceVisualizer.ViewControllers;

namespace SliceVisualizer.UI
{
    internal class SliceVisualizerFlowCoordinator : FlowCoordinator
    {
        private MainViewController? _mainViewController;
        // private BindingsViewController _bindingsViewController;
        // private MiscViewController _miscViewController;
        // private ThresholdViewController _thresholdViewController;

        public void Awake()
        {
            if (!_mainViewController)
            {
                _mainViewController = BeatSaberUI.CreateViewController<MainViewController>();
            }
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            try
            {
                if (firstActivation)
                {
                    SetTitle("SliceVisualizer settings");
                    showBackButton = true;
                    ProvideInitialViewControllers(_mainViewController);
                }
                _mainViewController?.Activated();
            }
            catch (Exception e)
            {
                Plugin.Log.Error(e);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}