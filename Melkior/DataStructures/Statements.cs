using System.Collections.Generic;

namespace Melkior
{
    public abstract class Stmt
    {
        public Any result;
        public int line;
public Stmt Line(Token token)
{
    line = token.line;
    return this;
}

        public interface IVisitor<T>
        {
            T VisitBlockStmt(Block stmt);
            T VisitClassStmt(Class stmt);
            T VisitDoWhileStmt(DoWhile stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitFunctionStmt(Function stmt);
            T VisitIfStmt(If stmt);
            T VisitPrintStmt(Print stmt);
            T VisitPauseStmt(Pause stmt);
            T VisitReturnStmt(Return stmt);
            T VisitVarStmt(Var stmt);
            T VisitWhileStmt(While stmt);
            T VisitForeachStmt(Foreach stmt);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Block: Stmt
        {
            public List<Stmt> statements;

            public Block(List<Stmt> statements)
            {
                this.statements = statements;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitBlockStmt(this);
            }
        }

        public class Class: Stmt
        {
            public Token name;
            public Token parent;
            public List<Stmt> methods;

            public Class(Token name, Token parent, List<Stmt> methods)
            {
                this.name = name;
                this.parent = parent;
                this.methods = methods;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitClassStmt(this);
            }
        }

        public class DoWhile: Stmt
        {
            public Stmt loop;
            public Expr condition;

            public DoWhile(Stmt loop, Expr condition)
            {
                this.loop = loop;
                this.condition = condition;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitDoWhileStmt(this);
            }
        }

        public class Expression: Stmt
        {
            public Expr expression;

            public Expression(Expr expression)
            {
                this.expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitExpressionStmt(this);
            }
        }

        public class Function: Stmt
        {
            public Token name;
            public List<Token> prms;
            public List<Stmt> body;

            public Function(Token name, List<Token> prms, List<Stmt> body)
            {
                this.name = name;
                this.prms = prms;
                this.body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitFunctionStmt(this);
            }
        }

        public class If: Stmt
        {
            public Expr condition;
            public Stmt thenStmt;
            public Stmt elseStmt;

            public If(Expr condition, Stmt thenStmt, Stmt elseStmt)
            {
                this.condition = condition;
                this.thenStmt = thenStmt;
                this.elseStmt = elseStmt;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitIfStmt(this);
            }
        }

        public class Print: Stmt
        {
            public Expr expression;

            public Print(Expr expression)
            {
                this.expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitPrintStmt(this);
            }
        }

        public class Pause: Stmt
        {

            public Pause()
            {
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitPauseStmt(this);
            }
        }

        public class Return: Stmt
        {
            public Expr value;

            public Return(Expr value)
            {
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitReturnStmt(this);
            }
        }

        public class Var: Stmt
        {
            public Token name;
            public Token type;
            public Expr initializer;
            public bool writable;

            public Var(Token name, Token type, Expr initializer, bool writable)
            {
                this.name = name;
                this.type = type;
                this.initializer = initializer;
                this.writable = writable;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitVarStmt(this);
            }
        }

        public class While: Stmt
        {
            public Expr condition;
            public Stmt loop;

            public While(Expr condition, Stmt loop)
            {
                this.condition = condition;
                this.loop = loop;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitWhileStmt(this);
            }
        }

        public class Foreach: Stmt
        {
            public Token item;
            public Token key;
            public Expr iterable;
            public Stmt loop;

            public Foreach(Token item, Token key, Expr iterable, Stmt loop)
            {
                this.item = item;
                this.key = key;
                this.iterable = iterable;
                this.loop = loop;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitForeachStmt(this);
            }
        }

    }
}
