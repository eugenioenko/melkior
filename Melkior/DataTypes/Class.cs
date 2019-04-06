using System;
using System.Collections.Generic;

namespace Melkior
{

    class Class : Any
    {
        public Any parent;
        public Any name;

        public Class(Any name, Dictionary<Any, Any> value, Any parent) : base(value, DataType.Class)
        {
            this.name = name;
            this.value = value;
            this.parent = parent;
        }

        public new Any Get(Any key)
        {

            if (((Dictionary<Any, Any>)value).ContainsKey(key))
            {
                return ((Dictionary<Any, Any>)value)[key];
            }
            if (parent.IsClass())
            {
                return parent.Get(name);
            }
            return new Null();
        }

        public new void Set(Any key, Any value)
        {
            throw new MelkiorException("Methods of classes cannot be set dinamycally. Trying to set '" + key + "' in class '" + this + "'");
        }

        public override string ToString()
        {
            return (string)name.value;
        }

    }
}
