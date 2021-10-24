using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace CloxCodeGen
{
    /// <summary>
    /// To Run, 
    ///     - cd <path>/CloxCodenGen
    ///     - dotnet run ..\Clox
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: CloxCodeGen <output_directory>");
            }

            string outputDirectory = args[0];

            DefineAst(outputDirectory, "Expr", new List<string>
            {
                "Binary : Expr left, Token @operator, Expr right",
                "Grouping : Expr expression",
                "Literal : System.Object value",
                "Unary : Token @operator, Expr right",
            });
        }

        private static void DefineAst(string outputDirectory, string baseName, IList<string> types)
        {
            string path = $"{outputDirectory}/{baseName}.cs";

            using var stream = new FileStream(path, FileMode.Create);
            using var writer = new StreamWriter(stream);
            writer.WriteLine("namespace Clox");
            writer.WriteLine("{");

            writer.WriteLine($"\tpublic abstract class {baseName}");
            writer.WriteLine("\t{");

            // Interface
            DefineVisitor(writer, baseName, types);
            writer.WriteLine();

            // base Accept method
            writer.WriteLine($"\t\tpublic abstract T Accept<T>(IVisitor<T> visitor);");
            writer.WriteLine();

            // Concrete type implementations
            foreach (var type in types)
            {
                string className = type.Split(":")[0].Trim();
                string fields = type.Split(":")[1].Trim();
                DefineType(writer, baseName, className, fields);

                writer.WriteLine();
            }            

            writer.WriteLine("\t}");
            writer.WriteLine("}");
        }

        private static void DefineVisitor(StreamWriter writer, string baseName, IEnumerable<string> types)
        {
            writer.WriteLine($"\t\tpublic interface IVisitor<T>");
            writer.WriteLine("\t\t{");

            foreach (var type in types)
            {
                string typeName = type.Split(":")[0].Trim();
                writer.WriteLine($"\t\t\tpublic T Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }

            writer.WriteLine("\t\t}");
        }

        private static void DefineType(StreamWriter writer, string baseName, string className, string fields)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            // Class definition
            writer.WriteLine($"\t\tpublic class {className} : {baseName} ");
            writer.WriteLine("\t\t{");

            // Properties
            string[] fieldArr = fields.Split(", ");
            foreach (string field in fieldArr)
            {
                writer.WriteLine($"\t\t\tpublic {textInfo.ToTitleCase(field)};");
            }

            writer.WriteLine();

            // Constructor
            writer.WriteLine($"\t\t\tpublic {className}({fields})");
            writer.WriteLine("\t\t\t{");
            foreach (string field in fieldArr)
            {
                string name = field.Split(" ")[1];
                writer.WriteLine($"\t\t\t\t{textInfo.ToTitleCase(name)} = {name};");
            }

            writer.WriteLine("\t\t\t}");
            writer.WriteLine();

            // Implement interface
            writer.WriteLine("\t\t\tpublic override T Accept<T>(IVisitor<T> visitor)");
            writer.WriteLine("\t\t\t{");
            writer.WriteLine($"\t\t\t\treturn visitor.Visit{className}{baseName}(this);");
            writer.WriteLine("\t\t\t}");

            writer.WriteLine("\t\t}");
        }
    }
}
