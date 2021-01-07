using System.Runtime.CompilerServices;
using IPA.Config.Data;
using IPA.Config.Stores;

namespace SliceVisualizer.Configuration
{
    public class FloatConverter
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
}
