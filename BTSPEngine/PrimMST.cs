using QuikGraph;

namespace BTSPEngine
{
    public static class AlgorithmExtensions
    {
        public static AdjacencyGraph<int, TEdge> PrimMST<TEdge>(this IVertexListGraph<int, TEdge> graph, Func<TEdge, double> edgeWeights, int root = 0) 
            where TEdge : IEdge<int>
        {
            var tree = new AdjacencyGraph<int, TEdge>(false, graph.VertexCount, graph.VertexCount + 1);
            var vertices = Enumerable.Range(0, graph.VertexCount);
            tree.AddVertexRange(vertices);

            var dists = Enumerable.Repeat(double.PositiveInfinity, graph.VertexCount).ToArray();
            dists[root] = 0.0;

            var weightedEdges = new TEdge[graph.VertexCount];
            var queue = new ListPriorityQueue(vertices);

            while (queue.Count > 0)
            {
                int minVertex = queue.DequeueMin(dists);

                if(weightedEdges[minVertex] != null)
                {
                    tree.AddEdge(weightedEdges[minVertex]);
                }

                foreach (var edge in graph.OutEdges(minVertex))
                {
                    if (queue.Contains(edge.Target) && edgeWeights(edge) < dists[edge.Target])
                    {
                        dists[edge.Target] = edgeWeights(edge);
                        weightedEdges[edge.Target] = edge;
                    }
                }
            }            

            return tree;
        }
    }

    public class ListPriorityQueue
    {
        private readonly HashSet<int> elements;

        public int Count => elements.Count;

        public ListPriorityQueue(int capacity)
        {
            elements = new HashSet<int>(capacity);
        }

        public ListPriorityQueue(IEnumerable<int> range)
        {
            elements = new HashSet<int>(range);
        }

        public void Add(int item) => elements.Add(item);
        public bool Contains(int elem) => elements.Contains(elem);

        public int DequeueMin(double[] priorities)
        {
            var min = elements.MinBy(x => priorities[x]);
            elements.Remove(min);
            return min;
        }
    }
}
