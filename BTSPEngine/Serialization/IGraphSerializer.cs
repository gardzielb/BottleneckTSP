using QuikGraph;

namespace BTSPEngine.Serialization
{
	public interface IGraphSerializer
	{
		void Serialize(BidirectionalMatrixGraph<WeightedEdge> graph, string path);
		BidirectionalMatrixGraph<WeightedEdge> Deserialize(string path);
		void Serialize(UndirectedGraph<int, WeightedEdge> cycle, string path, double maxCost);
	}
}
