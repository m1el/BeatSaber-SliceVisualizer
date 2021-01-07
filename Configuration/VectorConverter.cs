using System.Runtime.CompilerServices;
using IPA.Config.Data;
using IPA.Config.Stores;
using System.Linq;
using UnityEngine;

namespace SliceVisualizer.Configuration
{
    public class VectorConverter : ValueConverter<Vector3>
    {
        public VectorConverter() { }
        public override Vector3 FromValue(Value value, object parent)
        {
            switch (value)
            {
                case List list:
                    {
                        var array = list.Select(FloatConverter.ValueToFloat).ToArray();
                        return new Vector3(array[0], array[1], array[2]);
                    }

                default:
                    throw new System.ArgumentException("Vector3 deserialization expects list of numbers");
            }
        }

        public override Value ToValue(Vector3 obj, object parent)
        {
            var array = new float[] { obj.x, obj.y, obj.z };
            return Value.From(array.Select(x => Value.Float((decimal)x)));
        }
    }
}
