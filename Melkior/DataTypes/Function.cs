using System;
using System.Collections.Generic;
using System.Text;

namespace Melkior
{

    interface ICallable
    {
        int Arity();
        Any Call(Interpreter inter, Any thiz, List<Any> args);
    }

    class Function : ICallable
    {
        private Stmt.Function declaration;
        private Scope closure;
        public Prototype prototype;

        public Function(Stmt.Function declaration, Scope closure)
        {
            this.declaration = declaration;
            this.closure = closure;
            this.prototype = new Prototype();
        }

        public int Arity()
        {
            return declaration.prms.Count;
        }

        public Any Call(Interpreter inter, Any thiz, List<Any> args)
        {
            Scope funcScope = new Scope(closure);
            for (var i = 0; i < declaration.prms.Count; ++i)
            {
                funcScope.Define(declaration.prms[i].lexeme, args[i]);
            }

            return inter.ExecuteFuncClosure(declaration.body, funcScope);
        }

        public override string ToString()
        {
            return declaration.name.lexeme;
        }
    }
}
