using BTSPEngine;
using QuickGraph;
using QuickGraph.Serialization;

namespace Sandbox;

static class Program
{
	public static void Main()
	{
		var generator = new BTSPGenerator();
		generator.GenerateToFile(4, "btsp01");

		var deserializer = new GraphMLDeserializer();
		var graph = deserializer.Deserialize("btsp01");
		Console.WriteLine(graph.Vertices);
	}
}