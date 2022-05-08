using System.IO;
using System.Windows;
using BTSPEngine;

namespace GUI;

public partial class GeneratorDialog : Window
{
	public GeneratorDialog()
	{
		InitializeComponent();
	}

	private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
	{
		var vertexCount = int.Parse(VertexCountInput.Text);
		var maxEdgeWeight = double.Parse(MaxWeightInput.Text);
		var generator = new BTSPGenerator(maxEdgeWeight);
		var instanceCount = int.Parse(InstanceCountInput.Text);

		const string dirName = "generated-examples";
		Directory.CreateDirectory(dirName);

		int i0 = 0;
		while (File.Exists($"{MakeFileName(dirName, vertexCount, maxEdgeWeight, i0)}.graphml"))
			i0++;

		for (int i = 0; i < instanceCount; i++)
		{
			generator.GenerateToFile(vertexCount, MakeFileName(dirName, vertexCount, maxEdgeWeight, i0 + i));
		}

		MessageBox.Show("Pomyślnie wygenerowano przykłady");
	}

	private string MakeFileName(string dir, int n, double wMax, int i) =>
		i == 0 ? $"{dir}/btsp-n{n}-w{wMax}" : $"{dir}/btsp-n{n}-w{wMax}-{i}";
}