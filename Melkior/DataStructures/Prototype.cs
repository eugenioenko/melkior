using System;
using System.Collections.Generic;

namespace Melkior
{
    class Prototype
    {
        public Dictionary<string, Any> values;
        public Prototype parent;
        public Any owner;

        public Prototype()
        {
            values = new Dictionary<string, Any>();
            parent = null;
            owner = null;
        }

        public void Set(string key, Any value)
        {
            if (values.ContainsKey(key))
            {
                values[key] = value;
            }
            else
            {
                values.Add(key, value);
            }
        }

        public Any Get(string key)
        {
            if (values.ContainsKey(key))
            {
                return values[key];
            }
            if (parent != null)
            {
                return parent.Get(key);
            }
            Console.WriteLine("[entity error] => " + key + " is not defined in ");
            throw new NotImplementedException();
            // todo add error handeling
        }
    }
}
