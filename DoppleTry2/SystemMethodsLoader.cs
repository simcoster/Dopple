using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DoppleTry2
{
    class SystemMethodsLoader
    {
        Dictionary<string, MethodDefinition> SystemMethods = new Dictionary<string,MethodDefinition>();
        public SystemMethodsLoader()
        {
            List<AssemblyDefinition> myLibraries = new List<AssemblyDefinition>();
            myLibraries.Add(AssemblyDefinition.ReadAssembly(@"C:\Windows\assembly\GAC_MSIL\System.Core\3.5.0.0__b77a5c561934e089\system.core.dll"));
            myLibraries.Add(AssemblyDefinition.ReadAssembly(@"C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\mscorlib.dll"));
            foreach (var library in myLibraries)
            {
                //TypeDefinition type = library.MainModule.Types.First(x => x.FullName == "System.Linq.Enumerable");
                foreach(var typeDef in library.MainModule.Types)
                {
                    foreach (var method in typeDef.Methods.Where(x => x.FullName.Contains("Sum")).ToList())
                    {
                        if (SystemMethods.ContainsKey(method.FullName))
                        {
                            continue;
                        }
                        SystemMethods.Add(method.FullName, method);
                    }
                }         
            }
        }

        internal bool TryGetSystemMethod(Instruction instruction, out MethodDefinition systemMethodDef)
        {
            var metRef = (MethodReference) instruction.Operand;
            string nameToSearch = metRef.FullName;
            nameToSearch = nameToSearch.Replace("!!0", "T");
            nameToSearch = Regex.Replace(nameToSearch, "<[^ ]*?>\\(", "(");
            nameToSearch = Regex.Replace(nameToSearch, "<[^ ]*?>::", "::");
            if (SystemMethods.ContainsKey(nameToSearch))
            {
                systemMethodDef = SystemMethods[nameToSearch];
                return true;
            }
            systemMethodDef = null;
            return false;
        }
    }
}
