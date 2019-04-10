using System;
using System.Collections.Generic;

namespace Melkior
{
    class Interpreter : Expr.IVisitor<Any>, Stmt.IVisitor<Any>
    {
        public Scope global;
        public Scope scope;
        private List<Stmt> statements;
        public Interpreter() {
            global = new Scope();
            scope = global;
        }

        public List<Stmt> Interpret(List<Stmt> statements)
        {
            this.statements = statements;
            var current = statements[0];
            try
            {
                foreach (var statement in statements)
                {
                    current = statement;
                    Execute(statement);
                }

                Any main = global.Get("main");

                if (!main.IsNull())
                {
                    ((Function)main).Call(this, new Null(), new List<Any>() { new String("argument") });
                }
            }
            catch (MelkiorException error)
            {
                Console.WriteLine("[Melkior Runtime Error] (line: " + current.line + ") " +
                    "near " + current.ToString());
                Console.WriteLine(error.message);
                return null;

            }
            catch(Exception e)
            {
                Console.WriteLine("[Melkior Unhandeled Runtime Error] (line: " + current.line +
                    ") near " + current.ToString());
                Console.WriteLine(e.Message);

                return null;
            }
            return statements;
        }

        private Any Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private Any Execute(Stmt stmt)
        {
            var result = stmt.Accept(this);
            stmt.result = result;
            return result;
        }

        private void Error(string message)
        {
            throw new MelkiorException(message);
        }

        public Any VisitAssignExpr(Expr.Assign expr)
        {
            Any value = Evaluate(expr.value);
            scope.Assign(expr.name.lexeme, value);
            return value;
        }

        public Any VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.left);
            var right = Evaluate(expr.right);

