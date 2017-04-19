using Microsoft.Win32;
using MSI2.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MSI2.FileContent
{
    public class IOTxtFile
    {
        public static Network LoadNetworkConfiguration()
        {
            Network network = new Network();
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Open Network Config.";
            open.Filter = "Txt File (*.txt)|*.txt";
            
            if (open.ShowDialog() == true)
            {
                try
                {
                    List<string> lines = new List<string>();
                    System.IO.StreamReader file =
                        new System.IO.StreamReader(open.FileName);

                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        lines.Add(line.ToLower().Replace(" ",string.Empty));
                    }
                    file.Close();
                    if(!ParseText(lines, network))
                    {
                        MessageBox.Show("File has wrong format !");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error ! "+ ex.Message);
                }
            }
            else MessageBox.Show("File not selected !");

            return NetworkHelper.GenerateNetwork(network);
        }

        public static bool ParseText (List<string> text, Network network)
        {
            bool correct = true;
            
            Tuple<int,bool> tempTuple = TakeNumberFromBrackets(text[0]);            
            if (!text[0].Contains("input") || tempTuple.Item2 == false)
            {
                return false;
            }
            network.InputLength = tempTuple.Item1;

            tempTuple = TakeNumberFromBrackets(text[1]);
            if (!text[1].Contains("output") || tempTuple.Item2 == false)
            {
                return false;
            }
            network.Classes = tempTuple.Item1;

            tempTuple = TakeNumberFromBrackets(text[2]);
            if (!text[2].Contains("hidden") || tempTuple.Item2 == false)
            {
                return false;
            }
            network.HiddenLayers = tempTuple.Item1;

            Tuple<int[],bool> tempTableTuple = TakeTableFromBrackets(text[3], network.HiddenLayers);
            if (!text[3].Contains("neurons") || tempTableTuple.Item2 == false)
            {
                return false;
            }
            network.NeuronPerLayer = tempTableTuple.Item1;

            Tuple <bool,bool> tempBoolTuple = TakeBoolFromBrackets(text[4]);
            if (!text[4].Contains("bias") || tempBoolTuple.Item2 == false)
            {
                return false;
            }
            network.Bias = tempBoolTuple.Item1;

            tempTuple = TakeNumberFromBrackets(text[5]);
            if (!text[5].Contains("seed") || tempTuple.Item2 == false)
            {
                return false;
            }
            network.Seed = tempTuple.Item1;

            tempTuple = TakeNumberFromBrackets(text[6]);
            if (!text[6].Contains("iter") || tempTuple.Item2 == false)
            {
                return false;
            }
            network.Iterations = tempTuple.Item1;
            network.CompleteData = true;

            return correct;
        }
        public static Tuple<int,bool> TakeNumberFromBrackets(string text)
        {
            int number = 0;
            bool correct = true;

            int ind1 = text.IndexOf("<");
            int ind2 = text.IndexOf(">");

            string substring = text.Substring(ind1 + 1, ind2 - ind1 - 1);
            correct = int.TryParse(substring, out number);
            if (number < 1)
                correct = false;

            return new Tuple<int, bool>(number, correct);
        }
        public static Tuple<bool, bool> TakeBoolFromBrackets(string text)
        {
            bool value = false;
            bool correct = false;

            int ind1 = text.IndexOf("<");
            int ind2 = text.IndexOf(">");

            string substring = text.Substring(ind1 + 1, ind2 - ind1 - 1);
            if (substring == "y" || substring == "yes")
            {
                value = true;
                correct = true;
            }
            if (substring == "n" || substring == "no")
            {
                value = false;
                correct = true;
            }
            return new Tuple<bool, bool>(value, correct);
        }
        public static Tuple<int[], bool> TakeTableFromBrackets(string text, int length)
        {
            int[] values = new int[length];
            bool correct = true;

            int ind1 = text.IndexOf("<");
            int ind2 = text.IndexOf(">");

            string substring = text.Substring(ind1 + 1, ind2 - ind1 - 1);
            string[] strings = substring.Split(',');
            if (strings.Length != length)
                correct = false;

            if(correct == true)
            {
                for(int i = 0 ; i < length ; i ++)
                {
                    if(!int.TryParse(strings[i], out values[i]))
                    {
                        correct = false;
                        break;
                    }
                    else if(values[i] < 1)
                    {
                        correct = false;
                        break;
                    }
                }
            }

            return new Tuple<int[], bool>(values, correct);
        }
    }
}
