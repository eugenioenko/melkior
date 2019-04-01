using System;
using System.Collections.Generic;

namespace Melkior
{

    class Dict : Any
    {

        public Dict(Dictionary<Any, Any> value) : base(value, DataType.Dict) {
            this.value = value;
        }

        public new Any Get(Any key)
        {
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

    }
}
