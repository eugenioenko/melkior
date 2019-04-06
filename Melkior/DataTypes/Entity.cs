using System;
using System.Collections.Generic;

namespace Melkior
{

    class Entity : Any
    {
        public Any constructor;

        public Entity(Dictionary<Any, Any> value, Any constructor) : base(value, DataType.Entity)
        {
            this.value = value;
            this.constructor = constructor;
        }

        public new Any Get(Any key)
        {
            var method = ((Class)constructor).Get(key);
            if (method.IsFunction())
            {
                return method;
            }

            try
            {
                return ((Dictionary<Any, Any>)value)[key];
            }
            catch
            {
                return new Any(null, DataType.Null);
            }
        }

        public new void Set(Any key, Any value)
        {
            ((Dictionary<Any, Any>)this.value)[key] = value;
        }

        public override string ToString()
        {
            return " Object " + constructor;
        }

    }
}
