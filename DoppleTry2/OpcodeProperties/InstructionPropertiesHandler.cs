using Mono.Cecil.Cil;

namespace DoppleTry2.OpcodeProperties
{
    public static class InstructionPropertiesHandler
    {
        public static int GetMemReadCount(Code code)
        {
            return MemoryProperties.GetMemReadCount(code);
        }
        public static int GetMemStoreCount(Code code)
        {
            return MemoryProperties.GetMemStoreCount(code);
        }
        public static bool HasDataflowBackRelated(Code code)
        {
            return DataflowProperties.HasDataflowBackRelated(code);
        }

    }
}