﻿using QuikGraph;

namespace BTSPEngine
{
    public static class AlgorithmExtensions
    {
        public static (UndirectedGraph<int, TEdge>, double) PrimMST<TEdge>(this IVertexListGraph<int, TEdge> graph, Func<TEdge, double> edgeWeights, int root = 0) 
            where TEdge : IEdge<int>
        {
            var tree = new UndirectedGraph<int, TEdge>(false);
            var vertices = Enumerable.Range(0, graph.VertexCount);
            double totalWeight = 0.0;
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
                    totalWeight += edgeWeights(weightedEdges[minVertex]);
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

            return (tree, totalWeight);
        }

        public static UndirectedGraph<int, TEdge> ConnectedComponentContaining<TEdge>(
            this UndirectedGraph<int, TEdge> graph, int v
        ) where TEdge : IEdge<int>
        {
            var component = new UndirectedGraph<int, TEdge>();
            var visitArray = new bool[graph.VertexCount];
            var vertexQueue = new Queue<int>();
            vertexQueue.Enqueue(v);
            
            while (vertexQueue.Count > 0)
            {
                int u = vertexQueue.Dequeue();
                if (visitArray[u])
                    continue;
                
                visitArray[u] = true;
                foreach (var edge in graph.AdjacentEdges(u))
                {
                    int w = edge.Source == u ? edge.Target : edge.Source;
                    if (visitArray[w]) 
                        continue;

                    component.AddVerticesAndEdge(edge);
                    vertexQueue.Enqueue(w);
                }
            }

            return component;
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
