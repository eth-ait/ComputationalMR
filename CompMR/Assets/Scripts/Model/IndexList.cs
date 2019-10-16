using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class IndexList : MonoBehaviour
    {
        public string name;

        public List<int> Indices;

        public int Length()
        {
            if (Indices == null)
                return -1;

            return Indices.Count;
        }
    }
}