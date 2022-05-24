using BTSPEngine;
using QuikGraph;
using QuikGraph.Algorithms;
using System.Linq;

namespace Tests;

public static class Utilities
{
	public static bool IsCycle<T>(T cycle) where T : IEdgeSet<int, WeightedEdge>, IVertexSet<int>
	{
		var edges = cycle.Edges.ToList();
		var currentVertex = edges.First().Target;

		while (edges.Count > 0)
		{
			bool foundMatch = false;
			foreach (var edge in edges.Skip(edges.Count == 1 ? 0 : 1))
			{
				if (currentVertex == edge.Source)
				{
					edges.Remove(edge);
					currentVertex = edge.Target;
					foundMatch = true;
					break;
				}
				else if (currentVertex == edge.Target)
				{
					edges.Remove(edge);
					currentVertex = edge.Source;
					foundMatch = true;
					break;
				}
			}
			if (!foundMatch)
				return false;
		}

		return true;
	}

	public static bool IsDistAtMostThree<TCycle, TTree>(TCycle hamiltonCycle, TTree tree)
		where TCycle : IEdgeSet<int, WeightedEdge>
		where TTree : IUndirectedGraph<int, WeightedEdge>
	{
		foreach (var edge in hamiltonCycle.Edges)
		{
			if (!tree.ContainsEdge(edge.Source, edge.Target) && !tree.ContainsEdge(edge.Target, edge.Source))
			{
				var pathFunc = tree.ShortestPathsDijkstra(e => 1, edge.Source);
				if (pathFunc(edge.Target, out var result))
				{
					if (result.Count() > 3)
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
		}

		return true;
	}
}
