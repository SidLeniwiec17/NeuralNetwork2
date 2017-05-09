using MSI2.Content;
using MSI2.FileContent;
using MSI2.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MSI2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Network globalNetwork;
        List<Face> learningSet;
        List<Face> unknownSet;
        List<Face> solution;
        List<float> errorsHistory;
        string testDir;
        public MainWindow()
        {
            InitializeComponent();
            globalNetwork = new Network();
            learningSet = new List<Face>();
            unknownSet = new List<Face>();
            solution = new List<Face>();
            errorsHistory = new List<float>();
            testDir = "";
        }

        private void LoadNetwConf_Click(object sender, RoutedEventArgs e)
        {
            Network temporaryNetwork = IOTxtFile.LoadNetworkConfiguration();
            globalNetwork = temporaryNetwork;
            Console.WriteLine("testy");
        }
        private void SaveNetwork_Click(object sender, RoutedEventArgs e)
        {
            if (globalNetwork.CompleteData)
            {
                IOBinFile.SaveBinary(globalNetwork);
            }
            else
            {
                MessageBox.Show("Network save aborted !");
            }
            Console.WriteLine("testy");
        }
        private void LoadNetwork_Click(object sender, RoutedEventArgs e)
        {
            globalNetwork = IOBinFile.LoadBinary();
            Console.WriteLine("testy");
        }
        private async void LoadPictConf_Click(object sender, RoutedEventArgs e)
        {
            //Zbior uczacy
            BlakWait.Visibility = Visibility.Visible;
            List<List<string>> pictures = FileLoader.GetImages(true);
            await ConvertKnownPic(pictures);
            BlakWait.Visibility = Visibility.Collapsed;
        }
        private void SavePict_Click(object sender, RoutedEventArgs e)
        {
            //Zbior uczacy
            if (FileLoader.SaveBinary(learningSet) == 1)
                Console.WriteLine("zapisano do binarki");
        }
        private void LoadPict_Click(object sender, RoutedEventArgs e)
        {
            //Zbior uczacy
            learningSet.Clear();
            learningSet = FileLoader.LoadBinary();
            if (learningSet.Count >= 1)
            {
                Console.WriteLine("wczytano z binarki " + learningSet.Count + " danych");
            }
        }
        private async void LoadPictTestConf_Click(object sender, RoutedEventArgs e)
        {
            //Zbior testowy
            BlakWait.Visibility = Visibility.Visible;
            List<List<string>> tempUnknownPictures = FileLoader.GetImages(true);
            await ConvertUnKnownPic(tempUnknownPictures);
            BlakWait.Visibility = Visibility.Collapsed;
        }
        private void SavePictTest_Click(object sender, RoutedEventArgs e)
        {
            //Zbior testowy
            if (FileLoader.SaveBinary(unknownSet) == 1)
                Console.WriteLine("zapisano do binarki");
        }
        private void LoadPictTest_Click(object sender, RoutedEventArgs e)
        {
            //Zbior testowy        
            unknownSet.Clear();
            unknownSet = FileLoader.LoadBinary();
            if (unknownSet.Count >= 1)
            {
                Console.WriteLine("wczytano z binarki " + unknownSet.Count + " danych");
            }
        }
        private async Task ConvertKnownPic(List<List<string>> pictures)
        {
            await Task.Run(() =>
            {
                learningSet = FaceHelper.ConvertFaces(pictures, true);
            });
        }
        private async Task ConvertUnKnownPic(List<List<string>> tempUnknownPictures)
        {
            await Task.Run(() =>
            {
                unknownSet = FaceHelper.ConvertFaces(tempUnknownPictures, true);
            });
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (learningSet.Count > 0 && globalNetwork.CompleteData == true)
            {
                BlakWait.Visibility = Visibility.Visible;
                DataSet data = new DataSet(learningSet, globalNetwork.Classes);
                await PerformLearning(data, globalNetwork);
                LearningHelper.CreateErrorFile(errorsHistory);
                BlakWait.Visibility = Visibility.Collapsed;
            }
        }
        private async Task PerformLearning(DataSet data, Network network)
        {
            await Task.Run(() =>
            {
                List<float> errorHistory = new List<float>();
                Random rand = new Random(network.Seed);
                for (int i = 0; i < network.Iterations; i++)
                {
                    float error = 0.0f;
                    error = LearningHelper.Learn(network, data);
                    LearningHelper.RandomizeSet(rand, data);
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("Epoka " + i + " / " + network.Iterations);
                    Console.WriteLine("Error " + error + " %");
                    errorHistory.Add(error);
                }
                errorsHistory = errorHistory;
            });
        }

        //-------------------------------------------------------------------
        //--------------------------TESTING SECTION--------------------------
        //-------------------------------------------------------------------
        public void Logger(String lines, string dir)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(dir + "\\logs.txt", true);
            file.WriteLine(lines);
            file.Close();
        }
        private async Task PerformLearning(DataSet data, Network network, string dir, int index)
        {
            await Task.Run(() =>
            {
                List<float> errorHistory = new List<float>();
                Random rand = new Random(network.Seed);
                Logger(network.ToString(), dir);
                for (int i = 0; i < network.Iterations; i++)
                {
                    float error = 0.0f;
                    error = LearningHelper.Learn(network, data);
                    LearningHelper.RandomizeSet(rand, data);
                    string txt = "";
                    txt += "-----------------------------------------" + Environment.NewLine;
                    string timer = string.Format("{0:dd.MM.yyyy_hh:mm:ss}",
                    DateTime.Now);
                    txt += timer + Environment.NewLine;
                    txt += "Epoka " + i + " / " + network.Iterations + Environment.NewLine;
                    txt += "Mean Squared Error " + error + " %" + Environment.NewLine;
                    Logger(txt, dir);

                    errorHistory.Add(error);
                }
                errorsHistory = errorHistory;

                float testingError = TestingHelper.GetTestingSetError(network, new DataSet(unknownSet, network.Classes));
                string txt2 = "";
                txt2 += "-----------------------------------------" + Environment.NewLine;
                txt2 += "------------FINAL TESTING ERROR----------" + Environment.NewLine;
                txt2 += "-----------------------------------------" + Environment.NewLine;
                string timer2 = string.Format("{0:dd.MM.yyyy_hh:mm:ss}",
                DateTime.Now);
                txt2 += timer2 + Environment.NewLine;
                txt2 += "Testing Error " + testingError + "%" + Environment.NewLine;
                Logger(txt2, dir);

                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine(timer2 + ": test " + index + " przeprowadzono");

            });
        }
        private async void Button3_Click(object sender, RoutedEventArgs e)
        {
            if (unknownSet.Count > 0 && learningSet.Count > 0 && testDir.Length > 3)
            {
                try
                {
                    string networkTestAddres = testDir + "\\Networks";
                    List<string> networksPaths = Directory.GetFiles(networkTestAddres).ToList();
                    for (int i = 0; i < networksPaths.Count; i++)
                    {
                        string answersTestAddres = testDir + "\\Answers";
                        string newPath = System.IO.Path.Combine(answersTestAddres, i.ToString());
                        System.IO.Directory.CreateDirectory(newPath);
                        Network temporaryNetwork = IOTxtFile.LoadNetworkConfiguration(networksPaths[i]);
                        if (unknownSet.Count > 0 && learningSet.Count > 0 && temporaryNetwork.CompleteData == true)
                        {
                            BlakWait.Visibility = Visibility.Visible;
                            DataSet data = new DataSet(learningSet, temporaryNetwork.Classes);

                            await PerformLearning(data, temporaryNetwork, newPath, i);
                            //zapisac siec + wynik
                            IOBinFile.SaveBinary(temporaryNetwork, newPath + "\\learnedNetwork");
                            LearningHelper.CreateErrorFile(errorsHistory, newPath);
                            BlakWait.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Error ! Nie można przeprowadzić testów.");
                }
            }
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = folderBrowserDialog.SelectedPath;
                if (path != null)
                    testDir = path;
            }
        }
    }
}
