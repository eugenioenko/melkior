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
                    return new Any(null, DataType.Null);
                }
            }
            if (key.IsString())
            {
                if (key.value as string == "length")
                {
                    return new Number((value as List<Any>).Count);
                }

                if (Runtime.ArrayMethods.ContainsKey(key))
                {
                    return Runtime.ArrayMethods[key];
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
                    (this.value as List<Any>).AddRange(
                        Enumerable.Repeat(new Any(null, DataType.Null), index - length + 1)
                    );
                }                
                 
                (this.value as List<Any>)[index] = value;
                return;
             
            }

            throw new MelkiorError(key + " is not a valid index for array " + this);
        }

        public static String Join(Array array, String separator)
        {
            return new String(
                string
                    .Join(separator.value as string, (array.value as List<Any>)
                    .Select(r => r.ToString()))
            );
        }

        public override string ToString()
        {
            return "[" +
                string.Join(", ", (value as List<Any>)
                .Select(r => r.ToString()))
             + "]";
        }
    }

}
