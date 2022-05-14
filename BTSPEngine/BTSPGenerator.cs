using QuikGraph;
using QuikGraph.Serialization;
using System.Xml;

namespace BTSPEngine;

public class BTSPGenerator
{
	public BidirectionalMatrixGraph<WeightedEdge> Generate(int size, int? seed = null)
	{
		Random rng = seed switch
		{
			null => new Random(),
			not null => new Random(seed.Value),
		};

		var vertexMap = new (double x, double y)[size];
		for (int i = 0; i < size; i++)
		{
			vertexMap[i].x = rng.NextDouble() * 100;
			vertexMap[i].y = rng.NextDouble() * 100;
		}

		var graph = new BidirectionalMatrixGraph<WeightedEdge>(size);
		for (int u = 0; u < size; u++)
		{
			for (int v = u + 1; v < size; v++)
			{
				var weight = Distance(vertexMap[u], vertexMap[v]);
				graph.AddEdge(new WeightedEdge(u, v, weight));
				graph.AddEdge(new WeightedEdge(v, u, weight));
			}
		}

		return graph;
	}

	public void GenerateToFile(int size, string name)
	{
		var graph = Generate(size);

		var xmlSettings = new XmlWriterSettings
		{
			Indent = true,
			IndentChars = "\t"
		};

		using (var xmlWriter = XmlWriter.Create($"{name}.graphml", xmlSettings))
		{
			graph.SerializeToGraphML<int, WeightedEdge, BidirectionalMatrixGraph<WeightedEdge>>(xmlWriter);
		}
	}

	private double Distance((double x, double y) u, (double x, double y) v)
	{
		var dx = v.x - u.x;
		var dy = v.y - u.y;
		return Math.Sqrt(dx * dx + dy * dy);
	}
}