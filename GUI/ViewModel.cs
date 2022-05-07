using BTSPEngine;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using QuikGraph;
using QuikGraph.Serialization;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace GUI
{
    public class ViewModel : ObservableRecipient
    {
        /// <summary>
        /// Creates a new <see cref="SubredditWidgetViewModel"/> instance.
        /// </summary>
        public ViewModel()
        {
            GenerateCommand = new RelayCommand(GenerateInstances);
            CalculateCommand = new AsyncRelayCommand(Calculate);
            OpenFileCommand = new RelayCommand(ReadFile);
            OpenExplorerCommand = new RelayCommand(OpenExplorer);
            Directory.CreateDirectory(outputPath);
        }

        public IRelayCommand GenerateCommand { get; }
        public IRelayCommand OpenFileCommand { get; }
        public IRelayCommand OpenExplorerCommand { get; }
        public IAsyncRelayCommand CalculateCommand { get; }

        private string inputGraphFileName;
        private readonly string outputPath = "../Results";

        private bool isProgressBarVisible = false;

        public bool IsProgressBarVisible
        {
            get => isProgressBarVisible;
            set => SetProperty(ref isProgressBarVisible, value);
        }

        private bool isCalculationAvailable;

        public bool IsCalculationAvailable
        {
            get => isCalculationAvailable;
            set => SetProperty(ref isCalculationAvailable, value);
        }

        private string selectedStrategy;
        public string SelectedStrategy
        {
            get => selectedStrategy;
            set => SetProperty(ref selectedStrategy, value);
        }

        private string resultText = "";
        public string ResultText
        {
            get => resultText;
            set => SetProperty(ref resultText, value);
        }

        private string fileStatusText = "Nie wczytano pliku";

        public string FileStatusText
        {
            get => fileStatusText;
            set => SetProperty(ref fileStatusText, value);
        }

        private BidirectionalMatrixGraph<WeightedEdge> inputGraph { get; set; }

        private void ReadFile()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var deserializer = new GraphMLDeserializer();
                try
                {
                    inputGraph = deserializer.Deserialize(openFileDialog.FileName);
                    FileStatusText = $"Poprawnie wczytano plik {Path.GetFileName(openFileDialog.FileName)}";
                    inputGraphFileName = openFileDialog.FileName;
                    IsCalculationAvailable = true;
                }
                catch (ArgumentException)
                {
                    FileStatusText = $"Plik {Path.GetFileName(openFileDialog.FileName)} ma niepoprawny format";
                }
            }
        }

        private async Task Calculate()
        {
            IsProgressBarVisible = true;
            var (graph, weight) = await Task.Run(() => inputGraph.PrimMST(e => e.Weight));

            var path = SerializeToFile(graph, weight);
            ResultText = $"Wynik wynosi {weight}. Wynik zapisano w pliku {path}";
            IsProgressBarVisible = false;
        }

        private string SerializeToFile(UndirectedGraph<int, WeightedEdge> graph, double weight)
        {
            var xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };
            var filename = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(inputGraphFileName));
            var fullPath = Path.Combine(outputPath, $"{filename}_result_{weight:0.##}.graphml");
            using var xmlWriter = XmlWriter.Create(fullPath, xmlSettings);
            graph.SerializeToGraphML<int, WeightedEdge, UndirectedGraph<int, WeightedEdge>>(xmlWriter);            
            return Path.GetFileName(fullPath);
        }

        public void OpenExplorer()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = Path.GetFullPath(outputPath);
                process.Start();
            }
            catch (Win32Exception)
            {
                Console.WriteLine("Nie udało się otworzyć folderu");
            }
        }

        private void GenerateInstances()
        {
            var generateDialog = new GeneratorDialog();
            generateDialog.Show();
        }
    }
}
