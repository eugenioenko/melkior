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

        /// <summary>
        ///  Determines whether one string can be found in another
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other">The string to search</param>
        /// <returns>true or false</returns>
        public static Boolean Contains(String self, Any other)
        {
            return new Boolean((self.value as string).Contains(other.ToString()));
        }

        /// <summary>
        /// Determines whether one string ends with another
        /// </summary>
        /// <param name="self">The string on which to search</param>
        /// <param name="other">The string to search</param>
        /// <returns>boolean true or false</returns>
        public static Boolean EndsWith(String self, Any other)
        {
            return new Boolean((self.value as string).EndsWith(other.ToString()));
        }

        /// <summary>
        /// Gets the zero based index of the first occurrence of one string in another
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other">The string to search</param>
        /// <returns>If found the zero based index of the occurrence. If not found returns -1</returns>
        public static Number IndexOf(String self, Any other)
        {
            return new Number((self.value as string).IndexOf(other.ToString()));
        }

        /// <summary>
        /// Gets the zero based index of the last occurrence of one string in another
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other">The string to search</param>
        /// <returns>If found the zero based index of the occurrence. If not found returns -1</returns>
        public static Number LastIndexOf(String self, Any other)
        {
            return new Number((self.value as string).LastIndexOf(other.ToString()));
        }

        /// <summary>
        /// Replaces old value with new value in the self string and returns it
        /// </summary>
        /// <param name="self">The string on which to replace</param>
        /// <param name="old">The string to be replaced</param>
        /// <param name="new">The string to replace all the occurences of oldValue</param>
        /// <returns>Returns a new string with all the ocurrences of old value replaced with new value</returns>
        public static String Replace(String self, Any oldValue, Any newValue)
        {
            return new String((self.value as string).Replace(oldValue.ToString(), newValue.ToString()));
        }

        /// <summary>
        /// Determines whether one string starts with another
        /// </summary>
        /// <param name="self">The string on which to search</param>
        /// <param name="other">The string to search</param>
        /// <returns>true or false</returns>
        public static Boolean StartsWith(String self, Any other)
        {
            return new Boolean((self.value as string).StartsWith(other.ToString()));
        }

        /// <summary>
        /// Returns the string converted to lowercase
        /// </summary>
        /// <param name="self">The string to be converted</param>
        /// <returns>new lowercased string</returns>
        public static String ToLower(String self)
        {
            return new String((self.value as string).ToLower());
        }

        /// <summary>
        /// Returns the string converted to uppercase
        /// </summary>
        /// <param name="self">The string to be converted</param>
        /// <returns>new upercased string</returns>
        public static String ToUpper(String self)
        {
            return new String((self.value as string).ToUpper());
        }

        /// <summary>
        /// Retrives a part of the string starting with the specified zero based start index and length
        /// </summary>
        /// <param name="self">The string from which to retrive</param>
        /// <param name="start">Zero based index from where to start</param>
        /// <param name="length">The length of the retrived string</param>
        /// <returns></returns>
        public static String SubString(String self, Number start, Any length)
        {
            if(length.IsNull())
            {
                return new String((self.value as string).Substring(start.ToInteger()));
            }
            else
            {
                return new String((self.value as string).Substring(start.ToInteger(), length.ToInteger()));
            }
        }

    }

}
