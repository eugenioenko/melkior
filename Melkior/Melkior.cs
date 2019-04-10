using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

namespace Melkior
{
    public class Melkior
    {
        [DllImport("kernel32.dll")]
        static extern void GetCurrentThreadStackLimits(out uint lowLimit, out uint highLimit);

        [DllImport("kernel32.dll")]
        static extern bool SetThreadStackGuarantee(out uint StackSizeInBytes);

        static public uint GetThreadStackSize()
        {
            uint size = 4 * 1024 * 1024;
            SetThreadStackGuarantee(out size);

            uint low;
            uint high;
            GetCurrentThreadStackLimits(out low, out high);
            return (high - low) / 1024;
        }

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

            var options = new List<string>(args);
            if (options.IndexOf("--t") != -1)
            {
                Transpile(source, filename + ".js");
            }
            else
            {
                Execute(source);
            }
        }

        static void HelpAndVersion()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Melkior interpreter version 0.8");
            Console.WriteLine("usage: melkior [source] [arguments]");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("source: the filename of the sourcecode to be executed");
            Console.WriteLine("arguments: an array list of arguments to be passed to the main/entry function");
            Console.WriteLine("example: melkior source_code.mel ['first argument', 123]");
            Console.WriteLine("Current Stack Size {0}Kb", GetThreadStackSize());
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
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
            var result = interpreter.Interpret(statements);
            return result;
        }

        public static void Transpile(string source, string filename)
        {
            Scanner scanner = new Scanner();
            List<Token> tokens = scanner.Scan(source);
            if (tokens == null)
            {
                return;
            }

            Parser parser = new Parser();
            var statements = parser.Parse(tokens);

            if (statements == null)
            {
                return;
            }

            Transpiler transpiler = new Transpiler();
            var transpiled = transpiler.Transpile(statements);
            File.WriteAllText(filename, transpiled);
        }
    }
}
