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
            return (value as Dictionary<Any, Any>)[key];
        }

        public new void Set(Any key, Any value)
        {
            (this.value as Dictionary<Any, Any>)[key] = value;
        }

    }
}
