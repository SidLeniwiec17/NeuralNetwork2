using MSI2.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI2.Helpers
{
    public static class TestingHelper
    {
        public static float GetTestingSetError(Network network, DataSet testingSet)
        {
            float error = 0.0f;
            int correctAnswerCounter = 0;
            List<List<float>> placeForOutputs = LearningHelper.CreateNodesForErrors(network);
            for (int i = 0; i < testingSet.Input.Count; i++)
            {
                float[] solution = new float[network.Classes];
                solution = NetworkCalculation.CalculateSingleRecord(network, testingSet.Input[i].ToArray(), placeForOutputs);
                bool areSame = CompareAnswers(solution,testingSet.Output[i].ToArray());
                if (areSame)
                    correctAnswerCounter++;
            }
            error = (float)(((double)correctAnswerCounter * 100.0) / (double)testingSet.Output.Count);
            return error;
        }
        public static bool CompareAnswers(float[] networkAnswer, float[] idealAnswer)
        {
            bool correct = false;
            int networkIndex = GetClassIndex(networkAnswer);
            int idealInsex = GetClassIndex(idealAnswer);

            if (networkIndex == idealInsex)
                correct = true;
            return correct;
        }
        public static int GetClassIndex (float[] classes)
        {
            int index = -1;
            float maxVal = -1.0f;
            for(int i = 0 ; i < classes.Length ; i++)
            {
                if(classes[i]>=maxVal)
                {
                    maxVal = classes[i];
                    index = i;
                }
            }
            return index;
        }
    }
}
