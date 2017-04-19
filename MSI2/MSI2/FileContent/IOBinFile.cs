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
    public class IOBinFile
    {
        public static int SaveBinary(Network network)
        {
            if (network.Layers.Length <1 || network.CompleteData == false)
            {
                MessageBox.Show("Nie ma danych do zapisania");
                return -1;
            }

            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Network"; // Default file name
            save.DefaultExt = ".bin"; // Default file extension
            save.Title = "Save As...";
            save.Filter = "Binary File (*.bin)|*.bin";
            save.RestoreDirectory = true;
            save.InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;

            Nullable<bool> result = save.ShowDialog();
            if (result == true)
            {
                string filename = save.FileName;
                FileStream fs = new FileStream(filename, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, network);
                BinaryWriter w = new BinaryWriter(fs);
                w.Close();
                fs.Close();
            }
            return 1;
        }

        public static Network LoadBinary()
        {
            Network network = new Network();
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Open File...";
            open.Filter = "Binary File (*.bin)|*.bin";
            if (open.ShowDialog() == true)
            {
                try
                {
                    FileStream fs = new FileStream(open.FileName, FileMode.Open);
                    BinaryFormatter bf = new BinaryFormatter();
                    BinaryReader br = new BinaryReader(fs);

                    network = (Network)bf.Deserialize(fs);

                    fs.Close();
                    br.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Load file error: "+ ex.Message);
                }
            }
            else MessageBox.Show("No selected file !");

            if(network.CompleteData == false)
            {
                network = new Network();
                MessageBox.Show("Loaded network incomplete !");
            }
            return network;
        }
    }
}
