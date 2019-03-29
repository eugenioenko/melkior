using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Melkior
{
    class Array : Any
    {

        public Array(List<Any> value) : base(value, DataType.Array) {
            this.value = value;
        }

        public new Any Get(Any key)
        {
            if (key.IsNumber())
            {
                try
                {
                    return (value as List<Any>)[Convert.ToInt32(key.value)];
                }
                catch
                {
                    var length = (value as List<Any>).Count;
                    return new Any(null, DataType.Null);
                }
               
            }
            if (key.IsString())
            {
                switch (key.value)
                {
                    case "length":
                        return new Number((value as List<Any>).Count);
                    case "size":
                        return Runtime.ArrayLength(value as List<Any>);
                    case "each":
                        return Runtime.ArrayEach(value as List<Any>);

                }
            }
            throw new MelkiorError(key + " does not exist in" + this);
        }

        public new void Set(Any key, Any value)
        {
            if (key.IsNumber())
            {
                var index = Convert.ToInt32(key.value);
                var length = (this.value as List<Any>).Count;

                if (index >= length)
                {
                    (this.value as List<Any>).AddRange(Enumerable.Repeat(new Any(null, DataType.Null), index - length + 1));
                }                
                 
                (this.value as List<Any>)[index] = value;
                return;
             
            }
            throw new MelkiorError(key + " is not a valid index for array " + this);
        }

        public new string ToString()
        {
            return "<Array[" + (value as List<Any>).Count + "]>";
        }
    }

}
