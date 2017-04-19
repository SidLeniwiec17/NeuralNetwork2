using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI2.Content
{
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
    }
}
