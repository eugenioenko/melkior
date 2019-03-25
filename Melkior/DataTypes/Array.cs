using System;
using System.Collections.Generic;
using System.Text;


namespace Melkior
{
    class Array : Any
    {
        public new readonly List<Any> value;

        public Array(List<Any> value) : base(value, DataType.Array) {
            this.value = value;
        }

        public new Any Get(Any key)
        {
            if (key.IsNumber())
            {
                return value[Convert.ToInt32(key.value)];
            }
            throw new MelkiorError(key + " does not exist in" + this);
        }

        public new void Set(Any key, Any value)
        {
            if (key.IsNumber())
            {
                 this.value[(int)key.value] = value;
            }
            throw new MelkiorError(key + " is not a valid index for array " + this);
        }

        public new string ToString()
        {
            return "<Array[" + value.Count + "]>";
        }
    }

}