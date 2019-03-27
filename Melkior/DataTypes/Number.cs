using System;
namespace Melkior
{
    class Number : Any
    {

        public Number(double value) : base(value, DataType.Number) {
            this.value = value;  
        }

        public static Number operator +(Number left, Number right)
        {
            if (left.type != DataType.Number && right.type != DataType.Number)
                return new Number(double.NaN);
            return new Number((double)left.value + (double)right.value);
        }

        public static Number operator -(Number left, Number right)
        {
            if (left.type != DataType.Number && right.type != DataType.Number)
                return new Number(double.NaN);
            return new Number((double)left.value - (double)right.value);
        }

        public static Number operator /(Number left, Number right)
        {
            if (left.type != DataType.Number && right.type != DataType.Number)
                return new Number(double.NaN);
            return new Number((double)left.value / (double)right.value);
        }

        public static Number operator *(Number left, Number right)
        {
            if (left.type != DataType.Number && right.type != DataType.Number)
                return new Number(double.NaN);
            return new Number((double)left.value * (double)right.value);
        }

        public static Number operator %(Number left, Number right)
        {
            if (left.type != DataType.Number && right.type != DataType.Number)
                return new Number(double.NaN);
            return new Number((double)left.value % (double)right.value);
        }

        public static Boolean operator <(Number left, Number right)
        {
            return new Boolean((double)left.value < (double)right.value);
        }

        public static Boolean operator >(Number left, Number right)
        {
            return new Boolean((double)left.value > (double)right.value);
        }

        public static Boolean operator <=(Number left, Number right)
        {
            return new Boolean((double)left.value <= (double)right.value);
        }

        public static Boolean operator >=(Number left, Number right)
        {
            return new Boolean((double)left.value >= (double)right.value);
        }
    }

}
