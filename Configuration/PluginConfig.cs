using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using SiraUtil.Converters;
using UnityEngine;
using SliceVisualizer.Models;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SliceVisualizer.Configuration
{
    internal class PluginConfig
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual float SliceWidth { get; set; } = 0.05f;

        [UseConverter(typeof(EnumConverter<ScoreScalingMode>))]
        public virtual ScoreScalingMode ScoreScaling { get; set; } = ScoreScalingMode.Linear;
        public virtual float ScoreScaleMin { get; set; } = 0.05f;
        public virtual float ScoreScaleMax { get; set; } = 0.5f;
        public virtual float ScoreScale { get; set; } = 1.0f;
        [UseConverter(typeof(ColorConverter))]
        public virtual Color MissedAreaColor { get; set; } = new Color(0f, 0f, 0f, 0.5f);
        [UseConverter(typeof(ColorConverter))]
        public virtual Color SliceColor { get; set; } = new Color(1f, 1f, 1f, 1f);
        [UseConverter(typeof(ColorConverter))]
        public virtual Color ArrowColor { get; set; } = new Color(1f, 1f, 1f, 1f);
        [UseConverter(typeof(ColorConverter))]
        public virtual Color BadDirectionColor { get; set; } = new Color(0f, 0f, 0f, 1f);
        [UseConverter(typeof(ColorConverter))]
        public virtual Color CenterColor { get; set; } = new Color(1f, 1f, 1f, 1f);
        public virtual bool UseCustomColors { get; set; } = false;
        [UseConverter(typeof(ColorConverter))]
        public virtual Color LeftColor { get; set; } = new Color(0.659f, 0.125f, 0.125f, 1f);
        [UseConverter(typeof(ColorConverter))]
        public virtual Color RightColor { get; set; } = new Color(0.125f, 0.392f, 0.659f, 1f);
        public virtual float CubeLifetime { get; set; } = 1f;
        public virtual float PopEnd { get; set; } = 0.1f;
        public virtual float FadeStart { get; set; } = 0.5f;
        public virtual float PopDistance { get; set; } = 0.5f;
        public virtual bool PositionFromCubeTransform { get; set; } = true;
        public virtual bool RotationFromCubeTransform { get; set; } = true;
        public virtual float CubeScale { get; set; } = 0.9f;
        public virtual float CenterScale { get; set; } = 0.2f;
        public virtual float ArrowScale { get; set; } = 0.6f;
        public virtual float UIOpacity { get; set; } = 1.0f;
        [UseConverter(typeof(Vector3Converter))]
        public virtual Vector3 CanvasOffset { get; set; } = new Vector3(0f, 0f, 16f);
        public virtual float CanvasScale { get; set; } = 1f;
    }
}
