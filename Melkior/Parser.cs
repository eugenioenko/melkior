using System;
using System.Collections.Generic;

namespace Melkior
{
    class Parser
    {
        private int current;
        private List<Token> tokens;

        public Parser() {
            current = 0;
        }

        public List<Stmt> Parse(List<Token> tokens)
        {
            this.tokens = tokens;
            current = 0;
            var statements = new List<Stmt>();

            try
            {
                while (!Eof())
                {
                    statements.Add(Declaration());
                }
                return statements;
            }
            catch(MelkiorException e)
            {
                Console.WriteLine(e.message);
                return null;
            }
            catch(Exception)
            {
                Console.WriteLine("[Unhandeled Parse Error] => at " + Peek());
                return null;
            }
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (tokens[current].type == type)
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private Token Advance()
        {
            if (!Eof())
            {
                current++;
            }
            return Previous();
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private bool Check(params TokenType[] types)
        {
            foreach (var type in types) {
                if (tokens[current].type == type)
                {
                    return true;
                }
            }
            return false;
        }

        private bool Eof()
        {
            return tokens[current].type == TokenType.Eof ||
                current >= tokens.Count;
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }
            Error(Previous(), message);
            return null;
        }

        private Token Discard(TokenType type, string message)
        {
            if (Check(type))
            {
                Error(Previous(), message);
            }
            return Peek();
        }

        private void Error(Token token, string message)
        {
            var line = token.line;
            var col = token.column;
            var chr = token.lexeme;
            throw new MelkiorException("[Melkior Parse Error] at (" + line +
                ":" + col + ") near `" + chr + "` => " + message);
        }

        private Stmt Declaration()
        {
            if (Match(TokenType.Var))
            {
                return VarStatement(Previous().type);
            }
            if (Match(TokenType.Func))
            {
                return FuncStatement();
            }
            if (Match(TokenType.Class))
            {
                return ClassStatement();
            }

            return Statement();
        }

        private Stmt VarStatement(TokenType varType)
        {
            Token name = Consume(TokenType.Identifier, "Expected a name for a variable");
            // type definition
            // Consume(TokenType.Colon, "Expected a type definition after variable name");
            // Token type = Consume(TokenType.Identifier, "Expected a type after variable name");
            Expr initializer = null;
            if (Match(TokenType.Equal))
            {
                initializer = Expression();
            }
            Consume(TokenType.Semicolon, "Expected semicolon after a variable initialization");
            var writable = varType == TokenType.Var ? true : false;
            return new Stmt.Var(name, null, initializer, writable).Line(Previous());
        }

        private Stmt ClassStatement()
        {
            Token name = Consume(TokenType.Identifier, "Expected class name after class statement");
            Token parent = null;
            if (Match(TokenType.Inherits))
            {
                parent = Consume(TokenType.Identifier, "Expected a parent class name after inhertis");
            }
            var methods = new List<Stmt>();

            var clazz = new Stmt.Class(name, parent, methods).Line(name);

            if (Match(TokenType.End))
            {
                return clazz;
            }

            do
            {
                methods.Add(FuncStatement());
            } while (!Match(TokenType.End));

            return clazz;
        }

        private Stmt FuncStatement()
        {
            Token name = Consume(TokenType.Identifier, "Expected a function name after func keyword");
            List<Token> parameters = FuncParameters();
            Discard(TokenType.Do, "Unexpected 'do' at the start of function body");
            List<Stmt> body = Block(TokenType.End);
            Consume(TokenType.End, "Expected 'end' at the end of a function");
            return new Stmt.Function(name, parameters, body).Line(name);
        }

        private List<Token> FuncParameters()
        {
            Consume(TokenType.LeftParen, "Expected parameter definition after function name");
            var parameters = new List<Token>();
            if (!Check(TokenType.RightParen))
            {
                do
                {
                    parameters.Add(Consume(TokenType.Identifier, "Expected a parameter name"));
                } while (Match(TokenType.Comma));
            }
            Consume(TokenType.RightParen, "Expected ')' after function parameters");
            return parameters;
        }

        private Stmt Statement()
        {
            if (Match(TokenType.Print))
            {
                return PrintStatement();
            }
            if (Match(TokenType.If))
            {
                return IfStatement();
            }
            if (Match(TokenType.While))
            {
                return WhileStatement();
            }
            if (Match(TokenType.For))
            {
                return ForStatement();
            }
            if (Match(TokenType.Foreach))
            {
                return ForeachStatement();
            }
            if (Match(TokenType.Do))
            {
                return BlockStatement();
            }
            if (Match(TokenType.Repeat))
            {
                return RepeatStatement();
            }
            if (Match(TokenType.Return))
            {
                return ReturnStatement();
            }
            if (Match(TokenType.Pause))
            {
                return PauseStatement();
            }
           
            return ExpressionStatement();
        }

        private Stmt PrintStatement()
        {
            var value = Expression();
            Consume(TokenType.Semicolon, "Expected a semicolon after print statement");
            return new Stmt.Print(value).Line(Previous());
        }

        private Stmt PauseStatement()
        {
            return new Stmt.Pause().Line(Previous());
        }

        private Stmt BlockStatement()
        {
            var block = new Stmt.Block(Block(TokenType.End));
            Consume(TokenType.End, "Expected 'end' at the end of the block");
            return block;
        }

        private Stmt IfStatement()
        {
            var line = Previous();
            Expr condition = Expression();
            Consume(TokenType.Then, "Expected 'then' after if condition");
            Discard(TokenType.Do, "Unexpected 'do' after 'then'");
            Stmt thenStmt = new Stmt.Block(Block(TokenType.End, TokenType.Else, TokenType.Elseif)).Line(line);

            if (Match(TokenType.End))
            {
                return new Stmt.If(condition, thenStmt, null).Line(line);
            }

            Stmt elseStmt = null;
            if (Match(TokenType.Else))
            {
                elseStmt = new Stmt.Block(Block(TokenType.End));
                Consume(TokenType.End, "Expected 'end' after else block");
                return new Stmt.If(condition, thenStmt, elseStmt).Line(line);
            }

            if (Match(TokenType.Elseif))
            {
                elseStmt = IfStatement();
                return new Stmt.If(condition, thenStmt, elseStmt).Line(line);
            }

            Consume(TokenType.End, "Expected 'end' after if block");
            return null;
        }

        private Stmt WhileStatement()
        {
            var line = Previous();
            Expr condition = Expression();
            Stmt loop = Statement();

            return new Stmt.While(condition, loop).Line(line);
        }

        private Stmt RepeatStatement()
        {
            var line = Previous();
            Stmt loop = new Stmt.Block(Block(TokenType.While));
            Consume(TokenType.While, "while condition expected after repeat block");
            Expr condition = Expression();
            return new Stmt.DoWhile(loop, condition).Line(line);
        }

        private List<Stmt> Block(params TokenType[] enders)
        {
            List<Stmt> statements = new List<Stmt>();
            while(!Check(enders) && !Eof())
            {
                statements.Add(Declaration());
            }
            return statements;
        }

        private Stmt ReturnStatement()
        {
            var keyword = Previous();
            var current = Peek();
            Expr value = null;
            if (keyword.line == current.line)
            {
                value = Expression();
            }
            Consume(TokenType.Semicolon, "Expected a semicolon after return statement");
            return new Stmt.Return(value).Line(keyword);
        }

        private Stmt ForStatement()
        {
            throw new NotImplementedException();
        }

        private Stmt ForeachStatement()
        {
            Token item = Consume(TokenType.Identifier, "Expected a variable name after foreach");
            Token key = null;
            if (Match(TokenType.With))
            {
                key = Consume(TokenType.Identifier, "Expected an index name after 'with'");
            }
            Consume(TokenType.In, "Expected 'in' after foreach variable");
            Expr iterable = Primary();
            Stmt loop = Statement();
            return new Stmt.Foreach(item, key, iterable, loop).Line(item);
        }
      
        private Stmt ExpressionStatement()
        {
            var expression = Expression();
            var line = Consume(TokenType.Semicolon, "Expected a semicolon after an expression");
            return new Stmt.Expression(expression).Line(line);
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            Expr expr = Ternary();
            if (Match(TokenType.Equal, TokenType.PlusEqual,
            TokenType.MinusEqual, TokenType.StarEqual, TokenType.SlashEqual))
            {
                var oprtr = Previous();
                var value = Assignment();

                if (expr is Expr.Variable)
                {
                    if (oprtr.type != TokenType.Equal)
                    {
                        value = new Expr.Binary(
                            new Expr.Variable((expr as Expr.Variable).name), 
                            oprtr, value
                        );
                    }
                    return new Expr.Assign((expr as Expr.Variable).name, value);
                } 
                else if (expr is Expr.Get)
                {
                    if (oprtr.type != TokenType.Equal)
                    {
                        value = new Expr.Binary(
                            new Expr.Get((expr as Expr.Get).entity, (expr as Expr.Get).key), 
                            oprtr, value);
                    }
                    return new Expr.Set((expr as Expr.Get).entity, (expr as Expr.Get).key, value);
                }
            }
            return expr;
        }

        private Expr Ternary()
        {
            Expr expr = LogicalOr();
            if (Match(TokenType.Yep))
            {
                Expr thenExpr = Ternary();
                Consume(TokenType.Nop, "Expected an 'else' after 'then'");
                Expr elseExpr = Ternary();
                return new Expr.Ternary(expr, thenExpr, elseExpr);
            }
            return expr;
        }

        private Expr LogicalOr()
        {
            Expr expr = LogicalAnd();
            while (Match(TokenType.Or))
            {
                Token oprtr = Previous();
                Expr right = LogicalAnd();
                expr = new Expr.Logical(expr, oprtr, right);
            }
            return expr;
        }

        private Expr LogicalAnd()
        {
            Expr expr = Equality();
            while (Match(TokenType.And))
            {
                Token oprtr = Previous();
                Expr right = Equality();
                expr = new Expr.Logical(expr, oprtr, right);
            }
            return expr;
        }

        private Expr Equality()
        {
            Expr expr = Comparision();
            while (Match(TokenType.EqualEqual, TokenType.BangEqual))
            {
                Token oprtr = Previous();
                Expr right = Comparision();
                expr = new Expr.Binary(expr, oprtr, right);
            }
            return expr;
        }

        private Expr Comparision()
        {
            Expr expr = Addition();
            while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                Token oprtr = Previous();
                Expr right = Addition();
                expr = new Expr.Binary(expr, oprtr, right);
            }
            return expr;
        }

