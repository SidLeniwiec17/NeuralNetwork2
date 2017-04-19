using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI2.Content
{
    [Serializable]
    public struct Layer
    {
        private int inputVal;
        private int outputVal;
        private float[,] values;

        public int InputNeurons
        {
            get{
                return inputVal;
            }
            set{
                inputVal = value;
            }
        }
        public int OutputNeurons
        {
            get{
                return outputVal;
            }
            set{
                outputVal = value;
            }
        }
        public float[,] Values
        {
            get{
                return values;
            }
            set{
                values = value;
            }
        }

        public Layer(int input, int output)
        {
            this.inputVal = input;
            this.outputVal = output;
            this.values = new float[output, input];
        }
    }
}
