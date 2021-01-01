using System.Runtime.CompilerServices;
using IPA.Config.Data;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SliceVisualizer
{
    class Dummy
    {
        public static float ValueToFloat(Value val)
        {
            if (val is FloatingPoint point)
            {
                return (float)point.Value;
            }
            else if (val is Integer integer)
            {
                return integer.Value;
            }
            else
            {
                throw new System.ArgumentException("List element was not a number");
            }
        }

    }

    public class ColorConverter : ValueConverter<Color>
    {
        public ColorConverter() { }
        public override Color FromValue(Value value, object parent)
        {
            if (value is List list)
            {
                var array = list.Select(Dummy.ValueToFloat).ToArray();
                return new Color(array[0], array[1], array[2], array[3]);
            }
            else if (value is Text text)
            {
                var color = new Color(1f, 1f, 1f, 1f);
                ColorUtility.TryParseHtmlString(text.Value, out color);
                return color;
            }
            else
            {
                throw new System.ArgumentException("Color deserializer expectes either string or array");
            }
        }

        public override Value ToValue(Color obj, object parent)
        {
            Plugin.Log.Info(string.Format("trying to serialize color: {0}", obj));
            var array = new float[] { obj.r, obj.g, obj.b, obj.a };
            return Value.From(array.Select(x => Value.Float((decimal)x)));
        }
    }

    public class VectorConverter : ValueConverter<Vector3>
    {
        public VectorConverter() { }
        public override Vector3 FromValue(Value value, object parent)
        {
            if (value is List list)
            {
                var array = list.Select(Dummy.ValueToFloat).ToArray();
                return new Vector3(array[0], array[1], array[2]);
            }
            else
            {
                throw new System.ArgumentException("Vector3 deserialization expects list of numbers");
            }
        }

        public override Value ToValue(Vector3 obj, object parent)
        {
            Plugin.Log.Info(string.Format("trying to serialize vec3: {0}", obj));
            var array = new float[] { obj.x, obj.y, obj.z };
            return Value.From(array.Select(x => Value.Float((decimal)x)));
        }
    }

    internal class PluginConfig
    {
        public enum ScoreScalingMode {
            Linear,
            Sqrt,
            Log,
        }
        public static PluginConfig Instance { get; set; }
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
        public virtual Color CenterColor { get; set; } = new Color(1f, 1f, 1f, 1f);
        public virtual float CubeLifetime { get; set; } = 1f;
        public virtual float PopEnd { get; set; } = 0.1f;
        public virtual float FadeStart { get; set; } = 0.5f;
        public virtual float PopDistance { get; set; } = 0.5f;
        public virtual bool PositionFromCubeTransform { get; set; } = false;
        public virtual bool RotationFromCubeTransform { get; set; } = false;
        public virtual float CubeScale { get; set; } = 0.9f;
        public virtual float CenterScale { get; set; } = 0.2f;
        public virtual float ArrowScale { get; set; } = 0.6f;
        [UseConverter(typeof(VectorConverter))]
        public virtual Vector3 CanvasOffset { get; set; } = new Vector3(0f, 0f, 16f);
        public virtual float CanvasScale { get; set; } = 1f;

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
    }
}