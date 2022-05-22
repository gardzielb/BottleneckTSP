using QuikGraph;
using QuikGraph.Serialization;
using System.Xml;

namespace BTSPEngine;

public class GraphMLSerializer : IGraphSerializer
{
	public void Serialize(BidirectionalMatrixGraph<WeightedEdge> graph, string path)
	{
		var xmlSettings = new XmlWriterSettings { Indent = true, IndentChars = "\t" };
		using (var xmlWriter = XmlWriter.Create($"{path}.graphml", xmlSettings))
		{
			graph.SerializeToGraphML<int, WeightedEdge, BidirectionalMatrixGraph<WeightedEdge>>(xmlWriter);
		}
	}

	public BidirectionalMatrixGraph<WeightedEdge> Deserialize(string path)
	{
		string graphMLNamespace = "";

		using (var xmlReader = XmlReader.Create(path))
		{
			// read flow until we hit the graphml node
			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "graphml")
				{
					graphMLNamespace = xmlReader.NamespaceURI;
					break;
				}
			}

			if (string.IsNullOrEmpty(graphMLNamespace))
				throw new ArgumentException("graphml node not found");

			if (!xmlReader.ReadToDescendant("graph", graphMLNamespace))
				throw new ArgumentException("graph node not found");

			int vertexCount = int.Parse(ReadAttributeValue(xmlReader, "parse.nodes"));
			var graph = new BidirectionalMatrixGraph<WeightedEdge>(vertexCount);

			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.Element &&
				    xmlReader.NamespaceURI == graphMLNamespace &&
				    xmlReader.Name == "edge")
				{
					using (var subReader = xmlReader.ReadSubtree())
					{
						ReadAttributeValue(xmlReader, "id");
						int source = int.Parse(ReadAttributeValue(xmlReader, "source"));
						if (source >= vertexCount)
							throw new ArgumentException("Invalid edge source");

						int target = int.Parse(ReadAttributeValue(xmlReader, "target"));
						if (target >= vertexCount)
							throw new ArgumentException("Invalid edge target");

						while (subReader.Read())
						{
							if (xmlReader.NodeType == XmlNodeType.Element &&
							    xmlReader.Name == "data" &&
							    xmlReader.NamespaceURI == graphMLNamespace)
							{
								double weight = subReader.ReadElementContentAsDouble();
								graph.AddEdge(new WeightedEdge(source, target, weight));
							}
						}
					}
				}
			}

			return graph;
		}
	}

	private string ReadAttributeValue(XmlReader reader, string attributeName)
	{
		reader.MoveToAttribute(attributeName);
		if (!reader.ReadAttributeValue())
			throw new ArgumentException("missing " + attributeName + " attribute");
		return reader.Value;
	}
}