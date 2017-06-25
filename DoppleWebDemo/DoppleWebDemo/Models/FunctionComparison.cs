using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DoppleWebDemo.Controllers.Helpers;
using GraphSimilarityByMatching;
using System;

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
        [NotMapped]
        public NodePairings PairingFirstToSecond { get; internal set; }
        [NotMapped]
        public NodePairings PairingSecondToFirst { get; internal set; }

        public FunctionComparison()
        {
            ScoreFirstContainedInSecond = -1.0;
            ScoreSecondContainedInFirst = -1.0;
            ScoreTwoWay = -1.0;
        }

        internal void CalculateScores()
        {
            FirstFuncNodesAndEdges = GraphCreator.CompileCode(FirstFunctionCode);
            SecondFuncNodesAndEdges = GraphCreator.CompileCode(SecondFunctionCode);

            PairingFirstToSecond = GraphSimilarityCalc.GetDistance(FirstFuncNodesAndEdges.Nodes, SecondFuncNodesAndEdges.Nodes);
            PairingSecondToFirst = GraphSimilarityCalc.GetDistance(SecondFuncNodesAndEdges.Nodes, FirstFuncNodesAndEdges.Nodes);

            ScoreFirstContainedInSecond = Math.Round(PairingFirstToSecond.TotalScore / PairingFirstToSecond.SourceSelfPairings.TotalScore, 2);
            ScoreSecondContainedInFirst = Math.Round(PairingSecondToFirst.TotalScore / PairingSecondToFirst.SourceSelfPairings.TotalScore, 2);

            ScoreTwoWay = (PairingFirstToSecond.TotalScore + PairingSecondToFirst.TotalScore) / (PairingSecondToFirst.SourceSelfPairings.TotalScore + PairingFirstToSecond.SourceSelfPairings.TotalScore);
            ScoreTwoWay = Math.Round(ScoreTwoWay.Value, 2);
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

        public System.Data.Entity.DbSet<DoppleWebDemo.Models.Feedback> Feedbacks { get; set; }
    }
}