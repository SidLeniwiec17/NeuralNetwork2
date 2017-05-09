using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI2.Content
{
    public class DataSet
    {
        public List<List<float>> Input { get; set; }
        public List<List<float>> Output { get; set; }

        public DataSet(List<Face> faces, int classes)
        {
            Input = new List<List<float>>();
            Output = new List<List<float>>();
            for(int i = 0 ; i < faces.Count ; i++)
            {
                Input.Add(Normalize(faces[i].Gradients));
                List<float> answers = new List<float>();
                for (int j = 0; j < classes; j++)
                    answers.Add(0.0f);
                answers[faces[i].ClassIndex] = 1.0f;
                Output.Add(answers);
            }
        }
        public static List<float> Normalize(List<int> gradients)
        {
            List<float> grads = new List<float>();
            //0, 1, 2, 3, 4 -> [0,1]
            for (int j = 0; j < gradients.Count; j++)
            {
                grads.Add((float)gradients[j]* 0.25f);
            }
            return grads;
        }
    }
}
