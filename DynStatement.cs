using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis.Emit;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using FluentFTP;

namespace testorm
{
    internal class DynStatement
    {
        static Action<string> Write = Console.WriteLine;

        public List<Student> executeCode(List<Student> list)
        {
            Write("Let's compile!");

            string codeToCompile = @"
            using System;
            using System.Text;
            using System.Collections.Generic;
            using testorm;
            namespace RoslynCompileSample
            {
                public class Writer
                {
                    public void Write(List<CellItem> list)
                    {
                        
                    }
                }
            }";
            Write("Parsing the code into the SyntaxTree");
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);

            string assemblyName = Path.GetRandomFileName();
            Write("create the random file :"+assemblyName);
            
            string basePath = Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location)??"";
            var refPaths = new[] {
                typeof(System.Object).GetTypeInfo().Assembly.Location,
                typeof(JToken).GetTypeInfo().Assembly.Location,
                typeof(Student).GetTypeInfo().Assembly.Location,
                typeof(IFtpClient).GetTypeInfo().Assembly.Location,
                Path.Combine(basePath, "system.dll"),
                Path.Combine(basePath, "system.data.dll"),
                Path.Combine(basePath, "System.Runtime.dll")
            };
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

            Write("Adding the following references");
            foreach (var r in refPaths)
                Write(r);

            Write("Compiling ...");
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (!result.Success)
                {
                    Write("Compilation failed!");
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    Write("Compilation successful! Now instantiating and executing the code ...");
                    ms.Seek(0, SeekOrigin.Begin);

                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetType("RoslynCompileSample.Writer");
                    var instance = assembly.CreateInstance("RoslynCompileSample.Writer");
                    var meth = type.GetMember("Write").First() as MethodInfo;
                    if(meth != null && instance != null)
                    {
                        object? obj = meth.Invoke(instance, new[] { list });
                        if(obj != null && typeof(List<Student>) == obj.GetType())
                        {
                            return (List<Student>)obj;
                        }
                    }
                }
                return null;
            }
        }
    }
}
