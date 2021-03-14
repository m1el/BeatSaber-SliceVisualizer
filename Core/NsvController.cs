using System;
using SliceVisualizer.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using HMUI;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.Components.Settings;
using SiraUtil.Tools;

namespace SliceVisualizer.Core
{
    internal class NsvController : IInitializable, ITickable, IDisposable
    {
        private readonly PluginConfig _config;
        private readonly SiraLog _logger;
        private readonly BeatmapObjectManager _beatmapObjectManager;
        private readonly NsvSlicedBlock[] _slicedBlockPool;
        private readonly Factories.NsvBlockFactory _blockFactory;

        private GameObject _canvasGO = null!;

        private static readonly int MaxItems = 12;

        public NsvController(BeatmapObjectManager beatmapObjectManager, Factories.NsvBlockFactory blockFactory, SiraLog logger)
        {
            _config = PluginConfig.Instance;
            _logger = logger;
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

            try
            {
                CreateCheckbox();
            }
            catch (Exception err)
            {
                _logger.Info(string.Format("Cannot create checkbox: {0}", err));
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
                if (!slicedBlock) { continue; }
                slicedBlock.Dispose();
            }
        }
        
        private void OnNoteCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (!_config.Enabled)
            {
                return;
            }

            // Re-use cubes at the same column & layer to avoid UI cluttering
            var noteData = noteController.noteData;
            // doing the modulus twice is required for negative indices
            var lineLayer = (((int) noteController.noteData.noteLineLayer % 3) + 3) % 3;
            var lineIndex = ((noteController.noteData.lineIndex % 4) + 4) % 4;
            var blockIndex = lineIndex + 4 * lineLayer;
            var slicedBlock = _slicedBlockPool[blockIndex];
            slicedBlock.SetData(noteController, noteCutInfo, noteData);
        }

        public void CreateCheckbox()
        {
            var canvas = GameObject.Find("Wrapper/StandardGameplay/PauseMenu/Wrapper/MenuWrapper/Canvas").GetComponent<Canvas>();
            if (!canvas)
            {
                return;
            }

            var toggleObject = new ToggleSettingTag().CreateObject(canvas.transform);
            toggleObject.GetComponentInChildren<CurvedTextMeshPro>().text = "SliceVisualizer";

            if (!(toggleObject.transform is RectTransform toggleObjectTransform))
            {
                return;
            }

            toggleObjectTransform.anchoredPosition = new Vector2(27, -7);
            toggleObjectTransform.sizeDelta = new Vector2(-130, 7);

            var toggleSetting = toggleObject.GetComponent<ToggleSetting>();
            toggleSetting.Value = _config.Enabled;
            toggleSetting.toggle.onValueChanged.AddListener(enabled => _config.Enabled = enabled);
        }
    }
}