using QuikGraph;

namespace BTSPEngine;

using WeightedGraph = IVertexListGraph<int, WeightedEdge>;
using WeightedSparseGraph = UndirectedGraph<int, WeightedEdge>;

public class BTSPSolver
{
	public WeightedSparseGraph SolveBTSP(WeightedGraph btsp)
	{
		if (btsp.VertexCount < 3)
			throw new ArgumentException("BTSP graph must contain at least 3 vertices");
		
		var (bst, _) = btsp.PrimMST(e => e.Weight);
		var vertices = btsp.Vertices.ToArray();
		return FindHamilton(btsp, bst, vertices[0], vertices[1]);
	}

	private WeightedSparseGraph FindHamilton(WeightedGraph btsp, WeightedSparseGraph bst, int u, int v)
	{
		throw new NotImplementedException();
	}
}