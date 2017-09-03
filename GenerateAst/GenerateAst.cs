using System;
using System.Collections.Generic;
using System.IO;

namespace GenerateAst
{
    internal class GenerateAst
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1) 
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(1);
            }
            string outputDir = args[0];
            defineAst(outputDir, "Expr", new List<string>
            {
                "Assign   : Token name, Expr value",
                "Binary   : Expr left, Token op, Expr right",
                "Grouping : Expr expression",
                "Literal  : object value",
                "Unary    : Token op, Expr right",
                "Variable : Token name",
            });
            
            defineAst(outputDir, "Stmt", new List<string>
            {
                "Block      : List<Stmt> statements",
                "Expression : Expr expression",
                "Print      : Expr expression",
                "Var        : Token name, Expr initializer",
            });
        }
        
        private static void defineAst(
            string outputDir, string baseName, List<string> types)
        {
            string path = outputDir + "/" + baseName + ".cs";
            
            StreamWriter writer = new StreamWriter(path);

            writer.WriteLine("using Lox;");
            writer.WriteLine("using System.Collections.Generic;");

            writer.WriteLine("");
            writer.WriteLine($"public abstract class {baseName} \n{{");
            
            defineVisitor(writer, baseName, types);

            // The AST classes.
            foreach (string type in types)
            {
                string className = type.Split(':')[0].Trim();
                string fields = type.Split(':')[1].Trim(); 
                defineType(writer, baseName, className, fields);
            }
            
            // The base accept() method.
            writer.WriteLine("");
            writer.WriteLine("\tpublic abstract R accept<R>(Visitor<R> visitor);");
            
            writer.WriteLine("}");
            writer.Close();
        }
        
        private static void defineType(
            StreamWriter writer, string baseName,
            string className, string fieldList) {
            writer.WriteLine("");
            writer.WriteLine($"\tpublic class {className} : {baseName} \n\t{{");

            // Constructor.
            writer.WriteLine($"\t\tpublic {className} ( {fieldList} ) \n\t\t{{");

            // Store parameters in fields.
            string[] fields = fieldList.Split(new []{", "}, StringSplitOptions.None);
            foreach (string field in fields) 
            {
                string name = field.Split(' ')[1];
                writer.WriteLine($"\t\t\tthis.{name} = {name};");
            }

            writer.WriteLine("\t\t}");
            
            // Visitor pattern.
            writer.WriteLine();
            writer.WriteLine("\t\tpublic override R accept<R>(Visitor<R> visitor) \n\t\t{");
            writer.WriteLine($"\t\t\treturn visitor.visit{className}{baseName}(this);");
            writer.WriteLine("\t\t}");

            // Fields.
            writer.WriteLine();
            foreach (string field in fields) {
                writer.WriteLine($"\t\tpublic {field};");
            }

            writer.WriteLine("\t}");
        }
        
        private static void defineVisitor(
            StreamWriter writer, string baseName, List<string> types) 
        {
            writer.WriteLine("\tpublic interface Visitor<R> \n\t{");

            foreach (string type in types) 
            {
                string typeName = type.Split(':')[0].Trim();
                writer.WriteLine("\t\tR visit" + typeName + baseName + "(" +
                               typeName + " " + baseName.ToLower() + ");");
            }

            writer.WriteLine("\t}");
        }
    }
}