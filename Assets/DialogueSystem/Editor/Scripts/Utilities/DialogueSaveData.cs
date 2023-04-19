using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace MarianaTeixeira.DialogueSystem
{
    public static class DialogueSaveData
    {
        static List<DialogueNode> s_nodes;
        static List<Edge> s_edges;
        static DialogueGraphData s_graphData;

        public static void SaveGraphData(DialogueGraphView graphView, string fileName)
        {
            s_nodes = graphView.nodes.ToList().Cast<DialogueNode>().ToList();
            s_edges = graphView.edges.ToList();

            s_graphData = ScriptableObject.CreateInstance<DialogueGraphData>();

            // Save edges no LinkData
            foreach (var edge in s_edges)
            {
                var outputNode = edge.output.node as DialogueNode;
                var inputNode = edge.input.node as DialogueNode;

                s_graphData.LinksData.Add(new DialogueLinkData()
                {
                    BaseID = outputNode.NodeID,
                    TargetID = inputNode.NodeID,
                });
            }

            // Save nodes to NodeData
            foreach (var node in s_nodes)
            {
                s_graphData.NodesData.Add(new DialogueNodeData()
                {
                    ID = node.NodeID,
                    Name = node.CharacterName,
                    Dialogue = node.DialogueText,
                    Portrait = node.CharacterPortrait,
                    Position = node.GetPosition(),
                });
            }

            if (!AssetDatabase.IsValidFolder($"Assets/DialogueSystem/SaveFiles"))
                AssetDatabase.CreateFolder("Assets/DialogueSystem", "SaveFiles");

            AssetDatabase.CreateAsset(s_graphData, $"Assets/DialogueSystem/SaveFiles/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public static void ClearGraph(DialogueGraphView graphView)
        {
            s_nodes = graphView.nodes.ToList().Cast<DialogueNode>().ToList();
            foreach (var node in s_nodes)
            {
                graphView.RemoveElement(node);
            }

            s_edges = graphView.edges.ToList();
            foreach (var edge in s_edges)
            {
                graphView.RemoveElement(edge);
            }
        }

        public static void LoadGraphData(DialogueGraphView graphView, string fileName)
        {
            //s_graphData = Resources.Load<DialogueGraphData>(fileName);
            s_graphData = AssetDatabase.LoadAssetAtPath<DialogueGraphData>($"Assets/DialogueSystem/SaveFiles/{fileName}.asset");
            if (s_graphData == null) return;

            // Load nodes to GraphView
            foreach (var nodeData in s_graphData.NodesData)
            {
                DialogueNode node = new DialogueNode(nodeData.ID, nodeData.Name, nodeData.Dialogue, nodeData.Portrait);
                node.CreateNode();
                node.SetPosition(nodeData.Position);
                graphView.AddElement(node);
            }
            s_nodes = graphView.nodes.ToList().Cast<DialogueNode>().ToList();

            // Load edges to GraphView
            foreach (var link in s_graphData.LinksData)
            {
                var baseNode = s_nodes.Find(x => x.NodeID.Equals(link.BaseID));
                var targetNode = s_nodes.Find(x => x.NodeID.Equals(link.TargetID));

                Edge edge = new Edge()
                {
                    output = baseNode.OutputPort,
                    input = targetNode.InputPort,
                };
                graphView.AddElement(edge);
            }
        }
    }
}