using SliceVisualizer.Configuration;
using SliceVisualizer.Core;
using SliceVisualizer.Factories;
using Zenject;

namespace SliceVisualizer.Installers
{
    internal class NsvGameInstaller : Installer<NsvGameInstaller>
    {
        public NsvGameInstaller()
        {
        }

        public override void InstallBindings()
        {
            Container.BindFactory<NsvSlicedBlock, NsvBlockFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<NsvController>().AsSingle();
        }
    }
}