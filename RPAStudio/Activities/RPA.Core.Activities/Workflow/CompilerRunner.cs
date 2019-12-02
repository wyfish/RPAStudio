using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RPA.Core.Activities.Workflow
{
    public class CompilerRunner
    {
        private static readonly string CompiledCodeException = "没有可编译的代码来运行";
        private static readonly string CompileRunnerException = "错误 {0}：{1} 位于行{2}";
        private Assembly _assembly;

        private string _className;

        private string _methodName;

        private string[] _defaultAssemblies = new string[2]
        {
        "System.Data.DataSetExtensions.dll",
        "System.IO.Compression.FileSystem.dll"
        };

        public CompilerRunner(string code, string className, string methodName, int errLineOffset = 0, bool generateInMemory = true)
        {
            _className = className;
            _methodName = methodName;
            Compile(code, errLineOffset, generateInMemory);
        }

        public object Run(object[] args)
        {
            if (_assembly == null)
            {
                throw new Exception(CompiledCodeException);
            }
            return _assembly.GetType(_className).InvokeMember(_methodName, BindingFlags.InvokeMethod, null, _assembly, args);
        }

        private void Compile(string code, int errLineOffset, bool generateInMemory)
        {
            VBCodeProvider vBCodeProvider = new VBCodeProvider();
            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.GenerateInMemory = generateInMemory;
            compilerParameters.GenerateExecutable = false;
            compilerParameters.TreatWarningsAsErrors = false;
            string[] source = (from a in AppDomain.CurrentDomain.GetAssemblies().Where(delegate (Assembly a)
            {
                CultureInfo cultureInfo = a.GetName().CultureInfo;
                return !a.IsDynamic && ((cultureInfo != null && !cultureInfo.Equals(CultureInfo.CurrentCulture)) ? cultureInfo.Equals(CultureInfo.InvariantCulture) : true);
            })
                               group a by a.FullName into a
                               select a.First() into a
                               select a.Location into location
                               where !string.IsNullOrWhiteSpace(location)
                               select location).ToArray();
            compilerParameters.ReferencedAssemblies.AddRange(_defaultAssemblies);
            compilerParameters.ReferencedAssemblies.AddRange((from x in source
                                                              where !_defaultAssemblies.Any((string y) => x.Contains(y))
                                                              select x).ToArray());
            CompilerResults compilerResults = vBCodeProvider.CompileAssemblyFromSource(compilerParameters, code);
            if (!compilerResults.Errors.HasErrors)
            {
                if (generateInMemory)
                {
                    _assembly = compilerResults.CompiledAssembly;
                }
                else
                {
                    try
                    {
                        File.Delete(compilerResults.PathToAssembly);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning(ex.ToString());
                    }
                }
                return;
            }
            _assembly = null;
            throw new ArgumentException(CompiledCodeException + "\n" + GetErrorText(compilerResults, errLineOffset));
        }

        private static string GetErrorText(CompilerResults res, int errLineOffset)
        {
            string text = "";
            foreach (CompilerError error in res.Errors)
            {
                if (!error.IsWarning)
                {
                    string str = string.Format(CompileRunnerException, error.ErrorNumber, error.ErrorText, error.Line - errLineOffset);
                    text = text + str + "\n";
                }
            }
            return text;
        }
    }
}

