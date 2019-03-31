using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Melkior
{
    class String : Any
    {
        public String(string value) : base(value, DataType.String) {
            this.value = value;
           
        }

        public new Any Get(Any key)
        {
            if (key.IsNumber()) {
                try
                {
                    return new String(
                        (value as string)[Convert.ToInt32(key.value)].ToString()
                    );
                }
                catch
                {
                    return new Any(null, DataType.Null);
                }
            }

            if (key.IsString())
            {
                if (key.value as string == "length")
                {
                    return Length(this);
                }
            }

            if (Runtime.StringMethods.ContainsKey(key))
            {
                return Runtime.StringMethods[key];
            }

            throw new MelkiorError(key + " does not exist in" + this);
        }

        public static Number Length(String str)
        {
            return new Number((str.value as string).Length);
        }

        /// <summary>
        /// Splits a string into an array of multiple strings using an
        /// a list of strings as separator
        /// </summary>
        /// <param name="self">The string to be split</param>
        /// <param name="separators">An array of separator strings</param>
        /// <returns>An array of splitted strings</returns>
        public static Array Split(String self, Any separators)
        {
            string[] sep;
            if (separators.IsArray())
            {
                sep = (separators.value as List<Any>)
                    .ConvertAll(val => val.value as string)
                    .ToArray();
            }
            else
            {
                var list = new List<string>();
                list.Add(separators.ToString());
                sep = list.ToArray();
            }

            var splitted = (self.value as string)
                .Split(sep, StringSplitOptions.None)
                .ToList()
                .ConvertAll(val => new String(val) as Any);

            return new Array(splitted);
        }

        /// <summary>
        /// Concatenates two strings together and returns a new string
        /// </summary>
        /// <param name="self">String to concatenate</param>
        /// <param name="other">String or an Array of strings to concatanate</param>
        /// <returns>Concatanated string</returns>
        public static String Concat(String self, Any other)
        {
            if (other.IsString())
            {
                return new String(string.Concat(self.value, other.value));
            }
            if (other.IsArray())
            {
                return new String(string.Concat(self.value, Array.Join(other as Array, new String(""))));
            }
            return new String(string.Concat(self.value, other.ToString()));
        }

        public static Boolean Contains(String self, Any other)
        {
            return new Boolean((self.value as string).Contains(other.ToString()));
        }

    }

}
