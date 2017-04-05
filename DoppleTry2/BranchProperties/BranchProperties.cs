namespace Dopple.BranchPropertiesNS
{
    public class BranchProperties
    {
        private static BaseBranch BaseBranch = new BaseBranch();
        public BranchList Branches { get; private set; } = new BranchList() { BaseBranch };
        public MergingNodeProperties MergingNodeProperties { get; private set; } = new MergingNodeProperties();
    }
}
