using BTSPEngine;
using FluentAssertions;
using QuikGraph;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void Test1(IVertexListGraph<int, WeightedEdge> g)
        {
            var (path, cost) = ExactBTSP.CalculateExactBTSP(g, e => e.Weight);
            cost.Should().Be(2);
        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] {
                    GraphBuilder(5, new WeightedEdge []
                    {
                        new (0, 1, 1),
                        new (0, 2, 2),
                        new (0, 3, 2),
                        new (0, 4, 3),
                        new (1, 2, 1),
                        new (1, 3, 2),
                        new (1, 4, 2),
                        new (2, 3, 1),
                        new (2, 4, 2),
                        new (3, 4, 1),
                    })
                }
            };

        public static BidirectionalMatrixGraph<WeightedEdge> GraphBuilder(int n, IEnumerable<WeightedEdge> edges)
        {
            var g = new BidirectionalMatrixGraph<WeightedEdge>(n);
            foreach (var edge in edges)
            {
                g.AddEdge(edge);
                g.AddEdge(new WeightedEdge(edge.Target, edge.Source, edge.Weight));
            }
            return g;
        }
    }
}