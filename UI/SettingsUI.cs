using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;

namespace SliceVisualizer.UI
{
    internal class SettingsUI
    {
        public static SliceVisualizerFlowCoordinator? SliceVisualizerFlowCoordinator;
        public static bool Created;

        public static void CreateMenu()
        {
            if (!Created)
            {
                var menuButton = new MenuButton("SliceVisualizer", "Chase the perfect slice", ShowFlow);
                MenuButtons.instance.RegisterButton(menuButton);
                Created = true;
            }
        }

        public static void ShowFlow()
        {
            if (SliceVisualizerFlowCoordinator == null)
            {
                SliceVisualizerFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<SliceVisualizerFlowCoordinator>();
            }
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(SliceVisualizerFlowCoordinator);
        }
    }
}