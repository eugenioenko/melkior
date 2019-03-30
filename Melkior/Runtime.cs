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
                    var index =  new Number(0);
                    foreach (var item in values)
                    {
                        (args[0] as Callable).Call(
                            inter, thiz, new List<Any>() {
                                item, index, new Any(values, DataType.Array)
                            }
                        );
                        index.value = (double) index.value + 1;
                    }
                    return null;
                }
            );
        }

        public static Callable ArrayMap(List<Any> values)
        {
            return new Callable(
                (Interpreter inter, Any thiz, List<Any> args) => {
                    var index = new Number(0);
                    var mapped = new List<Any>();

                    foreach (var item in values)
                    {
                        mapped.Add(
                            (args[0] as Callable).Call(
                                inter, thiz, new List<Any>() {
                                    item, index, new Any(values, DataType.Array)
                                }
                            )
                        );
                        index.value = (double)index.value + 1;
                    }
                    return new Array(mapped);
                }
            );
        }
    }
}
