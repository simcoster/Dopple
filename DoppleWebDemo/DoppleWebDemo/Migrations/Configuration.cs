namespace DoppleWebDemo.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DoppleWebDemo.Models.FunctionComparisonDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DoppleWebDemo.Models.FunctionComparisonDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
                //context.FunctionComparisons.AddOrUpdate(
                //  p => p.Index,
                //  new Models.FunctionComparison{ FirstFunctionCode ="blah", SecondFunctionCode = "blahlahal"}
                //);
            
        }
    }
}
