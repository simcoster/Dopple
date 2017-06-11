namespace DoppleWebDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.function_comparison_table",
                c => new
                    {
                        Index = c.Int(nullable: false, identity: true),
                        FirstFunctionCode = c.String(),
                        SecondFunctionCode = c.String(),
                        ScoreFirstContainedInSecond = c.Double(),
                        ScoreSecondContainedInFirst = c.Double(),
                        ScoreTwoWay = c.Double(),
                    })
                .PrimaryKey(t => t.Index);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.function_comparison_table");
        }
    }
}
