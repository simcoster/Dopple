using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2
{
    public static class MemoryProperties
    {
        private static readonly CodeMemoryRefCount OneMemRead = new CodeMemoryRefCount(
            codes: new[] {  Code.Ldind_I1, Code.Ldind_U1, Code.Ldind_I2,
                Code.Ldind_U2, Code.Ldind_I4, Code.Ldind_U4,
                Code.Ldind_I8, Code.Ldind_I, Code.Ldind_R4,
                Code.Ldind_R8, Code.Ldind_Ref , Code.Cpobj, Code.Ldobj,
                Code.Castclass, Code.Isinst, Code.Unbox, Code.Unbox_Any,Code.Initobj     },
            refCount:1);

        private static readonly CodeMemoryRefCount[] CodeMemoryReadCounts = {OneMemRead};

        private static readonly CodeMemoryRefCount OneMemStore = new CodeMemoryRefCount(
            codes: new[] { Code.Cpobj, Code.Newobj, Code.Box, Code.Localloc, Code.Initobj, Code.Cpblk, }
                         .Concat(CodeGroups.StIndCodes).ToArray(),
            refCount: 1);

        private static readonly CodeMemoryRefCount[] CodeMemoryStoreCounts = { OneMemStore };

        public static int GetMemReadCount(Code code)
        {
            foreach (var codeMemoryRefCount in CodeMemoryReadCounts)
            {
                if (codeMemoryRefCount.Codes.Contains(code))
                {
                    return codeMemoryRefCount.RefCount;
                }
            }
            return 0;
        }

        public static int GetMemStoreCount(Code code)
        {
            foreach (var codeMemoryStoreCount in CodeMemoryStoreCounts)
            {
                if (codeMemoryStoreCount.Codes.Contains(code))
                {
                    return codeMemoryStoreCount.RefCount;
                }
            }
            return 0;
        }
    }
}