using BTSPEngine;

namespace Sandbox;

static class Program
{
	public static void Main()
	{
		var generator = new BTSPGenerator();
		generator.GenerateToFile(4, "btsp01.xml");
	}
}