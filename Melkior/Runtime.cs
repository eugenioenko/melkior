using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melkior
{
    
    sealed class Runtime
    {
   
        public static Dictionary<string, Callable> StringMethods = new Dictionary<string, Callable>()
        {
            { "split", Strings.Split }
        };

        public static Dictionary<string, Callable> ArrayMethods = new Dictionary<string, Callable>()
        {
            { "each", Arrays.Each },
            { "map", Arrays.Map }
        };

        public static class Strings
        {
            public static Callable Split = new Callable(
                (Interpreter inter, Any self, List<Any> args) =>
                {
                    return String.Split(self as String, args[0] as Any);
                }
            );
        }

        public static class Arrays
        {
            public static Callable Each =  new Callable(
                (Interpreter inter, Any self, List<Any> args) =>
                {
                    int index = 0;
                    foreach (var item in self.value as List<Any>)
                    {
                        (args[0] as Callable).Call(
                            inter, self, new List<Any>()
                            {
                                item, new Number(index), self
                            }
                        );
                        index += 1;
                    }
                    return null;
                }
            );

            public static Callable Map =  new Callable(
                (Interpreter inter, Any self, List<Any> args) =>
                {
                    int index = 0;
                    var mapped = new List<Any>();

                    foreach (var item in self.value as List<Any>)
                    {
                        mapped.Add(
                            (args[0] as Callable).Call(
                                inter, self, new List<Any>()
                                {
                                    item, new Number(index), self
                                }
                            )
                        );
                        index += 1;
                    }
                    return new Array(mapped);
                }
            );
        }
    }
}
