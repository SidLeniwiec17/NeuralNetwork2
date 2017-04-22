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
        public List<List<int>> Output { get; set; }

        public DataSet(List<Face> faces, int classes)
        {
            Input = new List<List<int>>();
            Output = new List<List<int>>();
            for(int i = 0 ; i < faces.Count ; i++)
            {
                Input.Add(faces[i].Gradients);
                List<int> answers = new List<int>();
                for (int j = 0; j < classes; j++)
                    answers.Add(0);
                answers[faces[i].ClassIndex] = 1;
                Output.Add(answers);
            }
        }
    }
}
