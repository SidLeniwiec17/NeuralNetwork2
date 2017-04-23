using MSI2.Content;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI2.Helpers
{
    public class LearningHelper
    {
        public static float Learn(Network network, DataSet data)
        {
            float meanError = 0.0f;
            List<List<float>> placeForErrors = CreateNodesForErrors(network);
            List<List<float>> placeForValues = CreateNodesForErrors(network);
            for(int i = 0 ; i < data.Input.Count ; i++)
            {
                float[] solution = new float [network.Classes];
                solution = NetworkCalculation.CalculateSingleRecord(network, data.Input[i].ToArray(), placeForValues);
                float[] error = SquaredError(solution, data.Output[i].ToArray());
                float tmpError = 0.0f;
                for (int j = 0; j < error.Length; j++)
                    tmpError += error[j];

                CalculateNodeErrors(placeForErrors, network, error);
                ModifyWages(network, placeForErrors, placeForValues, error, data.Input[i].ToArray(), 0.01f);
                meanError += tmpError;
            }
            meanError = meanError / (float)data.Input.Count;
            return meanError;
            //Console.WriteLine("Zmodyfikowano wagi");
        }
        public static void ModifyWages(Network network, List<List<float>> placeForErrors, List<List<float>> placeForValues, float[] endError, float[] input, float learningFactor)
        {
            //for(int i = 0 ; i < network.Layers.Length; i++)
            Parallel.For(0, network.Layers.Length, i =>
            {
                for (int j = 0; j < network.Layers[i].InputNeurons; j++)
                {
                    for (int q = 0; q < network.Layers[i].OutputNeurons; q++)
                    {
                        //otrzymany input
                        float y = 1.0f;
                        if (j < placeForValues[i].Count)
                            y = placeForValues[i][j];
                        float newWage = 0.0f;
                        newWage = learningFactor * (float)NetworkCalculation.SigmoidDerivative((double)y) * placeForErrors[i][j];
                        network.Layers[i].Values[q, j] = network.Layers[i].Values[q, j] - newWage;
                    }
                }
            });
        }
        public static float[] SquaredError(float[]networkAnswer, float[]idealAnswer)
        {
            float[] d = new float[networkAnswer.Length];
            for (int i = 0; i < d.Length; i++ )
            {
                d[i] = (float)(0.5 * Math.Pow((networkAnswer[i] - idealAnswer[i]),2));
            }
            return d;
        }
        public static List<List<float>>CreateNodesForErrors(Network network)
        {
            List<List<float>> errors = new List<List<float>>();
            List<float> input = new List<float>();
            for (int j = 0; j < network.InputLength; j++)
                input.Add(0.0f);
            if(network.Bias)
                input.Add(1.0f);
            errors.Add(input);
            for (int i = 0; i < network.HiddenLayers; i++ )
            {
                List<float> innerList = new List<float>();
                for (int j = 0; j < network.NeuronPerLayer[i]; j++)
                    innerList.Add(0.0f);
                if (network.Bias)
                    innerList.Add(1.0f);
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
                    //for(int j = 0 ; j < placeForErrors[i].Count ; j++)
                    Parallel.For(0, placeForErrors[i].Count, j =>
                    {
                        placeForErrors[i][j] = 0.0f;
                        for (int q = 0; q < endPointError.Length; q++)
                        {
                            placeForErrors[i][j] += endPointError[q] * network.Layers[network.Layers.Length - 1].Values[q, j];
                        }
                    });
                }
                else
                {
                    //for (int j = 0; j < placeForErrors[i].Count; j++)
                    Parallel.For(0, placeForErrors[i].Count, j =>
                    {
                        placeForErrors[i][j] = 0.0f;
                        int nextLayerNodes = placeForErrors[i + 1].Count;
                        if (network.Bias)
                            nextLayerNodes--;
                        for (int q = 0; q < nextLayerNodes; q++)
                        {
                            placeForErrors[i][j] += placeForErrors[i + 1][q] * network.Layers[i].Values[q, j];
                        }
                    });
                }
            }
        }
        public static void RandomizeSet(Random rnd, DataSet data)
        {
            for(int i = 0 ; i < data.Input.Count ; i++)
            {
                int newPosition = NextPosition(rnd,data.Input.Count);
                List<float> tmpInput = data.Input[newPosition];
                List<float> tmpOutput = data.Output[newPosition];
                data.Input[newPosition] = data.Input[i];
                data.Output[newPosition] = data.Output[i];
                data.Input[i] = tmpInput;
                data.Output[i] = tmpOutput;
            }
        }
        public static int NextPosition(Random rnd, int length)
        {
            //float min = 0.0f;
            //float max = 1.0f;
            int min = 0;
            int max = length - 1;
            int newPosition = rnd.Next(min, max);
            return newPosition;
        }

        public static void CreateErrorFile(List<float> errors)
        {
            string line = "";
            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter("errors.R");

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            line = "points<- c(";
            int i = 0;
            for (i = 0; i < errors.Count - 1; i++)
            {
                line += errors[i].ToString(nfi) + ",";
            }
            line += errors[i].ToString(nfi) + ")";
            file.WriteLine(line);
            file.WriteLine(@"plot(points , type= ""o"", col= ""red"")");
            file.WriteLine(@"title(main= ""Error"", col.main= ""black"", font.main= 4)");

            file.Close();
        }
    }
}
