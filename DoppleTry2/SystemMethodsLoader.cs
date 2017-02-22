using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace Dopple
{
    class SystemMethodsLoader
    {
        ConcurrentDictionary<string, MethodDefinition> SystemMethods = new ConcurrentDictionary<string,MethodDefinition>();
        public SystemMethodsLoader()
        {
            List<AssemblyDefinition> myLibraries = new List<AssemblyDefinition>();
            myLibraries.Add(AssemblyDefinition.ReadAssembly(@"C:\Windows\assembly\GAC_MSIL\System.Core\3.5.0.0__b77a5c561934e089\system.core.dll"));
            myLibraries.Add(AssemblyDefinition.ReadAssembly(@"C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\mscorlib.dll"));

            foreach (var library in myLibraries)
            {
                Parallel.ForEach(library.MainModule.Types, (typeToLoad) =>
                {
                    Parallel.ForEach(typeToLoad.Methods, (method) =>
                    {
                        SystemMethods.AddOrUpdate(method.FullName, method, (x, y) => y);
                    });
                    Parallel.ForEach(typeToLoad.NestedTypes, (nestedType) =>
                    {
                        Parallel.ForEach(nestedType.Methods, (method) =>
                        {
                            SystemMethods.AddOrUpdate(method.FullName, method, (x, y) => y);
                        });
                    });
                });
            }

            //foreach (var library in myLibraries)
            //{
            //    //TypeDefinition type = library.MainModule.Types.First(x => x.FullName == "System.Linq.Enumerable");
            //    foreach(var typeDef in library.MainModule.Types)
            //    {
            //        foreach (var method in typeDef.Methods.Where(x => x.FullName.Contains("Sum") || x.FullName.Contains("Select")).ToList())
            //        {
            //            if (SystemMethods.ContainsKey(method.FullName))
            //            {
            //                continue;
            //            }
            //            SystemMethods.Add(method.FullName, method);
            //        }
            //    }         
            //}
        }

        internal bool TryGetSystemMethod(Instruction instruction, out MethodDefinition systemMethodDef)
        {
            var metRef = (MethodReference) instruction.Operand;
            string nameToSearch = metRef.FullName;
            nameToSearch = Regex.Replace(nameToSearch, "<[^ ]*?>\\(", "(");
            nameToSearch = Regex.Replace(nameToSearch, "<[^ ]*?>::", "::");
            nameToSearch = nameToSearch.Replace("!!1", "TResult");
            var foundMethod = TryGetMethodDifferentOptions(nameToSearch);
            if (foundMethod != null && foundMethod.HasBody)
            {
                systemMethodDef = foundMethod;
                return true;
            }
            systemMethodDef = null;
            return false;
        }

        private MethodDefinition TryGetMethodDifferentOptions(string nameToSearch)
        {
            string nameToSearchOption1 = nameToSearch.Replace("!!0", "T");
            if (SystemMethods.ContainsKey(nameToSearchOption1))
            {
                return SystemMethods[nameToSearchOption1];
            }
            string nameToSearchOption2 = nameToSearch.Replace("!!0", "TSource");
            if (SystemMethods.ContainsKey(nameToSearchOption2))
            {
                return SystemMethods[nameToSearchOption2];
            }
            string nameToSearchOption3 = nameToSearch.Replace("!!0", "TResult");
            if (SystemMethods.ContainsKey(nameToSearchOption3))
            {
                return SystemMethods[nameToSearchOption3];
            }
            return null;
        }
    }
}