        private Expr Addition()
        {
            Expr expr = Modulus();
            while (Match(TokenType.Plus, TokenType.Minus))
            {
                Token oprtr = Previous();
                Expr right = Modulus();
                expr = new Expr.Binary(expr, oprtr, right);
            }
            return expr;
        }

        private Expr Modulus()
        {
            Expr expr = Multiplication();
            while (Match(TokenType.Percent))
            {
                Token oprtr = Previous();
                Expr right = Multiplication();
                expr = new Expr.Binary(expr, oprtr, right);
            }
            return expr;
        }

        private Expr Multiplication()
        {
            Expr expr = Unary();
            while (Match(TokenType.Star, TokenType.Slash))
            {
                Token oprtr = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, oprtr, right);
            }
            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.Minus, TokenType.Not, TokenType.Typeof))
            {
                Token oprtr = Previous();
                Expr right = Unary();
                return  new Expr.Unary(oprtr, right);
            }
            return New();
        }

        private Expr New()
        {
            if (Match(TokenType.New))
            {
                var constructor = Call();
                return new Expr.New(constructor);
            }

            return Call();
        }

        private Expr Call()
        {
            var expr = Primary();
            while (true)
            {
                if (Match(TokenType.LeftParen))
                {
                     return DoCall(expr);
                }
                else if (Match(TokenType.Dot))
                {
                   expr = DictionaryGet(expr);
                }
                else if (Match(TokenType.LeftBracket))
                {
                    expr = ArrayGet(expr);
                }
                else
                {
                    break;
                }
            }
   
            return expr;
        }

        private Expr DoCall(Expr expr)
        {
            var callee = expr;
            do
            {
                var args = new List<Expr>();
                if (!Check(TokenType.RightParen))
                {
                    do
                    {
                        args.Add(Expression());
                    }
                    while (Match(TokenType.Comma));
                    
                }
                Consume(TokenType.RightParen, "Expected ')' after function arguments");
                callee = new Expr.Call(callee, args, null);
                return callee;
            }
            while (Match(TokenType.LeftParen));
        }

        private Expr DictionaryGet(Expr expr)
        {
            Token name = Consume(TokenType.Identifier, "Expected a property name after '.'");
            return new Expr.Get(expr, new Expr.Key(name));
        }

        private Expr ArrayGet(Expr expr)
        {
            var index = Expression();
            Consume(TokenType.RightBracket, "Expected closing ']' after array index");
            return new Expr.Get(expr, index);
        }

        private Expr Primary()
        {
            if (Match(TokenType.Null))
            {
                return new Expr.Literal(null, DataType.Null);
            }
            if (Match(TokenType.False))
            {
                return new Expr.Literal(false, DataType.Boolean);
            }
            if (Match(TokenType.True))
            {
                return new Expr.Literal(true, DataType.Boolean);
            }
            if (Match(TokenType.Number))
            {
                return new Expr.Literal(double.Parse(Previous().literal.ToString()), DataType.Number);
            }
            if (Match(TokenType.String))
            {
                return new Expr.Literal(Previous().literal, DataType.String);
            }

            if (Match(TokenType.Base))
            {
                return new Expr.Base();
            }

            if (Match(TokenType.LeftBracket))
            {
                return DoArray();
            }
            if (Match(TokenType.LeftBrace))
            {
                return DoDictionary();
            }
            if (Match(TokenType.Identifier))
            {
                return new Expr.Variable(Previous());
            }
            if (Match(TokenType.LeftParen))
            {
                return DoGroup();
            }
            if (Match(TokenType.Func, TokenType.Lambda))
            {
                return DoLambda();
            }
            if (Match(TokenType.Range))
            {
                return DoRange();
            }

            Error(Peek(), "Unexpected character");
            return null;
        }

        private Expr DoArray()
        {
            var values = new List<Expr>();

            if (Match(TokenType.RightBracket))
            {
                return new Expr.Array(values);
            }

            do
            {
                values.Add(Expression());
            } while (Match(TokenType.Comma));

            Consume(TokenType.RightBracket, "Expected closing ']' after array values");
            return new Expr.Array(values);
        }

        private Expr DoDictionary()
        {
            var entries = new List<Expr>();

            if (Match(TokenType.RightBrace))
            {
                return new Expr.Dict(entries);
            }

            do
            {
                if (Match(TokenType.String, TokenType.Identifier))
                {
                    Token key = Previous();
                    Consume(TokenType.Colon, "Expected ':' colon after member");
                    var value = Expression();
                    entries.Add(new Expr.Set(null, new Expr.Key(key), value));
                }
                else
                {
                    Error(Previous(), "String or identifier expected after Dictionary {");
                }
            } while (Match(TokenType.Comma));

            Consume(TokenType.RightBrace, "Expected closing '}' after dictionary entries");

            return new Expr.Dict(entries);
        }

        private Expr DoGroup()
        {
            var expr = Expression();
            Consume(TokenType.RightParen, "Expected closing ')' after grouping expression");
            return expr;
        }

        private Expr DoLambda()
        {
            Token lambda = Previous();
            List<Token> parameters = FuncParameters();
            List<Stmt> body = new List<Stmt>();
            if (lambda.type == TokenType.Func)
            {
                Discard(TokenType.Do, "Unexpected 'do' at the start of function body");
                body = Block(TokenType.End);
                Consume(TokenType.End, "Expected 'end' after anonymous function body");
            }
            else
            {
                Consume(TokenType.Arrow, "Expected arrow '=>' after lambda parameters");
                body.Add(new Stmt.Return(Expression()));
            }
            var function = new Stmt.Function(lambda, parameters, body);
            return new Expr.Lambda(function);
        }

        private Expr DoRange()
        {
            Expr start = null;
            Expr end = null;
            Expr step = null;

            Consume(TokenType.LeftBracket, "Expected '[' at start of range epxression");
            if (!Check(TokenType.Colon))
            {
                start = Expression();
            }
            if (Match(TokenType.Colon) && !Check(TokenType.Colon))
            {
                end = Expression();
            }
            if (Match(TokenType.Colon) && !Check(TokenType.RightBracket))
            {
                step = Expression();
            }
            Consume(TokenType.RightBracket, "Expected ']' after range expression");
            return new Expr.Range(start, end, step);
        }

    }

}
