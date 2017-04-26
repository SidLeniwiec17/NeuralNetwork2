using MSI2.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI2.Helpers
{
    public class NetworkCalculation
    {
        //4. Zbior testowy + liczenie bledu

        public static float[] CalculateSingleRecord (Network network, float[] inputVector, List<List<float>> placeForOutputs)
        {
            float[] answer = new float[network.Classes];
            float[] previousNodesValues = new float[inputVector.Length];
            placeForOutputs[0] = inputVector.ToList();
            if (network.Bias)
                placeForOutputs[0].Add(1.0f);
            previousNodesValues = inputVector;
            float[] tempNodesValues = new float[network.NeuronPerLayer[0]];
            int outputNeurons = network.NeuronPerLayer[0];

            for (int i = 0; i < network.HiddenLayers + 1; i++)
            {
                if (i > 0)
                {
                    previousNodesValues = new float[tempNodesValues.Length];
                    previousNodesValues = tempNodesValues;
                    if (i < network.HiddenLayers)
                    {
                        tempNodesValues = new float[network.NeuronPerLayer[i]];
                        outputNeurons = network.NeuronPerLayer[i];
                    }
                    else
                    {
                        tempNodesValues = new float[network.Classes];
                        outputNeurons = network.Classes;
                    }
                }

                for (int j = 0; j < outputNeurons; j++)
                {
                    tempNodesValues[j] = 0.0f;
                    for(int x = 0 ; x < previousNodesValues.Length ; x++)
                    {
                         tempNodesValues[j] += (previousNodesValues[x] * network.Layers[i].Values[j,x]);
                    }
                    if(network.Bias)
                        tempNodesValues[j] += (1.0f * network.Layers[i].Values[j, previousNodesValues.Length]);


                    tempNodesValues[j] = ActivationFunction(tempNodesValues[j]);
                    if (i + 1 < placeForOutputs.Count)
                        placeForOutputs[i + 1][j] = tempNodesValues[j];
                    else
                        placeForOutputs[i][j] = tempNodesValues[j];

                }
            }
            answer = tempNodesValues;
            return answer;
        }
        public static float[] VectorToFloat(List<int> inputVector)
        {
            float[] newVector = new float[inputVector.Count];
            int counter = 0;
            foreach (var v in inputVector)
            {
                newVector[counter] = (float)v;
                counter++;
            }
            return newVector;
        }
        public static float ActivationFunction(float input)
        {
            float output = 0.0f;
            output = (float)Sigmoid(input);
            return output;
        }
        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }
        public static double SigmoidDerivative(double x)
        {
            double s = Sigmoid(x);
            return s * (1 - s);
        }
    }
}
