using BTSPEngine.Serialization;
using QuikGraph;

namespace BTSPEngine;

public class BTSPGenerator
{
	private readonly double maxWeight;

	public BTSPGenerator(double maxWeight = 100)
	{
		this.maxWeight = maxWeight;
	}

	public BidirectionalMatrixGraph<WeightedEdge> Generate(int size, int? seed = null)
	{
		Random rng = seed switch
		{
			null => new Random(),
			not null => new Random(seed.Value),
		};

		var vertexMap = new (double x, double y)[size];
		var randScale = maxWeight / Math.Sqrt(2);

		for (int i = 0; i < size; i++)
		{
			vertexMap[i].x = rng.NextDouble() * randScale;
			vertexMap[i].y = rng.NextDouble() * randScale;
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

	public void GenerateToFile(int size, string name, IGraphSerializer graphSerializer)
	{
		graphSerializer.Serialize(Generate(size), name);
	}

	private double Distance((double x, double y) u, (double x, double y) v)
	{
		var dx = v.x - u.x;
		var dy = v.y - u.y;
		return Math.Sqrt(dx * dx + dy * dy);
	}
}