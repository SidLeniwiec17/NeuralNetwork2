using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using MSI2.Content;

namespace MSI2.FileContent
{
    public class FileLoader
    {
        //flag - true ; jezeli wgrywamy zbior uczacy
        //flag - false ; jezeli wgrywamy zbior testowy
        public static List<List<String>> GetImages(bool flag)
        {
            List<List<string>> imagesList = new List<List<string>>();
            int s = 4;
            if (flag == false)
            {
                s = 1;
            }
            for (int i = 0; i < s; i++)
            {
                imagesList.Add(new List<string>());
            }
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = folderBrowserDialog.SelectedPath;
                if (flag == true)
                {
                    List<string> paths = Directory.GetDirectories(path).ToList();
                    if (paths.Count() == 4)
                    {
                        for (int i = 0; i < paths.Count(); i++)
                        {
                            switch (paths[i].Substring(paths[i].Length - 2))
                            {
                                case "BK":
                                    foreach (var file in Directory.GetFiles(paths[i]))
                                    {
                                        imagesList[0].Add(file);
                                    }
                                    break;
                                case "BM":
                                    foreach (var file in Directory.GetFiles(paths[i]))
                                    {
                                        imagesList[1].Add(file);
                                    }
                                    break;
                                case "LK":
                                    foreach (var file in Directory.GetFiles(paths[i]))
                                    {
                                        imagesList[2].Add(file);
                                    }
                                    break;
                                case "LM":
                                    foreach (var file in Directory.GetFiles(paths[i]))
                                    {
                                        imagesList[3].Add(file);
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    List<string> images = new List<string>();
                    foreach (var file in Directory.GetFiles(path))
                    {
                        images.Add(file);
                    }
                    imagesList.Add(images);
                }
            }
            return imagesList;
        }

        public static string GetDirectory()
        {
            string path = "";
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = folderBrowserDialog.SelectedPath;
            }
            return path;
        }

        public static void CopyAnswers(List<Face> solution, string answerPath)
        {
            for (int i = 0; i < solution.Count(); i++)
            {
                string picDir = solution[i].Path;
                string finalDir = answerPath + "\\" + solution[i].Class + "\\" + Path.GetFileName(picDir);
                File.Copy(@picDir, @finalDir);
            }
        }

        public static int SaveBinary(List<Face> faces)
        {
            if (faces.Count < 1)
            {
                MessageBox.Show("Nie ma danych do zapisania");
                return -1;
            }

            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "ZdjeciaInput"; // Default file name
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
                bf.Serialize(fs, faces);
                BinaryWriter w = new BinaryWriter(fs);
                w.Close();
                fs.Close();
            }
            return 1;

        }
        public static List<Face> LoadBinary()
        {
            List<Face> faces = new List<Face>();
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Open File...";
            open.Filter = "Binary File (*.bin)|*.bin";
            if (open.ShowDialog() == true)
            {
                FileStream fs = new FileStream(open.FileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                BinaryReader br = new BinaryReader(fs);

                faces = (List<Face>)bf.Deserialize(fs);

                fs.Close();
                br.Close();
            }
            else MessageBox.Show("Nie wybrano pliku !");
            return faces;
        }
    }
}
