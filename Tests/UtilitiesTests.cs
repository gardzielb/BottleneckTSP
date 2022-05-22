using BTSPEngine;
using FluentAssertions;
using QuikGraph;
using Xunit;

namespace Tests;

public class UtilitiesTests
{
	[Fact]
	public void IsHamilton_CorrectCycle_ShouldReturnTrue()
	{
		var cycle = new UndirectedGraph<int, WeightedEdge>();
		cycle.AddVerticesAndEdge(new(0, 1, 0));
		cycle.AddVerticesAndEdge(new(1, 2, 0));
		cycle.AddVerticesAndEdge(new(2, 3, 0));
		cycle.AddVerticesAndEdge(new(3, 0, 0));

		Utilities.IsCycle(cycle).Should().BeTrue();
	}

	[Fact]
	public void IsHamilton_IncorrectCycle_ShouldReturnFalse()
	{
		var cycle = new UndirectedGraph<int, WeightedEdge>();
		cycle.AddVerticesAndEdge(new(0, 1, 0));
		cycle.AddVerticesAndEdge(new(1, 2, 0));
		cycle.AddVerticesAndEdge(new(1, 3, 0));
		cycle.AddVerticesAndEdge(new(3, 0, 0));

		Utilities.IsCycle(cycle).Should().BeFalse();
	}
}
