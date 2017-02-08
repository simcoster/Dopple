using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2
{
    class SystemMethodsLoader
    {
        Dictionary<string, MethodDefinition> SystemMethods = new Dictionary<string,MethodDefinition>();
        public SystemMethodsLoader()
        {
            AssemblyDefinition myrLibrary = AssemblyDefinition.ReadAssembly(@"C:\Windows\assembly\GAC_MSIL\System.Core\3.5.0.0__b77a5c561934e089\system.core.dll");
            TypeDefinition type = myrLibrary.MainModule.Types.First(x => x.FullName == "System.Linq.Enumerable");
            foreach(var method in type.Methods)
            {
                SystemMethods.Add(method.FullName, method);
            }
        }
        internal bool TryGetSystemMethod(Instruction instruction, out MethodDefinition systemMethodDef)
        {
            var metRef = (MethodReference) instruction.Operand;
            if (SystemMethods.ContainsKey(metRef.FullName))
            {
                systemMethodDef = SystemMethods[((MethodReference) instruction.Operand).FullName];
                return true;
            }
            systemMethodDef = null;
            return false;
        }
    }
}
