using System.Collections.Generic;

namespace ConstraintUI.Model
{
    public class SolutionElementModel
    {
        public int Identifier { get; set; }
        public Dictionary<int, double> LodAndVisiblityDictionary;

        public SolutionElementModel(int id)
        {
            Identifier = id;
            LodAndVisiblityDictionary = new Dictionary<int, double>();
        }
    }
}