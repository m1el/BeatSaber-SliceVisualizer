using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using SliceVisualizer.Configuration;

namespace SliceVisualizer.ViewControllers
{
    internal class MainViewController : BSMLResourceViewController
    {
        public void Activated()
        {
            NotifyPropertyChanged("EnabledValue");
        }
        [UIValue("Enabled-value")]
        public bool EnabledValue
        {
            get => PluginConfig.Instance.Enabled;
            set
            {
                PluginConfig.Instance.Enabled = value;
            }
        }

        public override string ResourceName => "SliceVisualizer.Views.Main.bsml";
    }
}