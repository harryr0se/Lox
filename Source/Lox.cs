using System;
using System.Collections.Generic;
using System.IO;

namespace Lox
{
    public class Lox
    {
        private static bool hadError = false;
        
        public static void Main(string[] args)
        {
            if (args.Length > 1) 
            {
                Console.Write("Usage: jlox [script]");
            }
            else if (args.Length == 1) 
            {
                runFile(args[0]);
            } 
            else 
            {
                runPrompt();
            }
        }
        
        private static void runFile(string path)
        {
            run(File.ReadAllText(path));
            if (hadError)
            {
                Environment.Exit(65);
            }
        }
        
        private static void runPrompt()
        {
            for (;;) 
            { 
                Console.Write("-> ");
                run(Console.ReadLine());
                hadError = false;
            }
        }
        
        private static void run(string source) 
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();

            Parser parser = new Parser(tokens);
            Expr expression = parser.parse();

            // Stop if there was a syntax error.
            if (hadError) return;

            Console.WriteLine(new AstPrinter().print(expression));
        }
        
        public static void error(int line, string message) 
        {
            report(line, "", message);
        }
        
        private static void report(int line, string where, string message) 
        {
            Console.Error.WriteLine($"[line {line} ] Error {where}: {message}");
            hadError = true;
        }
        
        public static void error(Token token, string message) 
        {
            if (token.type == TokenType.EOF) 
            {
                report(token.line, " at end", message);
            }
            else 
            {
                report(token.line, " at '" + token.lexeme + "'", message);
            }
        }
        
    }
}