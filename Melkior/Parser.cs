using System;
using System.Collections.Generic;

namespace Melkior
{
    class Parser
    {
        private int current;
        private List<Token> tokens;

        public Parser() { }

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
            catch
            {
                Error(Peek(), "Unhandled Parsing Error");
            }

            return null;
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

        private void Error(Token token, string message)
        {
            Console.WriteLine("[Parse Error] => " + token +  ": " + message);
            Console.ReadKey();
            System.Environment.Exit(-1);
           
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
            // Consume(TokenType.Semicolon, "Expected semicolon after a variable initialization");
            var writable = varType == TokenType.Var ? true : false;
            return new Stmt.Var(name, null, initializer, writable);
        }

        private Stmt FuncStatement()
        {
            Token name = Consume(TokenType.Identifier, "Expected a function name after func keyword");
            List<Token> parameters = FuncParameters();
            Stmt body = Statement();

            return new Stmt.Function(name, parameters, body);
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
           
            return ExpressionStatement();
        }

        private Stmt PrintStatement()
        {
            var value = Expression();
            // Consume(TokenType.Semicolon, "Expected semicolon after print expression");
            return new Stmt.Print(value);
        }

        private Stmt BlockStatement()
        {
            var block = new Stmt.Block(Block(TokenType.End));
            Consume(TokenType.End, "Expected 'end' at the end of the block");
            return block;
        }

        private Stmt IfStatement()
        { 
            Expr condition = Expression();
            Consume(TokenType.Then, "Expected 'then' after if condition");
            Stmt thenStmt = new Stmt.Block(Block(TokenType.End, TokenType.Else, TokenType.Elseif));

            if (Match(TokenType.End))
            {
                return new Stmt.If(condition, thenStmt, null);
            }

            Stmt elseStmt = null;
            if (Match(TokenType.Else))
            {
                elseStmt = new Stmt.Block(Block(TokenType.End));
                Consume(TokenType.End, "Expected 'end' after else block");
                return new Stmt.If(condition, thenStmt, elseStmt);
            }

            if (Match(TokenType.Elseif))
            {
                elseStmt = IfStatement();
                return new Stmt.If(condition, thenStmt, elseStmt);
            }

            Consume(TokenType.End, "Expected 'end' after if block");
            return null;
        }

        private Stmt WhileStatement()
        {
            Expr condition = Expression();
            Stmt loop = Statement();

            return new Stmt.While(condition, loop);
        }

        private Stmt RepeatStatement()
        {
            
            Stmt loop = new Stmt.Block(Block(TokenType.While));
            Consume(TokenType.While, "while condition expected after repeat block");
            Expr condition = Expression();
            return new Stmt.DoWhile(loop, condition);
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

            return new Stmt.Return(value);
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
            Token array = Consume(TokenType.Identifier, "Expected an array name in foreach");
            Stmt loop = Statement();
            return new Stmt.Foreach(item, key, array, loop);
        }
      
        private Stmt ExpressionStatement()
        {
            var expression = Expression();
            // Consume(TokenType.Semicolon, "Expected a semicolon after an expression");
            return new Stmt.Expression(expression);
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
                    Token paren = Consume(TokenType.RightParen, "Expected ')' after function arguments");
                    callee = new Expr.Call(callee, paren, args, null);
                }
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
                return new Expr.Literal(Double.Parse(Previous().literal.ToString()), DataType.Number);
            }
            if (Match(TokenType.String))
            {
                return new Expr.Literal(Previous().literal, DataType.String);
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

    }

}
