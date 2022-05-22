using BTSPEngine;
using FluentAssertions;
using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Tests;

public class BTSPTests
{
	private readonly ITestOutputHelper output;

	public BTSPTests(ITestOutputHelper output)
	{
		this.output = output;
	}

	[Theory]
	[MemberData(nameof(RandomGraphs), parameters: 10)]
	public void CorrectnessTest(IVertexListGraph<int, WeightedEdge> inputGraph)
	{
		var bb = new BTSPSolver();
		var approxCycle = bb.SolveBTSP(inputGraph);

		var (mst, _) = inputGraph.PrimMST(e => e.Weight);
		var approxCost = approxCycle.Edges.MaxBy(x => x.Weight).Weight;
		var (exactCycle, exactCost) = inputGraph.ExactBTSP(e => e.Weight);
		output.WriteLine($"exactCost: {exactCost:0.###}, approxCost: {approxCost:0.###}");

		approxCost.Should().BeInRange(exactCost, 3 * exactCost);
		approxCycle.VertexCount.Should().Be(inputGraph.VertexCount);
		approxCycle.EdgeCount.Should().Be(inputGraph.VertexCount);
		Utilities.IsCycle(approxCycle).Should().BeTrue();
		Utilities.IsDistAtMostThree(approxCycle, mst).Should().BeTrue();
	}

	[Theory]
	[MemberData(nameof(RandomGraphs), parameters: 1000)]
	public void ApproxCorrectnessTest(IVertexListGraph<int, WeightedEdge> inputGraph)
	{
		var solver = new BTSPSolver();
		var approxCycle = solver.SolveBTSP(inputGraph);

		var (mst, _) = inputGraph.PrimMST(e => e.Weight);
		var approxCost = approxCycle.Edges.MaxBy(x => x.Weight).Weight;

		approxCycle.VertexCount.Should().Be(inputGraph.VertexCount);
		approxCycle.EdgeCount.Should().Be(inputGraph.VertexCount);
		Utilities.IsCycle(approxCycle).Should().BeTrue();
		Utilities.IsDistAtMostThree(approxCycle, mst).Should().BeTrue();
	}



	private static IEnumerable<object[]> RandomGraphs(int maxSize)
	{
		const int seed = 123;
		var generator = new BTSPGenerator(null, 100);
		var rng = new Random(seed);

		for (int i = 0; i < 100; i++)
		{
			yield return new object[] { generator.Generate(rng.Next(3, maxSize), i) };
		}
	}
}
