namespace Melkior
{
    class Boolean : Any
    {

        public Boolean(bool value) : base(value, DataType.Boolean) {
            this.value = value;
        }

        public override string ToString()
        {
            return value.Equals(true) ? "true" : "false"; 
        }
    }

}
