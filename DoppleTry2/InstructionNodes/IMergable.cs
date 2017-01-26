namespace DoppleTry2.InstructionNodes
{
    internal interface IMergable
    {
        void MergeInto(InstructionNode nodeToMergeInto);
    }
}