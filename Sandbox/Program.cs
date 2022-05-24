using BTSPEngine;
using BTSPEngine.Serialization;

namespace Sandbox;

static class Program
{
	public static void Main()
	{
		var batchTester = new BatchTester(new BTSPGenerator());
		batchTester.RunTests(fromSize: 5, toSize: 100, sizeStep: 5, samplesPerSize: 3, reportPath: "report01.csv");

		var x = 1.23456789;
		var y = 0;
		Console.WriteLine($"{x,10}");
		Console.WriteLine($"{y,10}");

		var serializer = new TxtMatrixGraphSerializer();
		var generator = new BTSPGenerator();

		var g1 = generator.Generate(10);
		serializer.Serialize(g1, "g1");
		var g2 = serializer.Deserialize("g1.txt");

		if (g1.VertexCount != g2.VertexCount)
		{
			Console.WriteLine("Wrong vertex count!");
			return;
		}

		if (g1.EdgeCount != g2.EdgeCount)
		{
			Console.WriteLine("Wrong edge count");
			return;
		}

		for (int u = 0; u < g1.VertexCount; u++)
		{
			for (int v = 0; v < g1.VertexCount; v++)
			{
				if (u == v)
					continue;

				g1.TryGetEdge(u, v, out WeightedEdge uv1);
				g2.TryGetEdge(u, v, out WeightedEdge uv2);

				if (uv1.Weight != uv2.Weight)
				{
					Console.WriteLine($"({u}, {v}, {uv1.Weight}) != ({u}, {v}, {uv2.Weight})");
					return;
				}
			}
		}

		// var btsp = generator.Generate(10);
		// var btspSolver = new BTSPSolver();
		// var hamCycle = btspSolver.SolveBTSP(btsp);
		// Console.WriteLine("dupa");
	}
}