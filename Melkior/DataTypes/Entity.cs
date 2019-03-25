using System;
using System.Collections.Generic;

namespace Melkior
{

    class Entity: Any
    {
        private readonly Prototype proto;

        public Entity(object value) : base(value)
        {
          
            proto = new Prototype();
        }

        public Entity(object value, DataType type) : base(value, type)
        {
            proto = new Prototype();
        }

      

    }
   
}
