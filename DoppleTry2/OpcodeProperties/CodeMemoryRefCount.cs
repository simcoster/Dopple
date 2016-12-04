using Mono.Cecil.Cil;

namespace DoppleTry2
{
    public class CodeMemoryRefCount
    {
        public CodeMemoryRefCount(Code[] codes, int refCount)
        {
            Codes = codes;
            RefCount = refCount;
        }

        public Code[] Codes { get; private set; }
        public int RefCount { get; private set; }
    }
}