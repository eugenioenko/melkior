using System;
using System.Collections.Generic;
using System.Text;

namespace Melkior
{
    class RangeValue
    { 
        public Any start;
        public Any end;
        public Any step;

        public RangeValue(Any start, Any end, Any step)
        {
            this.start = start;
            this.end = end;
            this.step = step;
        }

        public override bool Equals(object other)
        {
            if (!(other is RangeValue)) return false;

            return (start == ((RangeValue)other).start).IsTruthy() &&
                (end == ((RangeValue)other).end).IsTruthy() &&
                (step == ((RangeValue)other).step).IsTruthy();
        }

      
        public override int GetHashCode()
        {
            var hashCode = -1478498664;
            hashCode = hashCode * -1521134295 + EqualityComparer<Any>.Default.GetHashCode(start);
            hashCode = hashCode * -1521134295 + EqualityComparer<Any>.Default.GetHashCode(end);
            hashCode = hashCode * -1521134295 + EqualityComparer<Any>.Default.GetHashCode(step);
            return hashCode;
        }

        public static Any operator ==(RangeValue left, RangeValue right)
        {
            return  new Boolean(
                (left.start == right.start).IsTruthy() &&
                (left.end == right.end).IsTruthy() &&
                (left.step == right.step).IsTruthy()
            );
        }

        public static Any operator !=(RangeValue left, RangeValue right)
        {
            return new Boolean(
                (left.start != right.start).IsTruthy() ||
                (left.end != right.end).IsTruthy() ||
                (left.step != right.step).IsTruthy()
            );
        }
    }

    class Range : Any
    {

        public Range(RangeValue value) : base(value, DataType.Range)
        {
            this.value = value;
        }

        public void Normalize(int length)
        {
            if (((RangeValue)value).step.IsNull())
            {
                ((RangeValue)value).step = new Number(1);
            }

            if (((RangeValue)value).end.IsNull())
            {
                ((RangeValue)value).end = (double)((RangeValue)value).step.value > 0 ? new Number(length - 1) : new Number(0);
            }

            if (((RangeValue)value).start.IsNull())
            {
                ((RangeValue)value).start = (double)((RangeValue)value).step.value > 0 ? new Number(0) : new Number(length - 1);
            }
        }

        public override string ToString()
        {
            return "[range]";
        }
    }

}
