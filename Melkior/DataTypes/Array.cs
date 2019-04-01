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
                    return ((List<Any>)value)[Convert.ToInt32(key.value)];
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
                    return new Number(((List<Any>)value).Count);
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
                var length = ((List<Any>)this.value).Count;

                if (index >= length)
                {
                    ((List<Any>)this.value).AddRange(
                        Enumerable.Repeat(new Any(null, DataType.Null), index - length + 1)
                    );
                }                
                 
                ((List<Any>)this.value)[index] = value;
                return;
             
            }

            throw new MelkiorError(key + " is not a valid index for array " + this);
        }

        public static String Join(Array array, String separator)
        {
            return new String(
                string
                    .Join((string)separator.value, ((List<Any>)array.value)
                    .Select(r => r.ToString()))
            );
        }

        public override string ToString()
        {
            return "[" +
                string.Join(", ", ((List<Any>)value)
                .Select(r => r.ToString()))
             + "]";
        }
    }

}