            switch (expr.oprtr.type)
            {
                case TokenType.Plus:
                case TokenType.PlusEqual:
                    return left + right;
                case TokenType.Minus:
                case TokenType.MinusEqual:
                    return (left as Number) - (right as Number);
                case TokenType.Slash:
                case TokenType.SlashEqual:
                    return (left as Number) / (right as Number);
                case TokenType.Star:
                case TokenType.StarEqual:
                    return (left as Number) * (right as Number);
                case TokenType.Percent:
                case TokenType.PercentEqual:
                    return (left as Number) % (right as Number);
                case TokenType.Greater:
                    return (left as Number) > (right as Number);
                case TokenType.GreaterEqual:
                    return (left as Number) >= (right as Number);
                case TokenType.Less:
                    return (left as Number) < (right as Number);
                case TokenType.LessEqual:
                    return (left as Number) <= (right as Number);
                case TokenType.EqualEqual:
                    return left == right;
                case TokenType.BangEqual:
                    return left != right;
                default:
                    throw new MelkiorException("Unexpected binary operator: " + expr.oprtr);
            }
        }

        public Any VisitCallExpr(Expr.Call expr)
        {
            Any callee = Evaluate(expr.callee);
            var args = new List<Any>();

            foreach(var argument in expr.args)
            {
                args.Add(Evaluate(argument));
            }

            if (callee.type != DataType.Function || !(callee is Callable))
            {
                Error("`" + callee + "` is not a function");
            }

            Any self = null;
            if (expr.callee is Expr.Get)
            {
                if ((expr.callee as Expr.Get).entity is Expr.Base)
                {
                    self = scope.Get("this");
                }
                else
                {
                    self = Evaluate((expr.callee as Expr.Get).entity);
                }
            }

            // todo add aritiy check
            return (callee as Callable).Call(this, self, args);
        }

        public Any VisitGroupingExpr(Expr.Group expr)
        {
            return Evaluate(expr.expression);
        }

        public Any VisitLogicalExpr(Expr.Logical expr)
        {
            Any left = Evaluate(expr.left);

            if (expr.oprtr.type == TokenType.Or)
            {
                if (left.IsTruthy())
                {
                    return left;
                }
            }
            if (expr.oprtr.type == TokenType.And)
            {
                if (!left.IsTruthy())
                {
                    return left;
                }
            }
            return Evaluate(expr.right);

        }

        public Any VisitLiteralExpr(Expr.Literal expr)
        {

            if (expr.type == DataType.Number)
            {
                return new Number((double)expr.value);
            }
            if (expr.type == DataType.String)
            {
                return new String((string)expr.value);
            }
            if (expr.type == DataType.Boolean)
            {
                return new Boolean((bool)expr.value);
            }
            if (expr.type == DataType.Null)
            {
                return new Any(null, DataType.Null);
            }

            return new Any(expr.value, expr.type);
        }

        public Any VisitTernaryExpr(Expr.Ternary expr)
        {
            return Evaluate(expr.condition).IsTruthy() ? Evaluate(expr.thenExpr) : Evaluate(expr.elseExpr);
        }

        public Any VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.right) as Any;

            switch (expr.oprtr.type)
            {
                case TokenType.Minus:
                    return new Number(-(double)right.value);
                case TokenType.Typeof:
                    return right.TypeOf();
                case TokenType.Not:
                    return new Boolean(!right.IsTruthy());
                default:
                    throw new MelkiorException("Unexpected unary operator " + expr.oprtr);
            }
        }

        public Any VisitVariableExpr(Expr.Variable expr)
        {
            var data = scope.Get(expr.name.lexeme);
            return data;
        }

        public Any VisitBlockStmt(Stmt.Block stmt)
        {
            return ExecuteBlock(stmt.statements, new Scope(scope));
        }

        private Any ExecuteBlock(List<Stmt> statements, Scope nextScope)
        {
            var restoreScope = scope;
            scope = nextScope;
            foreach (var statement in statements)
            {
                Execute(statement);
            }
            scope = restoreScope;
            return new Any("block", DataType.Debug);
        }

        private Any ExecuteStatement(Stmt statement, Scope nextScope)
        {
            var restoreScope = scope;
            scope = nextScope;
            var executed = Execute(statement);
            scope = restoreScope;
            return executed;
        }

        public Any ExecuteFunction(List<Stmt> block, Scope nextScope)
        {
            Scope restoreScope = scope;
            Any result = new Any(null, DataType.Null);
            try
            {
                scope = nextScope;
                foreach (var statement in block)
                {
                    Execute(statement);
                }
            }
            catch (MelkiorReturn e)
            {
                result = e.value;
            }
            finally
            {
                scope = restoreScope;
            }
            return result;
        }



        public Any VisitExpressionStmt(Stmt.Expression stmt)
        {
            return Evaluate(stmt.expression);
        }

        public Any VisitFunctionStmt(Stmt.Function stmt)
        {
            var func = new Function(stmt, scope);
            scope.Define(stmt.name.lexeme, func);
            return func;
        }

        public Any VisitIfStmt(Stmt.If stmt)
        {
            if (Evaluate(stmt.condition).IsTruthy())
            {
                return Execute(stmt.thenStmt);
            }
            else if (stmt.elseStmt != null)
            {
                return Execute(stmt.elseStmt);
            }
            return new Any("if nop", DataType.Debug);
        }

        public Any VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.expression);
            var str = new String("method");
            Console.WriteLine(value);
            return value;
        }

        public Any VisitVarStmt(Stmt.Var stmt)
        {
            Any value = new Any(null, DataType.Null);

            if (stmt.initializer != null)
            {
                value = Evaluate(stmt.initializer) as Any;
            }

            scope.Define(stmt.name.lexeme, value);
            return value;
        }

        public Any VisitWhileStmt(Stmt.While stmt)
        {
            while (Evaluate(stmt.condition).IsTruthy())
            {
                Execute(stmt.loop);
            }
            return new Any("while", DataType.Debug);
        }

        public Any VisitDoWhileStmt(Stmt.DoWhile stmt)
        {
            do
            {
                Execute(stmt.loop);
            } while (Evaluate(stmt.condition).IsTruthy());

            return new Any("do while", DataType.Debug);
        }

        public Any VisitReturnStmt(Stmt.Return stmt)
        {
            Any value = null;
            if (stmt.value != null)
            {
                value = Evaluate(stmt.value);
            }
            throw new MelkiorReturn(value);
        }

        public Any VisitGetExpr(Expr.Get expr)
        {
            var entity = Evaluate(expr.entity);
            var key = Evaluate(expr.key);
            return entity.Get(key);
        }

        public Any VisitSetExpr(Expr.Set expr)
        {
            var entity = Evaluate(expr.entity);
            var value = Evaluate(expr.value);
            var key = Evaluate(expr.key);
            entity.Set(key, value);
            return value;
        }

        public Any VisitKeyExpr(Expr.Key expr)
        {
            return new String(expr.name.lexeme);
        }

        public Any VisitArrayExpr(Expr.Array expr)
        {
            var values = new List<Any>();

            foreach (var expression in expr.values)
            {
                values.Add(Evaluate(expression));
            }

            return new Array(values);
        }

        public Any VisitGroupExpr(Expr.Group expr)
        {
            return Evaluate(expr.expression);
        }

        public Any VisitDictExpr(Expr.Dict expr)
        {
            var dictionary = new Dict(new Dictionary<Any, Any>());
            foreach (var entry in expr.entries) {
                var key  = Evaluate((entry as Expr.Set).key);
                var value = Evaluate((entry as Expr.Set).value);
                dictionary.Set(key, value);
            }
            return dictionary;
        }

        public Any VisitForeachStmt(Stmt.Foreach stmt)
        {
            string name = stmt.item.lexeme;
            string key = stmt.key == null ? null : stmt.key.lexeme;
            var iterable = Evaluate(stmt.iterable);

            if (iterable.IsArray())
            {

                var index = new Number(0);
                foreach (var item in iterable.value as List<Any>)
                {
                    Scope loopScope = new Scope(scope);
                    loopScope.Define(name, item);
                    if (key != null)
                    {
                        loopScope.Define(key, index);
                    }
                    if (stmt.loop is Stmt.Block)
                    {
                        ExecuteBlock((stmt.loop as Stmt.Block).statements, loopScope);
                    }
                    else
                    {
                        ExecuteStatement(stmt.loop, loopScope);
                    }
                    
                    index.value = (double) index.value + 1;
                }
                return new Any(null, DataType.Null);
            }

            if (iterable.IsString())
            {

                var index = new Number(0);
                foreach (var item in iterable.value as string)
                {
                    Scope loopScope = new Scope(scope);
                    loopScope.Define(name, new String(item.ToString()));
                    if (key != null)
                    {
                        loopScope.Define(key, index);
                    }
                    if (stmt.loop is Stmt.Block)
                    {
                        ExecuteBlock((stmt.loop as Stmt.Block).statements, loopScope);
                    }
                    else
                    {
                        ExecuteStatement(stmt.loop, loopScope);
                    }

                    index.value = (double)index.value + 1;
                }
                return new Any(null, DataType.Null);
            }

            if (iterable.IsDict())
            {
                foreach (var item in iterable.value as Dictionary<Any, Any>)
                {
                    Scope loopScope = new Scope(scope);
                    loopScope.Define(name, item.Value);
                    if (key != null)
                    {
                        loopScope.Define(key, item.Key);
                    }
                    if (stmt.loop is Stmt.Block)
                    {
                        ExecuteBlock((stmt.loop as Stmt.Block).statements, loopScope);
                    }
                    else
                    {
                        ExecuteStatement(stmt.loop, loopScope);
                    }
                }
                return new Any(null, DataType.Null);
            }

            if (iterable.IsRange())
            {
                var start = ((RangeValue)iterable.value).start.ToInteger();
                var end = ((RangeValue)iterable.value).end.ToInteger();
                var step = ((RangeValue)iterable.value).step.ToInteger();
                var index = new Number(0);
                if (step > 0)
                {
                    for (var it = start; it <= end; it += step)
                    {
                        Scope loopScope = new Scope(scope);
                        loopScope.Define(name, new Number(it));
                        if (key != null)
                        {
                            loopScope.Define(key, index);
                        }
                        if (stmt.loop is Stmt.Block)
                        {
                            ExecuteBlock((stmt.loop as Stmt.Block).statements, loopScope);
                        }
                        else
                        {
                            ExecuteStatement(stmt.loop, loopScope);
                        }
                        index.value = (double)index.value + 1;
                    }
                }
                else if (step < 0)
                {
                    for (var it = start; it >= end; it += step)
                    {
                        Scope loopScope = new Scope(scope);
                        loopScope.Define(name, new Number(it));
                        if (key != null)
                        {
                            loopScope.Define(key, index);
                        }
                        if (stmt.loop is Stmt.Block)
                        {
                            ExecuteBlock((stmt.loop as Stmt.Block).statements, loopScope);
                        }
                        else
                        {
                            ExecuteStatement(stmt.loop, loopScope);
                        }
                        index.value = (double)index.value + 1;
                    }
                }
                else
                {
                    throw new MelkiorException("Step value in range can't be 0");
                }
                return new Any(null, DataType.Null);
            }

            Error(stmt.iterable + " is not an iterable collection");
            return null;

        }

        public Any VisitLambdaExpr(Expr.Lambda expr)
        {
            return new Function(expr.lambda as Stmt.Function, scope);
        }

        public Any VisitPauseStmt(Stmt.Pause stmt)
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        public Any VisitRangeExpr(Expr.Range expr)
        {
            return new Range(new RangeValue(
                expr.start != null ? Evaluate(expr.start) : new Null(),
                expr.end != null ? Evaluate(expr.end) : new Null(),
                expr.step != null ? Evaluate(expr.step) : new Null()
            ));
        }

        public Any VisitClassStmt(Stmt.Class stmt)
        {
            var name = new String(stmt.name.lexeme);
            Any parent;

            if (stmt.parent == null)
            {
                parent = new Null();
            }
            else
            {
                parent = scope.Get(stmt.parent.lexeme);
            }
      
            var methods = new Dictionary<Any, Any>();

            foreach (var method in stmt.methods)
            {
                var func = (Stmt.Function)method;
                methods.Add(new String(func.name.lexeme), new Function(func, scope));
            }
            var clazz = new Class(name, methods, parent);
            scope.Define(stmt.name.lexeme, clazz);
            return clazz;
        }

        public Any VisitNewExpr(Expr.New expr)
        {
            Class clazz =(Class)Evaluate(((Expr.Call)expr.constructor).callee);
            if (!clazz.IsClass())
            {
                throw new MelkiorException("new statement must be used with classes. '" + clazz + "' is not a class");
            }
            var entity = new Entity(new Dictionary<Any, Any>(), clazz);
            var constructor = clazz.Get(new String("constructor"));
            if (constructor.IsFunction())
            {
                var args = new List<Any>();
                foreach(var arg in ((Expr.Call)expr.constructor).args)
                {
                    args.Add(Evaluate(arg));
                }
                ((Function)constructor).Call(this, entity, args);
            }

            return entity;
        }

        public Any VisitBaseExpr(Expr.Base expr)
        {
            var self = scope.Get("this");

            if (!self.IsEntity())
            {
                throw new MelkiorException("base expression can be used only inside methods");
            }

            var clazz = ((Entity)self).constructor;
            var parent = ((Class)clazz).parent;
            if (parent.IsNull())
            {
                throw new MelkiorException("Class " + clazz + "is not inherited from parent");
            }

            return parent;
        }
    }
}
