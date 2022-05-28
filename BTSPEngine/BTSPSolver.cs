using QuikGraph;

namespace BTSPEngine;

using WeightedGraph = IVertexListGraph<int, WeightedEdge>;
using WeightedSparseGraph = UndirectedGraph<int, WeightedEdge>;

public record BTSPSolution(
	WeightedSparseGraph Cycle,
	double CycleMaxEdgeWeight,
	double BSTMaxEdgeWeight
);

public class BTSPSolver
{
	public BTSPSolution SolveBTSP(WeightedGraph btsp)
	{
		if (btsp.VertexCount < 3)
			throw new ArgumentException("BTSP graph must contain at least 3 vertices");

		var (bst, _) = btsp.PrimMST(e => e.Weight);
		var vertices = btsp.Vertices.ToArray();

		var u = vertices[0];
		var uvEdge = bst.AdjacentEdge(u, 0);
		var v = uvEdge.GetOtherVertex(u);

		var hamCycle = FindHamilton(btsp, bst, u, v);
		hamCycle.AddEdge(uvEdge);

		var cycleMaxWeight = hamCycle.Edges.Max(e => e.Weight);
		var bstMaxWeigth = bst.Edges.Max(e => e.Weight);

		return new BTSPSolution(hamCycle, cycleMaxWeight, bstMaxWeigth);
	}

	/// <summary>
	/// Find a Hamiltionian path from u to v in a full graph with given BST.
	/// </summary>
	private WeightedSparseGraph FindHamilton(WeightedGraph btsp, WeightedSparseGraph bst, int u, int v)
	{
		if (bst.VertexCount == 3)
			return Make3(btsp, bst, u, v);

		var uvBst = bst.Clone();
		uvBst.RemoveAdjacentEdgeIf(u, e => e.GetOtherVertex(u) == v);

		var uBst = uvBst.ConnectedComponentContaining(u);
		var vBst = uvBst.ConnectedComponentContaining(v);

		if (uBst.VertexCount > vBst.VertexCount)
		{
			(uBst, vBst) = (vBst, uBst);
			(u, v) = (v, u);
		}

		var hamPath = new UndirectedGraph<int, WeightedEdge>();
		var vv1 = ProcessBstComponent(hamPath, btsp, vBst, v);
		var v1 = vv1.GetOtherVertex(v);

		if (uBst.VertexCount == 1)
		{
			if (!btsp.TryGetEdge(u, v1, out WeightedEdge uv1))
				throw new ArgumentException("BTSP graph must be full");

			hamPath.AddVerticesAndEdge(uv1);
			return hamPath;
		}

		var uu1 = ProcessBstComponent(hamPath, btsp, uBst, u);
		var u1 = uu1.GetOtherVertex(u);
		if (!btsp.TryGetEdge(u1, v1, out WeightedEdge u1v1))
			throw new ArgumentException("BTSP graph must be full");

		hamPath.AddVerticesAndEdge(u1v1);
		return hamPath;
	}

	private WeightedEdge ProcessBstComponent(
		WeightedSparseGraph hamPath, WeightedGraph btsp,
		WeightedSparseGraph bst, int v
	)
	{
		var e = bst.AdjacentEdge(v, 0);

		if (bst.VertexCount == 2)
		{
			hamPath.AddVerticesAndEdge(e);
		}
		else
		{
			var uu1HamPath = FindHamilton(btsp, bst, e.Source, e.Target);
			hamPath.AddVerticesAndEdgeRange(uu1HamPath.Edges);
		}

		return e;
	}

	private WeightedSparseGraph Make3(WeightedGraph btsp, WeightedSparseGraph bst, int u, int v)
	{
		var w = bst.Vertices.First(x => x != u && x != v);
		var hamPath = new UndirectedGraph<int, WeightedEdge>();

		if (!btsp.TryGetEdge(u, w, out WeightedEdge uw))
			throw new ArgumentException("BTSP graph must be full");

		if (!btsp.TryGetEdge(w, v, out WeightedEdge wv))
			throw new ArgumentException("BTSP graph must be full");

		hamPath.AddVerticesAndEdge(uw);
		hamPath.AddVerticesAndEdge(wv);
		return hamPath;
	}
}
