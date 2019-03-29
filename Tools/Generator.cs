using System;
using System.Collections.Generic;
using System.IO;

namespace generator
{
    class Generator
    {
        public static readonly Dictionary<string, string[]> expressions =
            new Dictionary<string, string[]> {
                { "Assign", new string[]{"Token name", "Expr value"}},
                { "Binary", new string[]{"Expr left", "Token oprtr", "Expr right"}},
                { "Call", new string[]{"Expr callee", "Token paren", "List<Expr> args", "object thiz"}},
                { "Dict", new string[]{"List<Expr> entries"}},
                { "Get", new string[]{"Expr entity", "Expr key"}},
                { "Group", new string[]{"Expr expression"}},
                { "Key", new string[]{"Token name"}},
                // { "Lambda", new string[]{"Stmt lambda"}},
                { "Logical", new string[]{"Expr left", "Token oprtr", "Expr right"}},
                { "Array", new string[]{"List<Expr> values"}},
                { "Literal", new string[]{"object value", "DataType type"}},
                // { "New", new string[]{"Expr constructor"}},
                // { "Postfix", new string[]{"Token name", "number increment"}},
                // { "Range", new string[]{"Expr start", "Expr end", "Expr step"}},
                // { "RegEx", new string[]{"RegExp value"}},
                { "Set", new string[]{"Expr entity", "Expr key", "Expr value"}},
                // { "Super", new string[]{"List<Token> index", "List<Expr> args"}},
                { "Ternary", new string[]{"Expr condition", "Expr thenExpr", "Expr elseExpr"}},
                { "Unary",  new string[]{ "Token oprtr", "Expr right"}},
                { "Variable", new string[]{"Token name"}},
                { "String", new string[]{"string value"}}
        };

        public static readonly Dictionary<string, string[]> statements =
            new Dictionary<string, string[]> {
                { "Block", new string[]{"List<Stmt> statements"}},
                // { "Class", new string[]{"Token name", "Token parent", "List<Function> methods"}},
                { "DoWhile", new string[]{"Stmt loop", "Expr condition"}},
                { "Expression", new string[]{"Expr expression"}},
                { "Function", new string[]{"Token name", "List<Token> prms", "Stmt body"}},
                { "If", new string[]{"Expr condition", "Stmt thenStmt", "Stmt elseStmt"}},
                { "Print", new string[]{"Expr expression"}},
                { "Return", new string[]{"Expr value"}},
                { "Var", new string[]{"Token name", "Token type", "Expr initializer", "bool writable"}},
                { "While", new string[]{"Expr condition", "Stmt loop"}},
                { "Foreach", new string[]{ "Token item", "Token key", "Token iterable", "Stmt loop"}}
        };
  
        public static void GenerateAST(string className, Dictionary<string, string[]> ast, string filename, string imports)
        {
            List<string> lines = new List<string>();

            lines.Add("using System.Collections.Generic;");
            lines.Add("");
            lines.Add("namespace Melkior");
            lines.Add("{");
            lines.Add("    abstract class " + className);
            lines.Add("    {");
            lines.Add("        public interface IVisitor<T>");
            lines.Add("        {");
            foreach (string name in ast.Keys)
            {
                lines.Add("            T Visit" + name + className + "(" + name + " " + className.ToLower() + ");");
            }
            lines.Add("        }");
            lines.Add("");
            lines.Add("        public abstract T Accept<T>(IVisitor<T> visitor);");
            lines.Add("");
            foreach (var item in ast)
            {
                lines.Add("        public class " + item.Key + ": " + className);
                lines.Add("        {");
                foreach (var member in item.Value)
                {
                    lines.Add("            public " + member + ";" );
                }
                lines.Add("");
                lines.Add("            public " + item.Key + "(" + String.Join(", ", item.Value) +")");
                lines.Add("            {");
                foreach (var member in item.Value)
                {
                    var name = member.Split(' ')[1];
                    lines.Add("                this." + name + " = " + name + ";");
                }
                lines.Add("            }");
                lines.Add("");
                lines.Add("            public override T Accept<T>(IVisitor<T> visitor)");
                lines.Add("            {");
                lines.Add("                 return visitor.Visit" + item.Key + className + "(this);");
                lines.Add("            }");
                lines.Add("        }");
                lines.Add("");
            }
          
            lines.Add("    }");
            lines.Add("}");
            File.WriteAllLines(filename, lines.ToArray());
        }



        static void Main(string[] args)
        {
            GenerateAST("Expr", expressions, "C:\\Users\\Baires\\Source\\Repos\\atscript\\Melkior\\DataStructures\\Expressions.cs", "");
            GenerateAST("Stmt", statements, "C:\\Users\\Baires\\Source\\Repos\\atscript\\Melkior\\DataStructures\\Statements.cs", "");
            Console.WriteLine("Expressions and Statements generated");
        }
    }
}
