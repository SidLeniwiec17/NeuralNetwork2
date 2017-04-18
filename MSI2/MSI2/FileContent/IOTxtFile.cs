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
                catch(IOException ex)
                {
                    MessageBox.Show("Error ! "+ ex.Message);
                }
            }
            else MessageBox.Show("File not selected !");
            return network;
        }

        public static bool ParseText (List<string> text, Network network)
        {
            bool correct = true;

            return correct;
        }
    }
}
