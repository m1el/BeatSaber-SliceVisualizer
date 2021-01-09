using System;
using SliceVisualizer.Configuration;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SliceVisualizer.Core
{
    internal class NsvController : IInitializable, ITickable, IDisposable
    {
        private readonly PluginConfig _config;
        private readonly BeatmapObjectManager _beatmapObjectManager;
        private readonly NsvSlicedBlock[] _slicedBlockPool;
        private readonly Factories.NsvBlockFactory _blockFactory;

        private GameObject _canvasGO = null!;

        private static readonly int MaxItems = 12;

        public NsvController(PluginConfig config, BeatmapObjectManager beatmapObjectManager, Factories.NsvBlockFactory blockFactory)
        {
            _config = config;
            _beatmapObjectManager = beatmapObjectManager;
            _slicedBlockPool = new NsvSlicedBlock[MaxItems];
            _blockFactory = blockFactory;
        }

        public void Initialize()
        {
            _canvasGO = new GameObject("SliceVisualizerCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = _canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            _canvasGO.transform.localScale = Vector3.one * _config.CanvasScale;
            _canvasGO.transform.localPosition = _config.CanvasOffset;
            _beatmapObjectManager.noteWasCutEvent += OnNoteCut;

            for (var i = 0; i < MaxItems; i++)
            {
                _slicedBlockPool[i] = _blockFactory.Create();
                _slicedBlockPool[i].Init(_canvasGO.transform);
            }
        }

        public void Tick()
        {
            var delta = Time.deltaTime;
            foreach (var slicedBlock in _slicedBlockPool)
            {
                if (slicedBlock.isActive)
                {
                    slicedBlock.ExternalUpdate(delta);
                }
            }
        }

        public void Dispose()
        {
            _beatmapObjectManager.noteWasCutEvent -= OnNoteCut;

            foreach (var slicedBlock in _slicedBlockPool)
            {
                slicedBlock.Dispose();
            }
        }

        private void OnNoteCut(NoteController noteController, NoteCutInfo noteCutInfo)
        {
            // Re-use cubes at the same column & layer to avoid UI cluttering
            var noteData = noteController.noteData;
            var blockIndex = noteData.lineIndex + 4 * (int) noteData.noteLineLayer;
            var slicedBlock = _slicedBlockPool[blockIndex];
            slicedBlock.SetData(noteController, noteCutInfo, noteData);
        }
    }
}