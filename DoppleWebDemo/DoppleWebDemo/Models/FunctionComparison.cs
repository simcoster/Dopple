using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DoppleWebDemo.Controllers.Helpers;

namespace DoppleWebDemo.Models
{
    [Table("function_comparison_table")]
    public class FunctionComparison
    {
        [Key]
        public int Index { get; set; }
        [DataType(DataType.MultilineText)]
        public string FirstFunctionCode { get; set; }
        [DataType(DataType.MultilineText)]
        public string SecondFunctionCode { get; set; }
        public double? ScoreFirstContainedInSecond { get; set; }
        public double? ScoreSecondContainedInFirst { get; set; }
        public double? ScoreTwoWay { get; set; }
        [NotMapped]
        public NodesAndEdges FirstFuncNodesAndEdges { get; set; }
        [NotMapped]
        public NodesAndEdges SecondFuncNodesAndEdges { get; set; }


        public FunctionComparison()
        {
            ScoreFirstContainedInSecond = -1.0;
            ScoreSecondContainedInFirst = -1.0;
            ScoreTwoWay = -1.0;
        }
    }
    public class FunctionComparisonDBContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<FunctionComparisonDBContext>(null);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<FunctionComparison> FunctionComparisons { get; set; }
    }
}