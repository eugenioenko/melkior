using System;
using System.Collections.Generic;
using System.Text;


namespace Melkior
{
    class String : Any
    {
        public new string value;

        public String(string value) : base(value, DataType.String) {
            this.value = value;
        }

        public new Any Get(Any key)
        {
            if (key.value.ToString() == "length")
            {
                return new Number(value.Length);
            }
            throw new MelkiorError(key + " does not exist in" + this);
        }
    } 

}