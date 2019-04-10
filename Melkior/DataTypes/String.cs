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
                        (value.ToString())[Convert.ToInt32(key.value)].ToString()
                    );
                }
                catch
                {
                    return new Any(null, DataType.Null);
                }
            }

            if (key.IsString())
            {
                if (key.value.ToString() == "length")
                {
                    return Length(this);
                }
            }

            if (Runtime.StringMethods.ContainsKey(key))
            {
                return Runtime.StringMethods[key];
            }

            throw new MelkiorException(key + " does not exist in" + this);
        }

        public static Number Length(String str)
        {
            return new Number((str.value.ToString()).Length);
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
                    .ConvertAll(val => val.ToString())
                    .ToArray();
            }
            else
            {
                var list = new List<string>(){ separators.ToString() };
                sep = list.ToArray();
            }

            var splitted = (self.value.ToString())
                .Split(sep, StringSplitOptions.None)
                .ToList()
                .ConvertAll(val => (Any)new String(val));

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
            return new Boolean((self.value.ToString()).Contains(other.ToString()));
        }

        /// <summary>
        /// Determines whether one string ends with another
        /// </summary>
        /// <param name="self">The string on which to search</param>
        /// <param name="other">The string to search</param>
        /// <returns>boolean true or false</returns>
        public static Boolean EndsWith(String self, Any other)
        {
            return new Boolean((self.value.ToString()).EndsWith(other.ToString()));
        }

        /// <summary>
        /// Gets the zero based index of the first occurrence of one string in another
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other">The string to search</param>
        /// <returns>If found the zero based index of the occurrence. If not found returns -1</returns>
        public static Number IndexOf(String self, Any other)
        {
            return new Number((self.value.ToString()).IndexOf(other.ToString()));
        }

        /// <summary>
        /// Gets the zero based index of the last occurrence of one string in another
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other">The string to search</param>
        /// <returns>If found the zero based index of the occurrence. If not found returns -1</returns>
        public static Number LastIndexOf(String self, Any other)
        {
            return new Number((self.value.ToString()).LastIndexOf(other.ToString()));
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
            return new String((self.value.ToString()).Replace(oldValue.ToString(), newValue.ToString()));
        }

        /// <summary>
        /// Determines whether one string starts with another
        /// </summary>
        /// <param name="self">The string on which to search</param>
        /// <param name="other">The string to search</param>
        /// <returns>true or false</returns>
        public static Boolean StartsWith(String self, Any other)
        {
            return new Boolean((self.value.ToString()).StartsWith(other.ToString()));
        }

        /// <summary>
        /// Returns the string converted to lowercase
        /// </summary>
        /// <param name="self">The string to be converted</param>
        /// <returns>new lowercased string</returns>
        public static String ToLower(String self)
        {
            return new String((self.value.ToString()).ToLower());
        }

        /// <summary>
        /// Returns the string converted to uppercase
        /// </summary>
        /// <param name="self">The string to be converted</param>
        /// <returns>new upercased string</returns>
        public static String ToUpper(String self)
        {
            return new String((self.value.ToString()).ToUpper());
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
                return new String((self.value.ToString()).Substring(start.ToInteger()));
            }
            else
            {
                return new String((self.value.ToString()).Substring(start.ToInteger(), length.ToInteger()));
            }
        }

        /// <summary>
        /// Reverses the string and returns it
        /// </summary>
        /// <param name="self">The string to be reversed</param>
        /// <returns>The reversed string</returns>
        public static String Reverse(String self)
        {
            char[] str = (self.value.ToString()).ToCharArray();
            System.Array.Reverse(str);
            return new String(new string(str));
        }

        /// <summary>
        /// Executes a callback on each character of the string
        /// </summary>
        /// <param name="self">the self string to be iterated</param>
        /// <param name="callback">the callback function to be executed</param>
        /// <returns>Returns self string</returns>
        public static Any Each(String self, Callable callback, Interpreter inter)
        {
            int index = 0;
            foreach(var character in (self.value.ToString()))
            {
                callback.Call(inter, self, new List<Any>()
                {
                    new String(character.ToString()), new Number(index), self
                });
                index += 1;
            }
            return self;
        }
        /// <summary>
        /// Rempas each character of the string by using values returned from callback function
        /// </summary>
        /// <param name="self">the self string to be remapped</param>
        /// <param name="callback">the callback function</param>
        /// <param name="inter"></param>
        /// <returns>The new remapped string</returns>
        public static String Map(String self, Callable callback, Interpreter inter)
        {
            int index = 0;
            var result = new List<Any>();
            foreach (var character in (self.value.ToString()))
            {
                result.Add(
                    callback.Call(inter, self, new List<Any>()
                    {
                        new String(character.ToString()), new Number(index), self
                    })
                );
                index += 1;
            }
            return new String(string.Join("", result.Select(r => r.ToString())));
        }

    }

}
