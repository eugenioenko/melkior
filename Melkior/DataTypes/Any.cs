using System;
using System.Collections.Generic;
using System.Text;

namespace Melkior
{
    class Any : IEquatable<Any>
    {
        public object value;
        public readonly DataType type;

        public Any(object value, DataType type)
        {
            this.value = value;
            this.type = type;
        }

        public Any(object value)
        {
            this.value = value;
            this.type = DataType.Any;
        }

        public Any Get(Any key)
        {
            if (this is String) return (this as String).Get(key);
            if (this is Entity) return (this as Entity).Get(key);
            if (this is Number) return (this as Number).Get(key);
            if (this is Array) return (this as Array).Get(key);
            if (this is Dict) return (this as Dict).Get(key);
            throw new MelkiorError(key + " does not exist in" + this);
        }

        public bool IsArray()
        {
            return type == DataType.Array;
        }

        public bool IsNumber()
        {
            return type == DataType.Number;
        }

        public bool IsString()
        {
            return type == DataType.String;
        }

        public void Set(Any key, Any value)
        {
            if (this is Array)
            {
                (this as Array).Set(key, value);
                return;
            }
            if (this is Dict)
            {
                (this as Dict).Set(key, value);
                return;
            }
            throw new MelkiorError(key + " does not exist in" + this);
        }

        public static Any operator +(Any left, Any right)
        {
            if (left.type == DataType.Number && right.type == DataType.Number)
            {
                return new Number((double)left.value + (double)right.value);
            }
            return new String(left.value.ToString() + right.value.ToString());
        }


        public static Any operator -(Any left, Any right)
        {
            return new Number(Double.NaN);
        }

        public static Any operator /(Any left, Any right)
        {
            return new Number(Double.NaN);
        }

        public static Any operator *(Any left, Any right)
        {
            return new Number(Double.NaN);
        }

        public static Any operator %(Any left, Any right)
        {
            return new Number(Double.NaN);
        }

        public static Any operator |(Any left, Any right)
        {
            return new Number(Double.NaN);
        }

        public static Any operator ^(Any left, Any right)
        {
            throw new NotImplementedException();
        }

        public static Any operator >(Any left, Any right)
        {
            return new Number(Double.NaN);
        }

        public static Any operator >=(Any left, Any right)
        {
            return new Number(Double.NaN);
        }

        public static Any operator <(Any left, Any right)
        {
            return new Number(Double.NaN);
        }

        public static Any operator <=(Any left, Any right)
        {
            return new Number(Double.NaN);
        }

        public static Any operator ==(Any left, Any right)
        {
            if (left.type != right.type)
            {
                return new Boolean(false);
            }

            if (left.type == DataType.Null && right.type == DataType.Null)
            {
                return new Boolean(true);
            }
            return new Boolean(left.value.Equals(right.value));
        }

        public static Any operator !=(Any left, Any right)
        {
            if (left.type != right.type)
            {
                return new Boolean(true);
            }
            return new Boolean(!left.value.Equals(right.value));
        }

        public static Any operator !(Any left)
        {
            return new Boolean(!left.ToBoolean());
        }

        public bool ToBoolean()
        {
            if (type == DataType.Null)
            {
                return false;
            }
            if (type == DataType.Boolean)
            {
                return (bool)value;
            }
            if (type == DataType.Number && value.Equals(0.0))
            {
                return false;
            }
            if (type == DataType.String && value.ToString().Length == 0)
            {
                return false;
            }
            if (value == null)
            {
                throw new NullReferenceException();
            }
            return true;
        }

        public override string ToString()
        {
            if (value == null) return "null";
            if (IsArray()) return (this as Array).ToString();
            return value.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Any);
        }

        public bool Equals(Any other)
        {
            return value.Equals(other.value);

        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

    }
}
