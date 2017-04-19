using MSI2.Content;
using MSI2.FileContent;
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
        public MainWindow()
        {
            InitializeComponent();
            globalNetwork = new Network();
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
    }
}
