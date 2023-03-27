using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class DialogueSaveData
{
    static List<DialogueNode> _nodes;
    static List<Edge> _edges;
    static DialogueContainer _container;

    public static void SaveGraphData(DialogueGraphView graphView, string fileName)
    {
        _nodes = graphView.nodes.ToList().Cast<DialogueNode>().ToList();
        _edges = graphView.edges.ToList();

        _container = ScriptableObject.CreateInstance<DialogueContainer>();

        // Save edges no LinkData
        foreach (var edge in _edges)
        {
            var outputNode = edge.output.node as DialogueNode;
            var inputNode = edge.input.node as DialogueNode;

            _container._linkData.Add(new DialogueLinkData()
            {
                baseID = outputNode._NodeID,
                targetID = inputNode._NodeID,
            });
        }

        // Save nodes to NodeData
        foreach (var node in _nodes)
        {
            _container._nodesData.Add(new DialogueNodeData()
            {
                _id = node._NodeID,
                _name = node._CharacterName,
                _dialogue = node._DialogueText,
                _portrait = node._CharacterPortrait,
                _position = node.GetPosition(),
            });
        }

        if (!AssetDatabase.IsValidFolder($"Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        AssetDatabase.CreateAsset(_container, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public static void ClearGraph(DialogueGraphView graphView)
    {
        _nodes = graphView.nodes.ToList().Cast<DialogueNode>().ToList();
        foreach (var node in _nodes)
        {
            graphView.RemoveElement(node);
        }

        _edges = graphView.edges.ToList();
        foreach (var edge in _edges)
        {
            graphView.RemoveElement(edge);
        }
    }

    public static void LoadGraphData(DialogueGraphView graphView, string fileName)
    {
        _container = Resources.Load<DialogueContainer>(fileName);
        if (_container == null) return;

        // Load nodes to GraphView
        foreach (var nodeData in _container._nodesData)
        {
            DialogueNode node = new DialogueNode(nodeData._id, nodeData._name, nodeData._dialogue, nodeData._portrait);
            node.CreateNode();
            node.SetPosition(nodeData._position);
            graphView.AddElement(node);
        }
        _nodes = graphView.nodes.ToList().Cast<DialogueNode>().ToList();

        // Load edges to GraphView
        foreach (var link in _container._linkData)
        {
            var baseNode = _nodes.Find(x => x._NodeID.Equals(link.baseID));
            var targetNode = _nodes.Find(x => x._NodeID.Equals(link.targetID));

            Edge edge = new Edge()
            {
                output = baseNode._OutputPort,
                input = targetNode._InputPort,
            };
            graphView.AddElement(edge);
        }
    }
}