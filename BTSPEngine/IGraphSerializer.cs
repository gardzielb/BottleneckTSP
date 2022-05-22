using QuikGraph;

namespace BTSPEngine
{
	public interface IGraphSerializer
	{
		void Serialize(BidirectionalMatrixGraph<WeightedEdge> graph, string path);
		BidirectionalMatrixGraph<WeightedEdge> Deserialize(string path);
	}
}