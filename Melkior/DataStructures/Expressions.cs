using System.Collections.Generic;

namespace Melkior
{
    abstract class Expr
    {
        public interface IVisitor<T>
        {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitCallExpr(Call expr);
            T VisitDictExpr(Dict expr);
            T VisitGetExpr(Get expr);
            T VisitGroupExpr(Group expr);
            T VisitKeyExpr(Key expr);
            T VisitLambdaExpr(Lambda expr);
            T VisitLogicalExpr(Logical expr);
            T VisitArrayExpr(Array expr);
            T VisitLiteralExpr(Literal expr);
            T VisitRangeExpr(Range expr);
            T VisitSetExpr(Set expr);
            T VisitTernaryExpr(Ternary expr);
            T VisitUnaryExpr(Unary expr);
            T VisitVariableExpr(Variable expr);
            T VisitStringExpr(String expr);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Assign: Expr
        {
            public Token name;
            public Expr value;

            public Assign(Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitAssignExpr(this);
            }
        }

        public class Binary: Expr
        {
            public Expr left;
            public Token oprtr;
            public Expr right;

            public Binary(Expr left, Token oprtr, Expr right)
            {
                this.left = left;
                this.oprtr = oprtr;
                this.right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitBinaryExpr(this);
            }
        }

        public class Call: Expr
        {
            public Expr callee;
            public Token paren;
            public List<Expr> args;
            public object thiz;

            public Call(Expr callee, Token paren, List<Expr> args, object thiz)
            {
                this.callee = callee;
                this.paren = paren;
                this.args = args;
                this.thiz = thiz;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitCallExpr(this);
            }
        }

        public class Dict: Expr
        {
            public List<Expr> entries;

            public Dict(List<Expr> entries)
            {
                this.entries = entries;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitDictExpr(this);
            }
        }

        public class Get: Expr
        {
            public Expr entity;
            public Expr key;

            public Get(Expr entity, Expr key)
            {
                this.entity = entity;
                this.key = key;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitGetExpr(this);
            }
        }

        public class Group: Expr
        {
            public Expr expression;

            public Group(Expr expression)
            {
                this.expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitGroupExpr(this);
            }
        }

        public class Key: Expr
        {
            public Token name;

            public Key(Token name)
            {
                this.name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitKeyExpr(this);
            }
        }

        public class Lambda: Expr
        {
            public Stmt lambda;

            public Lambda(Stmt lambda)
            {
                this.lambda = lambda;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitLambdaExpr(this);
            }
        }

        public class Logical: Expr
        {
            public Expr left;
            public Token oprtr;
            public Expr right;

            public Logical(Expr left, Token oprtr, Expr right)
            {
                this.left = left;
                this.oprtr = oprtr;
                this.right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitLogicalExpr(this);
            }
        }

        public class Array: Expr
        {
            public List<Expr> values;

            public Array(List<Expr> values)
            {
                this.values = values;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitArrayExpr(this);
            }
        }

        public class Literal: Expr
        {
            public object value;
            public DataType type;

            public Literal(object value, DataType type)
            {
                this.value = value;
                this.type = type;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitLiteralExpr(this);
            }
        }

        public class Range: Expr
        {
            public Expr start;
            public Expr end;
            public Expr step;

            public Range(Expr start, Expr end, Expr step)
            {
                this.start = start;
                this.end = end;
                this.step = step;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitRangeExpr(this);
            }
        }

        public class Set: Expr
        {
            public Expr entity;
            public Expr key;
            public Expr value;

            public Set(Expr entity, Expr key, Expr value)
            {
                this.entity = entity;
                this.key = key;
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitSetExpr(this);
            }
        }

        public class Ternary: Expr
        {
            public Expr condition;
            public Expr thenExpr;
            public Expr elseExpr;

            public Ternary(Expr condition, Expr thenExpr, Expr elseExpr)
            {
                this.condition = condition;
                this.thenExpr = thenExpr;
                this.elseExpr = elseExpr;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitTernaryExpr(this);
            }
        }

        public class Unary: Expr
        {
            public Token oprtr;
            public Expr right;

            public Unary(Token oprtr, Expr right)
            {
                this.oprtr = oprtr;
                this.right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitUnaryExpr(this);
            }
        }

        public class Variable: Expr
        {
            public Token name;

            public Variable(Token name)
            {
                this.name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitVariableExpr(this);
            }
        }

        public class String: Expr
        {
            public string value;

            public String(string value)
            {
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                 return visitor.VisitStringExpr(this);
            }
        }

    }
}
