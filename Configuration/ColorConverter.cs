using IPA.Config.Data;
using IPA.Config.Stores;
using System.Linq;
using UnityEngine;

namespace SliceVisualizer.Configuration
{
    internal class ColorConverter : ValueConverter<Color>
    {
        public override Color FromValue(Value value, object parent)
        {
            switch (value)
            {
                case List list:
                    {
                        var array = list.Select(FloatConverter.ValueToFloat).ToArray();
                        return new Color(array[0], array[1], array[2], array[3]);
                    }

                case Text text:
                    {
                        return ColorUtility.TryParseHtmlString(text.Value, out var color) ? color : Color.white;
                    }

                default:
                    throw new System.ArgumentException("Color deserializer expects either string or array");
            }
        }

        public override Value ToValue(Color obj, object parent)
        {
            var array = new[] { obj.r, obj.g, obj.b, obj.a };
            return Value.From(array.Select(x => Value.Float((decimal)x)));
        }
    }
}
