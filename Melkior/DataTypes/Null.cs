namespace Melkior
{
    class Null : Any
    {

        public Null() : base(null, DataType.Null) { }

        public override string ToString()
        {
            return "null";
        }
    }

}
