using BTSPEngine;

namespace Sandbox;

static class Program
{
	public static void Main()
	{
		var generator = new BTSPGenerator();
		generator.GenerateToFile(5, "btsp01");
		generator.GenerateToFile(20, "btsp02");
	}
}