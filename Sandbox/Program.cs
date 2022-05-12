using BTSPEngine;

namespace Sandbox;

static class Program
{
	public static void Main()
	{
		var generator = new BTSPGenerator();
		var btsp = generator.Generate(10);
		var btspSolver = new BTSPSolver();
		var hamCycle = btspSolver.SolveBTSP(btsp);
		Console.WriteLine("dupa");
	}
}