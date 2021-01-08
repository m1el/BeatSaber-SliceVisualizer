using IPA.Logging;
using SiraUtil;
using SliceVisualizer.Configuration;
using Zenject;

namespace SliceVisualizer.Installers
{
    internal class NsvAppInstaller : Installer<Logger, PluginConfig, NsvAppInstaller>
    {
        private readonly Logger _logger;
        private readonly PluginConfig _config;

        public NsvAppInstaller(Logger logger, PluginConfig config)
        {
            _logger = logger;
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindLoggerAsSiraLogger(_logger);

            Container.BindInstance(_config).AsSingle();

            Container.BindInterfacesAndSelfTo<NsvAssetLoader>().AsSingle().Lazy();
        }
    }
}