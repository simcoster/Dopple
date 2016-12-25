using System;
using System.Collections.Generic;

namespace GraphSimilarity
{
    public class DuplicateKeySortedDictionary : SortedDictionary<int, List<EditPath>>
    {
        public void Add(EditPath editPath)
        {
            if (!this.ContainsKey(editPath.CumelativeCostPlusHeuristic))
            {
                this.Add(editPath.CumelativeCostPlusHeuristic, new List<EditPath>());
            }
            this[editPath.CumelativeCostPlusHeuristic].Add(editPath);
        }

        public void Remove(EditPath editPath)
        {
            this[editPath.CumelativeCostPlusHeuristic].Remove(editPath);
            if (this[editPath.CumelativeCostPlusHeuristic].Count ==0)
            {
                this.Remove(editPath.CumelativeCostPlusHeuristic);
            }
        }
    }
}