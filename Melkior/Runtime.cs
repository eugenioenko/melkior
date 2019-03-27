using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melkior
{
    class Runtime
    {
        public static Callable ArrayLength =  new Callable(
            (Interpreter inter, Any thiz, List<Any> args) => {
                return new Number((thiz.value as List<Any>).Count);
            }
        );
    }
}
