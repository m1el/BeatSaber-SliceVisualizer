using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SliceVisualizer.Configuration
{
    internal partial class PluginConfig
    {
        public static PluginConfig Instance { get; set; } = null!;
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
        [UseConverter(typeof(VectorConverter))]
        public virtual Vector3 CanvasOffset { get; set; } = new Vector3(0f, 0f, 16f);
        public virtual float CanvasScale { get; set; } = 1f;

        #region listeners
        /*
        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            Plugin.Log.Info("congfig onreload");
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            Plugin.Log.Info("config changed");
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            Plugin.Log.Info("copy from not implemented?");
            // This instance's members populated from other
        }
        */
        #endregion
    }
}
