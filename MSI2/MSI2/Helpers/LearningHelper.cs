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
        /// DO POPRAWY
        public static float Learn(Network network, DataSet data)
        {
            float meanError = 0.0f;
            List<List<float>> placeForErrors = CreateNodesForErrors(network);
            List<List<float>> placeForOutputs = CreateNodesForErrors(network);
            for(int i = 0 ; i < data.Input.Count ; i++)
            {
                float[] solution = new float [network.Classes];
                solution = NetworkCalculation.CalculateSingleRecord(network, data.Input[i].ToArray(), placeForOutputs);
                float[] MSError = SquaredError(solution, data.Output[i].ToArray());
                float[] lastLayerError = LastLayerError(solution,data.Output[i].ToArray());
                float tmpError = 0.0f;
                for (int j = 0; j < MSError.Length; j++)
                    tmpError += MSError[j];

                CalculateNodeErrors(placeForErrors, lastLayerError, network);
                ModifyWages(network, placeForErrors, placeForOutputs, network.LearningFactor);
                meanError += tmpError;
            }
            meanError = meanError / (float)data.Input.Count;
            return meanError;
        }
        
        public static void ModifyWages(Network network, List<List<float>> placeForErrors, List<List<float>> placeForValues, float learningFactor)
        {
            //for(int i = 0 ; i < network.Layers.Length; i++)
            Parallel.For(0, network.Layers.Length, i =>
            {
                for (int j = 0; j < network.Layers[i].InputNeurons; j++)
                {
                    for (int q = 0; q < network.Layers[i].OutputNeurons; q++)
                    {
                        float newWage = 0.0f;
                        newWage = ModifySingleWage(network.Layers[i].Values[q, j], placeForErrors[i+1][q], placeForValues[i][j], learningFactor);
                        network.Layers[i].Values[q, j] = newWage;
                    }
                }
            });
        }

        public static float[] LastLayerError(float[] networkAnswer, float[] idealAnswer)
        {
            float[] d = new float[networkAnswer.Length];
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = - (float)(idealAnswer[i] - networkAnswer[i]) * (networkAnswer[i] * (1.0f - networkAnswer[i]));
            }
            return d;
        }
        public static float ModifySingleWage(float oldWage, float error, float Output, float learningFactor)
        {
            float difference = error * Output * learningFactor;
            return oldWage - difference;
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
            List<float> lastList = new List<float> ();
            for (int i = 0; i < network.Classes; i++)
                lastList.Add(0.0f);
            errors.Add(lastList);
            return errors;
        }
        public static void CalculateNodeErrors(List<List<float>> placeForErrors, float[] lastError, Network network)
        {
            placeForErrors[placeForErrors.Count - 1] = lastError.ToList();
            for (int i = placeForErrors.Count - 2; i >= 0; i--)
            {
                for (int y = 0; y < placeForErrors[i].Count; y++)
                {
                    placeForErrors[i][y] = 0.0f;
                    for (int q = 0; q < placeForErrors[i + 1].Count - 1; q++)
                    {
                        placeForErrors[i][y] += placeForErrors[i + 1][q] * network.Layers[i].Values[q,y];
                    }
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
        public static void CreateErrorFile(List<float> errors, string dir)
        {
            string line = "";
            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter(dir+"\\errors.R");

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
