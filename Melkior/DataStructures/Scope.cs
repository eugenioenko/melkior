using System;
using System.Collections.Generic;

namespace Melkior
{
    class Scope
    {
        private Dictionary<string, Any> values;
        private Scope parent;

        public Scope(Scope parent = null)
        {
            values = new Dictionary<string, Any>();
            this.parent = parent;
        }
        
        public void Patch(string name, Any value)
        {
            if (values.ContainsKey(name))
            {
                values[name] = value;
            }
            else
            {
                values.Add(name, value);
            }
        }

        public bool Define(string name, Any value)
        {
            if (values.ContainsKey(name))
            {
                return false;
            }
            Patch(name, value);
            return true;
        }

        public bool Assign(string name, Any value)
        {
            if (values.ContainsKey(name))
            {
                Patch(name, value);
                return true;
            }
            else if (parent != null)
            {
                parent.Assign(name, value);
            }
            return false;
        }

        public Any Get(string name)
        {
            if (values.ContainsKey(name))
            {
                return values[name];
            }
            if (parent != null)
            {
                return parent.Get(name);
            }
            return new Null();
        }
        
    }
}
