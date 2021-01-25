using IPA.Config.Data;
using IPA.Config.Stores;
using System.Collections.Generic;
using System.Linq;

namespace SliceVisualizer.Configuration
{
    internal class NsvListConverter<T, C>: ValueConverter<List<T>>
        where C: ValueConverter<T>, new()
    {
        private C _converter;

        public NsvListConverter()
        {
            _converter = new C();
        }

        public override Value ToValue(List<T> obj, object parent)
        {
            return Value.From(obj.Select(x => _converter.ToValue(x, parent)));
        }

        public override List<T> FromValue(Value value, object parent)
        {
            if (value is List list)
            {
                return list.Select(x => _converter.FromValue(x, parent)).ToList();
            }

            throw new System.ArgumentException("Value is not a list");
        }
    }
}
