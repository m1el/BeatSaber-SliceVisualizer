using System;
using System.Collections.Generic;
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
        private readonly NsvSlicedBlock.Pool _slicedBlockPool;

        private readonly Dictionary<int, NsvSlicedBlock> _activeSlicedBlocks;
        private readonly Queue<int> _activeSlicedBlocksRemovalQueue;
        private GameObject _canvasGO = null!;

        public NsvController(PluginConfig config, BeatmapObjectManager beatmapObjectManager, NsvSlicedBlock.Pool slicedBlockPool)
        {
            _config = config;
            _beatmapObjectManager = beatmapObjectManager;
            _slicedBlockPool = slicedBlockPool;

            _activeSlicedBlocks = new Dictionary<int, NsvSlicedBlock>();
            _activeSlicedBlocksRemovalQueue = new Queue<int>();
        }

        public void Initialize()
        {
            _canvasGO = new GameObject("SliceVisualizerCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = _canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            _canvasGO.transform.localScale = Vector3.one * _config.CanvasScale;
            _canvasGO.transform.localPosition = _config.CanvasOffset;

            _beatmapObjectManager.noteWasCutEvent += OnNoteCut;
        }

        public void Tick()
        {
            var delta = Time.deltaTime;
            foreach (var kvp in _activeSlicedBlocks)
            {
                if (kvp.Value.ExternalUpdate(delta))
                {
                    _activeSlicedBlocksRemovalQueue.Enqueue(kvp.Key);
                }
            }

            while (_activeSlicedBlocksRemovalQueue.Count > 0)
            {
                var key = _activeSlicedBlocksRemovalQueue.Dequeue();
                _slicedBlockPool.Despawn(_activeSlicedBlocks[key]);
                _activeSlicedBlocks.Remove(key);
            }
        }

        public void Dispose()
        {
            _beatmapObjectManager.noteWasCutEvent -= OnNoteCut;

            foreach (var slicedBlock in _activeSlicedBlocks.Values)
            {
                _slicedBlockPool.Despawn(slicedBlock);
            }

            _activeSlicedBlocks.Clear();
        }

        private void OnNoteCut(NoteController noteController, NoteCutInfo noteCutInfo)
        {
            // Re-use cubes at the same column & layer to avoid UI cluttering
            var noteData = noteController.noteData;
            var blockIndex = noteData.lineIndex + 4 * (int) noteData.noteLineLayer;
            if (!_activeSlicedBlocks.TryGetValue(blockIndex, out var slicedBlock))
            {
                slicedBlock = _slicedBlockPool.Spawn();
                slicedBlock.Init((_canvasGO.transform as RectTransform)!);
                _activeSlicedBlocks.Add(blockIndex, slicedBlock);
            }

            slicedBlock.SetData(noteController, noteCutInfo, noteData);
        }
    }
}