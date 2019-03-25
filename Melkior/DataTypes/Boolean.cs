namespace Melkior
{
    class Boolean : Entity
    {
        public new readonly bool value;

        public Boolean(bool value) : base(value, DataType.Boolean) {
            this.value = value;
        }

        public Boolean(object value, DataType type) : base(value, type)
        {

        }


    }

}
