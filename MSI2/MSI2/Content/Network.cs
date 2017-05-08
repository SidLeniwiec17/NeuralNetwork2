using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI2.Content
{
    [Serializable]
    public class Network
    {
        public int InputLength { get; set; }
        public int Classes { get; set; }
        public int HiddenLayers { get; set; }
        public int[] NeuronPerLayer { get; set; }
        public bool Bias { get; set; }
        public int Seed { get; set; }
        public int Iterations { get; set; }
        public Layer[] Layers { get; set; }
        public bool CompleteData { get; set; }
        public float LearningFactor { get; set; }

        public Network()
        {

        }
        public Network(int inputLength, int classes, int hiddenLayers, int[] neuronPerLayer, bool bias, int seed, int iterations, Layer[] layers)
        {
            InputLength = inputLength;
            Classes = classes;
            HiddenLayers = hiddenLayers;
            NeuronPerLayer = neuronPerLayer;
            Bias = bias;
            Seed = seed;
            Iterations = iterations;
            Layers = layers;
        }
        public string ToString()
        {
            string content = "Input vector length: <" + InputLength + ">" + Environment.NewLine;
            content += "Output classes: <" + Classes + ">" + Environment.NewLine;
            content += "Hidden layers: <" + HiddenLayers + ">" + Environment.NewLine;
            string hlpString = "";
            foreach (var i in NeuronPerLayer)
                hlpString += i.ToString() + ",";
            content += "Neurons in each layer: <" + hlpString + ">" + Environment.NewLine;
            content += "Bias: <" + Bias.ToString() +">" + Environment.NewLine;
            content += "Seed: <" + Seed + ">" + Environment.NewLine;
            content += "Iterations: <" + Iterations + ">" + Environment.NewLine;
            content += "Learning factor: <" + LearningFactor + ">" + Environment.NewLine;
            return content;
        }
    }
}
