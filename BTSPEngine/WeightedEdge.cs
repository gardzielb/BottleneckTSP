using QuikGraph;
using System.Xml.Serialization;

namespace BTSPEngine;

public class WeightedEdge : IEdge<int>
{
	public int Source { get; }
	public int Target { get; }
	[XmlAttribute("weight")] public double Weight { get; }

	public WeightedEdge(int source, int target, double weight)
	{
		Source = source;
		Target = target;
		Weight = weight;
	}

	public override string ToString()
	{
		return $"({Source}, {Target}, {Weight})";
	}
}