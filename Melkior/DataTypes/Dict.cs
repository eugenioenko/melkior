using System;
using System.Collections.Generic;

namespace Melkior
{

    class Dict : Any
    {
        public new Dictionary<Any, Any> value;

        public Dict(Dictionary<Any, Any> value) : base(value, DataType.Dict) {
            this.value = value;
        }

        public new Any Get(Any key)
        {
            return value[key];
        }

        public new void Set(Any key, Any value)
        {
            this.value[key] = value;
        }

    }
}
