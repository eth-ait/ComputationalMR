using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Model
{
    public class SolutionModel
    {
        public double Objective { get; set; }

        public List<SolutionElementModel> SolutionElements { get; }

        public SolutionModel()
        {
            SolutionElements = new List<SolutionElementModel>();
        }

        public void AddEntry(int id, int lod, double visibility)
        {
            var existingElement = SolutionElements.SingleOrDefault(element => element.Identifier == id);

            if (existingElement != null)
            {
                existingElement.LodAndVisiblityDictionary.Add(lod, visibility);
            }
            else
            {
                SolutionElementModel element = new SolutionElementModel(id);
                element.LodAndVisiblityDictionary.Add(lod, visibility);
                SolutionElements.Add(element);
            }
        }
    }
}