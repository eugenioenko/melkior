using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melkior
{
    class Transpiler : Expr.IVisitor<string>, Stmt.IVisitor<string>
    {
        public List<string> transpiled;

        public Transpiler()
        {
            transpiled = new List<string>();
        }

        public string Transpile(List<Stmt> statements)
        {
            var current = statements[0];
            try
            {
                foreach (var statement in statements)
                {
                    current = statement;
                    transpiled.Add(Execute(statement));
                }
                
            }
            catch (MelkiorException error)
            {
                Console.WriteLine("[Melkior Runtime Error] (line: " + current.line + ") " +
                    "near " + current.ToString());
                Console.WriteLine(error.message);
                return null;

            }
            catch (Exception e)
            {
                Console.WriteLine("[Melkior Unhandeled Runtime Error] (line: " + current.line +
                    ") near " + current.ToString());
                Console.WriteLine(e.Message);

                return null;
            }
            return string.Join("\n", transpiled);
        }

        private string Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private string Execute(Stmt stmt)
        {
            return stmt.Accept(this);
         
        }

        private void Error(string message)
        {
            throw new MelkiorException(message);
        }

        public string VisitArrayExpr(Expr.Array expr)
        {
            var values = new List<string>();

            foreach (var expression in expr.values)
            {
                values.Add(Evaluate(expression));
            }
            return '[' + string.Join(", ", values) + ']';
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            if (expr.oprtr.type == TokenType.Equal)
            {
                return expr.name.lexeme + " = " + Evaluate(expr.value) + ";";
            }
            else
            {
                var right = Evaluate((expr.value as Expr.Binary).right);
                return expr.name.lexeme + " " + expr.oprtr.lexeme + " " + right; 
            }
        }

        public string VisitBaseExpr(Expr.Base expr)
        {
            return "super";
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Evaluate(expr.left) + " " + expr.oprtr.lexeme + " " + Evaluate(expr.right);
        }

        public string VisitBlockStmt(Stmt.Block stmt)
        {
            var statements = new List<string>();

            foreach(var statement in stmt.statements)
            {
                statements.Add(Execute(statement));
            }

            return '{' + string.Join("\n", statements) + '}';
        }

        public string VisitCallExpr(Expr.Call expr)
        {
            var args = new List<string>();

            foreach (var argument in expr.args)
            {
                args.Add(Evaluate(argument));
            }

            var callee = Evaluate(expr.callee);
            return callee + '(' + string.Join(", ", args) + ")";
        }

        public string VisitClassStmt(Stmt.Class stmt)
        {
            var result = "class " + stmt.name.lexeme;
            if (stmt.parent != null)
            {
                result += " extends " + stmt.parent.lexeme;
            }
            result += '{';
            foreach(var method in stmt.methods)
            {
                result += Execute(method);
            }
            result += '}';
            return result;
        }

        public string VisitDictExpr(Expr.Dict expr)
        {
            var properties = new List<string>();
            foreach (var entry in expr.entries)
            {
                var key = Evaluate((entry as Expr.Set).key);
                var value = Evaluate((entry as Expr.Set).value);
                properties.Add(key + ": " + value);
            }
            return '{' + string.Join(", ", properties) + "}";
        }

        public string VisitDoWhileStmt(Stmt.DoWhile stmt)
        {
            return "do {" + Execute(stmt.loop) + "} while (" + Evaluate(stmt.condition) + ");"; 
              
        }

        public string VisitExpressionStmt(Stmt.Expression stmt)
        {
            return Evaluate(stmt.expression) + ";";
        }

        public string VisitForeachStmt(Stmt.Foreach stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitFunctionStmt(Stmt.Function stmt)
        {
            var result = "";
            if (stmt.type == FunctionType.Function)
            {
                result += "function ";
            }
            result += stmt.name.lexeme;
            result += "(" + string.Join(", ", stmt.prms.Select(prm => prm.lexeme));
            result += ") {";
            foreach (var statement in stmt.body)
            {
                result += Execute(statement);
            }
            result += '}';
            return result;
        }

        public string VisitGetExpr(Expr.Get expr)
        {
            var entity = Evaluate(expr.entity);
            var key = Evaluate(expr.key);
            return entity + '.' + key;
        }

        public string VisitGroupExpr(Expr.Group expr)
        {
            return '(' + Evaluate(expr.expression) + ')';
        }

        public string VisitIfStmt(Stmt.If stmt)
        {
            var result = "if (" + Evaluate(stmt.condition) + ") " + Execute(stmt.thenStmt);
           
            if (stmt.elseStmt != null)
            {
                result += " else " +  Execute(stmt.elseStmt);
            }
    
            return result;
        }

        public string VisitKeyExpr(Expr.Key expr)
        {
            return expr.name.lexeme;
        }

        public string VisitLambdaExpr(Expr.Lambda expr)
        {
            throw new NotImplementedException();
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.type == DataType.String)
            {
                return '"' + expr.value.ToString() + '"';
            }
            if (expr.type == DataType.Number)
            {
                if ((double)expr.value % 1 == 0)
                {
                    return Convert.ToInt32(expr.value).ToString();
                }
            }
            if (expr.type == DataType.Boolean)
            {
                return (bool)expr.value ? "true" : "false";
            }
            if (expr.type == DataType.Null)
            {
                return "null";
            }

            return expr.value.ToString();
        }

        public string VisitLogicalExpr(Expr.Logical expr)
        {
            var oprtr = "";
            switch (expr.oprtr.type)
            {
                case TokenType.And: oprtr = "&&"; break;
                case TokenType.Or: oprtr = "||"; break;
            }
            return Evaluate(expr.left) + " " + oprtr + " " + Evaluate(expr.right);
        }

        public string VisitNewExpr(Expr.New expr)
        {
            var result = "new ";
            var constructor = expr.constructor as Expr.Call;
            result += Evaluate(constructor.callee) + '(';
            var args = new List<string>();
            foreach (var arg in ((Expr.Call)expr.constructor).args)
            {
                args.Add(Evaluate(arg));
            }
            result += string.Join(", ", args) + ")";
            return result;
        }

        public string VisitPauseStmt(Stmt.Pause stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitPrintStmt(Stmt.Print stmt)
        {
            return "console.log(" + Evaluate(stmt.expression) + ");";
        }

        public string VisitRangeExpr(Expr.Range expr)
        {
            throw new NotImplementedException();
        }

        public string VisitReturnStmt(Stmt.Return stmt)
        {
            return "return " + Evaluate(stmt.value);
        }

        public string VisitSetExpr(Expr.Set expr)
        {
            var entity = Evaluate(expr.entity);
            var value = Evaluate(expr.value);
            var key = Evaluate(expr.key);
            return entity + "." + key + " = " + value;
        }

        public string VisitTernaryExpr(Expr.Ternary expr)
        {
            return Evaluate(expr.condition) + " ? " + Evaluate(expr.thenExpr) + " : " + Evaluate(expr.elseExpr); 
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            var oprtr = expr.oprtr.lexeme;
            if (expr.oprtr.type == TokenType.Not)
            {
                oprtr = "!";
            }
            return oprtr + Evaluate(expr.right);
        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return expr.name.lexeme;
        }

        public string VisitVarStmt(Stmt.Var stmt)
        {
            var result = "var " + stmt.name.lexeme;

            if (stmt.initializer != null)
            {
                result += " = " + Evaluate(stmt.initializer);
            }
            result += ";";

            return result;
        }

        public string VisitWhileStmt(Stmt.While stmt)
        {
            return "while (" + Evaluate(stmt.condition) + ")" + Execute(stmt.loop);
        }
    }
}
