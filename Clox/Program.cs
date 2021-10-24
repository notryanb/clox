using System;
using System.Collections.Generic;
using System.IO;

namespace Clox
{
    class Program
    {
        static Boolean HadError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage clock [script]");

                Environment.ExitCode = 65;
                return;
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }

            // Testing the AST printer to ensure the Visitor pattern works.
            //Expr expression = new Expr.Binary(
            //    new Expr.Unary(
            //        new Token(TokenType.MINUS, "-", null, 1),
            //        new Expr.Literal(123)),
            //    new Token(TokenType.STAR, "*", null, 1),
            //    new Expr.Grouping(new Expr.Literal(45.67)));

            //Console.WriteLine(new AstPrinter().Print(expression));
        }

        private static void RunFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Run(bytes.ToString());

            if (HadError)
            {
                // Might want to look into exiting with system error codes.
                Environment.ExitCode = 65;
                return;
            }
        }

        private static void RunPrompt()
        {
            for(;;)
            {
                Console.WriteLine("> ");
                var line = Console.ReadLine();
                if (line == null) break;
                Run(line);
                HadError = false;
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine("[line " + line + " ] Error" + where + ": " + message);
            HadError = true;
        }
    }
}
