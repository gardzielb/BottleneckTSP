using QuikGraph;
using System.Text.RegularExpressions;

namespace BTSPEngine.Serialization
{
	public class TxtMatrixGraphSerializer : IGraphSerializer
	{
		public void Serialize(BidirectionalMatrixGraph<WeightedEdge> graph, string path)
		{
			using (var writer = new StreamWriter(new FileStream($"{path}.txt", FileMode.Create)))
			{
				writer.WriteLine(graph.VertexCount);
				foreach (int u in graph.Vertices)
				{
					foreach (int v in graph.Vertices)
					{
						bool isEdge = graph.TryGetEdge(u, v, out WeightedEdge uv);
						var edgeStr = $"{(isEdge ? uv.Weight : 0),20} ";
						writer.Write(edgeStr);
					}

					writer.Write('\n');
				}
			}
		}

		public void Serialize(UndirectedGraph<int, WeightedEdge> cycle, string path, double maxCost)
		{
			string pathWithExtension = Path.HasExtension(path) ? path : $"{path}.txt";
			using var writer = new StreamWriter(new FileStream(pathWithExtension, FileMode.Create));
			writer.WriteLine(maxCost);

			var edges = cycle.Edges.ToList();
			var currentVertex = edges.First().Target;
			writer.Write($"{currentVertex} ");
			while (edges.Count > 0)
			{
				bool foundMatch = false;
				foreach (var edge in edges.Skip(edges.Count == 1 ? 0 : 1))
				{
					if (currentVertex == edge.Source)
					{
						edges.Remove(edge);
						currentVertex = edge.Target;
						writer.Write($"{currentVertex} ");
						foundMatch = true;
						break;
					}
					else if (currentVertex == edge.Target)
					{
						edges.Remove(edge);
						currentVertex = edge.Source;
						writer.Write($"{currentVertex} ");
						foundMatch = true;
						break;
					}
				}
				if (!foundMatch)
					throw new ArgumentException("Not a cycle");
			}
			writer.Write('\n');
		}

		public BidirectionalMatrixGraph<WeightedEdge> Deserialize(string path)
		{
			using (var reader = new StreamReader(new FileStream(path, FileMode.Open)))
			{
				var vertexCount = int.Parse(reader.ReadLine());
				var graph = new BidirectionalMatrixGraph<WeightedEdge>(vertexCount);

				for (int u = 0; u < vertexCount; u++)
				{
					var row = Regex.Split(reader.ReadLine().Trim(), "[\t\\s]+");

					for (int v = 0; v < u; v++)
					{
						var weight = double.Parse(row[v]);
						if (!(weight > 0))
							continue;

						graph.AddEdge(new WeightedEdge(u, v, weight));
						graph.AddEdge(new WeightedEdge(v, u, weight));
					}
				}

				return graph;
			}
		}
	}
}
