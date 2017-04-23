using MSI2.Content;
using MSI2.FileContent;
using MSI2.Helpers;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            globalNetwork = new Network();
            learningSet = new List<Face>();
            unknownSet = new List<Face>();
            solution = new List<Face>();
            errorsHistory = new List<float>();
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
            //await ConvertUnKnownPic(tempUnknownPictures);
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
                await PerformLearning(data);
                LearningHelper.CreateErrorFile(errorsHistory);
                BlakWait.Visibility = Visibility.Collapsed;
            }
        }
        private async Task PerformLearning(DataSet data)
        {
            await Task.Run(() =>
            {
                List<float> errorHistory = new List<float>();
                Random rand = new Random(globalNetwork.Seed);
                for (int i = 0; i < globalNetwork.Iterations; i++)
                {
                    float error = 0.0f;
                    error = LearningHelper.Learn(globalNetwork, data);
                    LearningHelper.RandomizeSet(rand, data);
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("Epoka " + i + " / " + globalNetwork.Iterations);
                    Console.WriteLine("Error " + error + " %" );
                    errorHistory.Add(error);
                }
                errorsHistory = errorHistory;
            });
        }
    }
}
