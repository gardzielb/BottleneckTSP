using BTSPEngine;
using BTSPEngine.Serialization;
using CSharpFunctionalExtensions;
using Spectre.Console;

namespace BTSP.ConsoleUI;

class Program
{
	public async static Task Main(int? mode = null, int? vertexCount = null, int? maxEdgeWeight = null, int? instanceCount = null, string? filePath = null)
	{
		bool anyParameterEntered = mode.HasValue || vertexCount.HasValue || maxEdgeWeight.HasValue || instanceCount.HasValue || filePath is not null;
		do
		{
			if (!mode.HasValue || (mode != 1 && mode != 2))
			{
				mode = AnsiConsole.Prompt(
					new TextPrompt<int>("Chcesz wygenerować przykładowe instancje ([bold]1[/]) czy znaleźć rozwiązanie problemu komiwojażera krótkodystansowca ([bold]2[/])?")
					.AddChoices(new[] { 1, 2 })
					.HideChoices()
					.InvalidChoiceMessage("[red]Niepoprawny wybór, wpisz wartość jeszcze raz[/]")
				);
			}

			if (mode == 1)
			{
				if (!vertexCount.HasValue || vertexCount.Value < 1)
				{
					vertexCount = AnsiConsole.Prompt(
						new TextPrompt<int>("Liczba wierzchołków grafu:")
						.Validate(x => x < 1 ? ValidationResult.Error("[red]Graf musi mieć przynajmniej 1 wierzchołek[/]") : ValidationResult.Success())
						.ValidationErrorMessage("[red]Niepoprawna wartość[/]")
					);
				}

				if (!maxEdgeWeight.HasValue || maxEdgeWeight.Value <= 0)
				{
					maxEdgeWeight = AnsiConsole.Prompt(
						new TextPrompt<int>("Maksymalna waga krawędzi w grafie:")
						.Validate(x => x <= 0 ? ValidationResult.Error("[red]Wagi wierzchołków w grafie muszą być dodatnie[/]") : ValidationResult.Success())
						.ValidationErrorMessage("[red]Niepoprawna wartość[/]")
					);
				}

				if (!instanceCount.HasValue || instanceCount.Value < 1)
				{
					instanceCount = AnsiConsole.Prompt(
						new TextPrompt<int>("Liczba instancji:")
						.Validate(x => x < 1 ? ValidationResult.Error("[red]Wygenerowana musi zostać przynajmniej 1 instancja[/]") : ValidationResult.Success())
						.ValidationErrorMessage("[red]Niepoprawna wartość[/]")
					);
				}

				await Generate(vertexCount.Value, instanceCount.Value, maxEdgeWeight.Value);
			}
			else if (mode == 2)
			{
				if (filePath is null || !File.Exists(filePath))
				{
					filePath = AnsiConsole.Prompt(
						new TextPrompt<string>("Podaj ścieżkę do pliku wejściowego:")
						.Validate(str => File.Exists(str) ? ValidationResult.Success() : ValidationResult.Error("Podany plik nie istnieje"))
					);
				}

				var calculationResult = await Calculate(filePath);
				if (calculationResult.IsFailure)
				{
					AnsiConsole.Markup(calculationResult.Error);
				}
				else
				{
					AnsiConsole.Markup("Otrzymany wynik to: [bold]{0}[/]\nPlik wynikowy zapisano w: {1}\n", calculationResult.Value.maxEdgeWeight, calculationResult.Value.filePath);
				}
			}
			mode = null;
			vertexCount = null;
			maxEdgeWeight = null;
			instanceCount = null;
			filePath = null;
			AnsiConsole.MarkupLine("\n");
		} while (!anyParameterEntered);
	}

	private static string MakeFileName(string dir, int n, double wMax, int i) =>
	i == 0 ? $"{dir}/btsp-n{n}-w{wMax}" : $"{dir}/btsp-n{n}-w{wMax}-{i}";

	private static bool IsFileNameTaken(string dir, int i, int vertexCount, int maxEdgeWeight)
	{
		return File.Exists($"{MakeFileName(dir, vertexCount, maxEdgeWeight, i)}.graphml") ||
			   File.Exists($"{MakeFileName(dir, vertexCount, maxEdgeWeight, i)}.txt");
	}

	private static async Task Generate(int vertexCount, int instanceCount, int maxEdgeWeight)
	{
		var dirInfo = await AnsiConsole.Progress().StartAsync(async ctx =>
		{
			var task1 = ctx.AddTask("Generowanie przykładów");
			task1.MaxValue = instanceCount;

			IGraphSerializer graphSerializer = new TxtMatrixGraphSerializer();
			var generator = new BTSPGenerator(maxEdgeWeight);

			const string dirName = "generated-examples";
			var dirInfo = Directory.CreateDirectory(dirName);

			int i0 = 0;
			while (IsFileNameTaken(dirName, i0, vertexCount, maxEdgeWeight))
				i0++;

			while (!ctx.IsFinished)
			{
				await Task.Run(() => generator.GenerateToFile(
					vertexCount, MakeFileName(dirName, vertexCount, maxEdgeWeight, i0 + (int)task1.Value), graphSerializer
				));
				task1.Increment(1);
			}
			return dirInfo;
		});
		AnsiConsole.Markup("Pomyślnie wygenerowano przykłady w katalogu {0}\n", dirInfo?.FullName);
	}

	private static Task<Result<(double maxEdgeWeight, string filePath)>> Calculate(string filePath)
	{
		return AnsiConsole.Status()
			.Spinner(Spinner.Known.Dots)
			.StartAsync("Wczytywanie grafu...", async ctx =>
			{
				IGraphSerializer deserializer = new TxtMatrixGraphSerializer();

				try
				{
					var inputGraph = deserializer.Deserialize(filePath);

					ctx.Status("Wykonywanie obliczeń...");
					var solver = new BTSPSolver();
					var solution = await Task.Run(() => solver.SolveBTSP(inputGraph));

					ctx.Status("Zapisywanie wyniku...");
					var resultPath = Path.GetFullPath($"./{Path.GetFileNameWithoutExtension(filePath)}_result.txt");
					deserializer.Serialize(solution.Cycle, resultPath, solution.CycleMaxEdgeWeight);

					return (solution.CycleMaxEdgeWeight, resultPath);
				}
				catch (ArgumentException)
				{
					return Result.Failure<(double, string)>("Plik wejściowy ma niepoprawny format");
				}
			});
	}
}
