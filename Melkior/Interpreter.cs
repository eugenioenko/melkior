using System;
using System.Collections.Generic;

namespace Melkior
{
    class Interpreter : Expr.IVisitor<Any>, Stmt.IVisitor<Any>
    {
        public Scope global;
        public Scope scope;
        private List<Stmt> statements;

        public Interpreter() { }

        public Any Interpret(List<Stmt> statements)
        {

            this.statements = statements;
            global = new Scope();
            scope = global;
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }

                return null;
            } 
            catch
            {
                Error("Unhandled Error");
            }

            return null;
        }

        private Any Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private Any Execute(Stmt stmt)
        {
            return stmt.Accept(this);
        }

        private void Error(string message)
        {
            Console.WriteLine("[Runtime Error] => " + message);
            Environment.Exit(-1);
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
                    return null;

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
                Error("Runtime Error" + callee + " is not a function");
            }

            // todo add aritiy check
            return (callee as Callable).Call(this, callee, args);
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
                if (left.ToBoolean())
                {
                    return left;
                }
            }
            if (expr.oprtr.type == TokenType.And)
            {
                if (!left.ToBoolean())
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
            return Evaluate(expr.condition).ToBoolean() ? Evaluate(expr.thenExpr) : Evaluate(expr.elseExpr);
        }

        public Any VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.right) as Any;

            switch (expr.oprtr.type)
            {
                case TokenType.Minus:
                    return new Number(-(double)right.value);
                case TokenType.Typeof:
                    return new String(right.type.ToString());
                case TokenType.Not:
                    return new Boolean(!right.ToBoolean());
                default:
                    throw new NotImplementedException();
            }
        }

        public Any VisitVariableExpr(Expr.Variable expr)
        {
            var data = scope.Get(expr.name.lexeme);
            return data;
        }

        public Any VisitStringExpr(Expr.String expr)
        {
            return new String(expr.value);
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
            return null;
        }

        public Any ExecuteFuncClosure(Stmt statement, Scope nextScope)
        {
            Scope restoreScope = scope;
            Any result = null;
            try
            {
                scope = nextScope;
                result = Execute(statement);
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
            if (Evaluate(stmt.condition).ToBoolean())
            {
                return Execute(stmt.thenStmt);
            }
            else if (stmt.elseStmt != null)
            {
                return Execute(stmt.elseStmt);
            }
            return null;
        }

        public Any VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.expression);
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
            while (Evaluate(stmt.condition).ToBoolean())
            {
                Execute(stmt.loop);
            }
            return null;
        }

        public Any VisitDoWhileStmt(Stmt.DoWhile stmt)
        {
            do
            {
                Execute(stmt.loop);
            } while (Evaluate(stmt.condition).ToBoolean());
            return null;
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
            var iterable = scope.Get(stmt.iterable.lexeme);

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
                    ExecuteFuncClosure(stmt.loop, loopScope);
                    index.value = (double) index.value + 1;
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
                    ExecuteFuncClosure(stmt.loop, loopScope);
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
    }
}
