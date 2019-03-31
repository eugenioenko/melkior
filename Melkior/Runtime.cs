using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melkior
{
    
    sealed class Runtime
    {
   
        public static Dictionary<Any, Any> StringMethods = new Dictionary<Any, Any>()
        {
            { new String("concat"), Strings.Concat },
            { new String("contains"), Strings.Contains },
            { new String("includes"), Strings.Contains },
            { new String("split"), Strings.Split }
            
        };

        public static Dictionary<Any, Callable> ArrayMethods = new Dictionary<Any, Callable>()
        {
            { new String("each"), Arrays.Each },
            { new String("map"), Arrays.Map }
        };

        public static class Strings
        {
            public static Callable Concat = new Callable(
               (Interpreter inter, Any self, List<Any> args) =>
               {
                   return String.Concat(self as String, args[0]);
               }
           );

            public static Callable Contains = new Callable(
               (Interpreter inter, Any self, List<Any> args) =>
               {
                   return String.Contains(self as String, args[0]);
               }
           );

            public static Callable Split = new Callable(
               (Interpreter inter, Any self, List<Any> args) =>
               {
                   return String.Split(self as String, args[0]);
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
