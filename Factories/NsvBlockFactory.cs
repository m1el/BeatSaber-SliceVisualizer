using SliceVisualizer.Core;
using Zenject;

namespace SliceVisualizer.Factories
{
    internal class NsvBlockFactory : IFactory<NsvSlicedBlock>
    {
        private readonly DiContainer _container;

        public NsvBlockFactory(DiContainer container)
        {
            _container = container;
        }

        public NsvSlicedBlock Create()
        {
            var slicedBlock = _container.InstantiateComponentOnNewGameObject<NsvSlicedBlock>();
            slicedBlock.gameObject.name = $"{nameof(NsvSlicedBlock)} (Factorized)";

            return slicedBlock;
        }
    }
}