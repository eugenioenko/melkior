namespace Melkior
{
    class Boolean : Any
    {

        public Boolean(bool value) : base(value, DataType.Boolean) {
            this.value = value;
        }

        public Boolean(object value, DataType type) : base(value, type)
        {
            
        }

        public override string ToString()
        {
            return value.Equals(true) ? "true" : "false"; 
        }
    }

}
