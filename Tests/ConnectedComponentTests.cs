using System.Collections.Generic;
using System.Linq;
using BTSPEngine;
using QuikGraph;
using Xunit;

namespace Tests;

public class ConnectedComponentTests
{
	[Fact]
	public void ReturnsAllGraphForConnectedGraph()
	{
		var graph = new UndirectedGraph<int, WeightedEdge>();
		graph.AddVerticesAndEdge(new WeightedEdge(0, 1, 1));
		graph.AddVerticesAndEdge(new WeightedEdge(1, 2, 1));
		graph.AddVerticesAndEdge(new WeightedEdge(1, 3, 1));
		graph.AddVerticesAndEdge(new WeightedEdge(1, 4, 1));
		graph.AddVerticesAndEdge(new WeightedEdge(2, 5, 1));
		graph.AddVerticesAndEdge(new WeightedEdge(4, 6, 1));
		graph.AddVerticesAndEdge(new WeightedEdge(4, 7, 1));

		var connComp = graph.ConnectedComponentContaining(3);
		Assert.Equal(graph.Vertices.ToHashSet(), connComp.Vertices.ToHashSet());
		Assert.Equal(graph.Edges.ToHashSet(), connComp.Edges.ToHashSet());
	}

	[Fact]
	public void ReturnsConnectedComponentForNotConnectedGraph()
	{
		var graph = new UndirectedGraph<int, WeightedEdge>();

		var compEdges = new List<WeightedEdge>
		{
			new WeightedEdge(0, 1, 1),
			new WeightedEdge(1, 2, 1),
			new WeightedEdge(1, 3, 1),
			new WeightedEdge(1, 4, 1),
			new WeightedEdge(2, 5, 1),
			new WeightedEdge(4, 6, 1),
			new WeightedEdge(4, 7, 1)
		};
		graph.AddVerticesAndEdgeRange(compEdges);

		graph.AddVerticesAndEdge(new WeightedEdge(10, 11, 1));
		graph.AddVerticesAndEdge(new WeightedEdge(11, 12, 1));
		graph.AddVerticesAndEdge(new WeightedEdge(12, 13, 1));

		var connComp = graph.ConnectedComponentContaining(3);
		Assert.Equal(8, connComp.VertexCount);
		Assert.Equal(compEdges.ToHashSet(), connComp.Edges.ToHashSet());
	}
}