using MSI2.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI2.Helpers
{
    public class LearningHelper
    {
        public static void Learn(Network network, DataSet data)
        {
            List<List<float>> placeForErrors = CreateNodesForErrors(network);
            for(int i = 0 ; i < data.Input.Count ; i++)
            {
                float[] solution = new float [network.Classes];
                solution = NetworkCalculation.CalculateSingleRecord(network, NetworkCalculation.VectorToFloat(data.Input[i]));
                float[] error = SquaredError(solution, data.Output[i].ToArray());
                CalculateNodeErrors(placeForErrors, network, error);

            }
        }
        public static float[] SquaredError(float[]networkAnswer, int[]idealAnswer)
        {
            float[] d = new float[networkAnswer.Length];
            for (int i = 0; i < d.Length; i++ )
            {
                d[i] = (float)(0.5 * Math.Pow((networkAnswer[i] - (float)idealAnswer[i]),2));
            }
            return d;
        }
        public static List<List<float>>CreateNodesForErrors(Network network)
        {
            List<List<float>> errors = new List<List<float>>();
            List<float> input = new List<float>();
            for (int j = 0; j < network.InputLength; j++)
                input.Add(0.0f);
            errors.Add(input);
            for (int i = 0; i < network.HiddenLayers; i++ )
            {
                List<float> innerList = new List<float>();
                for (int j = 0; j < network.NeuronPerLayer[i]; j++)
                    innerList.Add(0.0f);

                errors.Add(innerList);
            }
                return errors;
        }
        public static void CalculateNodeErrors(List<List<float>> placeForErrors, Network network, float[] endPointError)
        {
            for(int i = placeForErrors.Count - 1 ; i >= 0  ; i--)
            {
                if(i == placeForErrors.Count - 1)
                {
                    for(int j = 0 ; j < placeForErrors[i].Count ; j++)
                    {
                        placeForErrors[i][j] = 0.0f;
                        for(int q = 0 ; q < endPointError.Length ; q++)
                        {
                            placeForErrors[i][j] += endPointError[q] * network.Layers[network.Layers.Length - 1].Values[q,j]; 
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < placeForErrors[i].Count; j++)
                    {
                        placeForErrors[i][j] = 0.0f;
                        for (int q = 0; q < placeForErrors[i+1].Count; q++)
                        {
                            placeForErrors[i][j] += placeForErrors[i + 1][q] * network.Layers[i].Values[q, j];
                        }
                    }
                }
            }
        }
    }
}
