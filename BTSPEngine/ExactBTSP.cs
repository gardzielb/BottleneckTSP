using BTSPEngine.Extensions;
using QuikGraph;

namespace BTSPEngine
{
	public static class ExactBTSPSolver
	{
		public static (UndirectedGraph<int, WeightedEdge> cycle, double cost) ExactBTSP<TEdge>(this IVertexListGraph<int, TEdge> graph, Func<TEdge, double> edgeWeights) where TEdge : IEdge<int>
		{
			var list = new List<TEdge>();
			double lowestMaxEdgeCost = double.PositiveInfinity;
			foreach (var permutation in graph.Vertices.Permutations())
			{
				var currentCycle = new List<TEdge>(graph.VertexCount);
				double currentMaxEdgeCost = double.NegativeInfinity;
				for (int i = 0; i < graph.VertexCount; i++)
				{
					if (graph.TryGetEdge(permutation[i], permutation[(i + 1) % graph.VertexCount], out var edge))
					{
						currentCycle.Add(edge);
						if (edgeWeights(edge) > currentMaxEdgeCost)
						{
							currentMaxEdgeCost = edgeWeights(edge);
						}
					}
				}

				if (currentMaxEdgeCost < lowestMaxEdgeCost)
				{
					lowestMaxEdgeCost = currentMaxEdgeCost;
					list = currentCycle;
				}
			}
			var g = new UndirectedGraph<int, WeightedEdge>(false);
			foreach (var e in list)
			{
				g.AddVerticesAndEdge(new WeightedEdge(e.Source, e.Target, edgeWeights(e)));
			}
			return (g, lowestMaxEdgeCost);
		}
	}
}
