using SliceVisualizer.Configuration;
using SliceVisualizer.Core;
using SliceVisualizer.Factories;
using Zenject;

namespace SliceVisualizer.Installers
{
    internal class NsvGameInstaller : Installer<NsvGameInstaller>
    {
        private readonly PluginConfig _pluginConfig;

        public NsvGameInstaller(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        public override void InstallBindings()
        {
            if (!_pluginConfig.Enabled)
            {
                return;
            }

            Container.BindMemoryPool<NsvSlicedBlock, NsvSlicedBlock.Pool>().WithInitialSize(8).WithMaxSize(12).FromFactory<NsvBlockFactory>();
            Container.BindInterfacesAndSelfTo<NsvController>().AsSingle();
        }
    }
}