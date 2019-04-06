using System;
using System.Collections.Generic;
using System.Text;

namespace Melkior
{

    delegate Any FunctionCall(Interpreter inter, Any self, List<Any> args);

    class Callable : Any
    {
        public FunctionCall Call;

        public Callable() : base(null, DataType.Function)
        {

        }

        public Callable(FunctionCall call): this ()
        {
            Call = call;
            value = call;
        }

        public override string ToString() {
            return "<internal function>";
        }
    }

    class Function : Callable
    {
        private Stmt.Function declaration;
        private Scope closure;

        public Function(Stmt.Function declaration, Scope closure): base()
        {
            this.declaration = declaration;
            this.closure = closure;

            Call = (Interpreter inter, Any self, List<Any> args) =>
            {
                Scope funcScope = new Scope(closure);
                for (var i = 0; i < declaration.prms.Count; ++i)
                {
                    funcScope.Define(declaration.prms[i].lexeme, args[i]);
                }

                if (self.IsEntity())
                {
                    funcScope.Define("this", self);
                }

                return inter.ExecuteFunction(declaration.body, funcScope);
            };
        }

        public override string ToString()
        {
            return declaration.name.lexeme;
        }
    }
}
