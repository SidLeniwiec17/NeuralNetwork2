using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI2.Content
{
    public class DataSet
    {
        public List<List<int>> Input { get; set; }
        public List<int> Output { get; set; }

        public DataSet(List<Face> faces)
        {
            for(int i = 0 ; i < faces.Count ; i++)
            {
                Input.Add(faces[i].Gradients);
                Output.Add(faces[i].ClassIndex);
            }
        }
    }
}
