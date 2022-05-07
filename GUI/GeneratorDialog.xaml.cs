using System.Diagnostics;
using System.Windows;

namespace GUI;

public partial class GeneratorDialog : Window
{
	public GeneratorDialog()
	{
		InitializeComponent();
	}

	private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
	{
		Trace.WriteLine("Generating");
	}
}