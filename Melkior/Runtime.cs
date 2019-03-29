using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melkior
{
    class Runtime
    {
        public static Callable ArrayLength(List<Any> values)
        {
            return new Callable(
                (Interpreter inter, Any thiz, List<Any> args) => {
                    return new Number(values.Count);
                }
            );
        }
        public static Callable ArrayEach(List<Any> values)
        {
            return new Callable(
                (Interpreter inter, Any thiz, List<Any> args) => {
                    for (int i = 0; i < values.Count; ++i)
                    {
                        (args[0] as Callable).Call(inter, thiz, new List<Any>() { values[i], new Number(i), new Any(values, DataType.Array) });
                    }
                    return null;
                }
            );
        }

        
    }
}
