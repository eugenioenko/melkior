using System;
using System.IO;
using System.Collections.Generic;

namespace Melkior
{
    public class Melkior
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                HelpAndVersion();
                return;
            }
            var filename = args[0];
            if (!File.Exists(filename))
            {
                HelpAndVersion();
                return;
            }
            string source = File.ReadAllText(filename);
            Execute(source);
        }

        static void HelpAndVersion()
        {
            Console.WriteLine("Melkior interpreter version 0.8");
            Console.WriteLine("usage: melkior [source] [arguments]");
            Console.WriteLine("");
            Console.WriteLine("source: the filename of the sourcecode to be executed");
            Console.WriteLine("arguments: an array list of arguments to be passed to the main/entry function");
            Console.WriteLine("example: melkior source_code.mel ['first argument', 123]");
        }

        public static List<Stmt> Execute(string source) 
        {
            Scanner scanner = new Scanner();
            List<Token> tokens = scanner.Scan(source);
            if (tokens == null)
            {
                return null;
            }

            Parser parser = new Parser();
            var statements = parser.Parse(tokens);

            if (statements == null)
            {
                return null;
            }
            Interpreter interpreter = new Interpreter();
            return interpreter.Interpret(statements);
        }
    }
}
