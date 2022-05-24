﻿using System.IO;
using System.Windows;
using BTSPEngine;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GUI;

public partial class GeneratorDialog : Window
{
	private readonly CancellationTokenSource cancelTokenSource = new();

	public int VertexCount { get; set; } = 10;
	public int MaxEdgeWeight { get; set; } = 100;
	public int InstanceCount { get; set; } = 1;
	public bool GenerateTxt { get; set; } = true;

	public GeneratorDialog()
	{
		InitializeComponent();
		DataContext = this;
	}

	private async void GenerateButton_OnClick(object sender, RoutedEventArgs e)
	{
		IGraphSerializer graphSerializer = GenerateTxt ? new TxtMatrixGraphSerializer() : new GraphMLSerializer();
		var generator = new BTSPGenerator(MaxEdgeWeight);

		const string dirName = "generated-examples";
		Directory.CreateDirectory(dirName);

		int i0 = 0;
		while (IsFileNameTaken(dirName, i0))
			i0++;

		generateButton.IsEnabled = false;
		statusText.Text = "Trwa generowanie...";

		await Task.Run(() =>
		{
			for (int i = 0; i < InstanceCount; i++)
			{
				generator.GenerateToFile(
					VertexCount, MakeFileName(dirName, VertexCount, MaxEdgeWeight, i0 + i), graphSerializer
				);
			}
		}, cancelTokenSource.Token);

		statusText.Text = "";
		generateButton.IsEnabled = true;
		MessageBox.Show("Pomyślnie wygenerowano przykłady");
	}

	private string MakeFileName(string dir, int n, double wMax, int i) =>
		i == 0 ? $"{dir}/btsp-n{n}-w{wMax}" : $"{dir}/btsp-n{n}-w{wMax}-{i}";

	private bool IsFileNameTaken(string dir, int i)
	{
		return File.Exists($"{MakeFileName(dir, VertexCount, MaxEdgeWeight, i)}.graphml") ||
		       File.Exists($"{MakeFileName(dir, VertexCount, MaxEdgeWeight, i)}.txt");
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		cancelTokenSource.Cancel();
		base.OnClosing(e);
	}

	private static bool IsTextAllowed(string text)
	{
		return !Regex.IsMatch(text, "[^0-9.-]+", RegexOptions.Compiled);
	}

	private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
	{
		e.Handled = !IsTextAllowed(e.Text);
	}
}