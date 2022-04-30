using System.ComponentModel;
using System.Xml.Serialization;
using QuickGraph;

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
}