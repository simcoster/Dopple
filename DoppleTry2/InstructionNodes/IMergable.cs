namespace Dopple.InstructionNodes
{
    internal interface IMergable
    {
        void MergeInto(InstructionNode nodeToMergeInto);
    }
}