using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

namespace MarianaTeixeira.DialogueSystem
{
    public static class DialogueSaveData
    {
        static string errorMessage;
        static List<DialogueNode> s_nodes;
        static List<Edge> s_edges;
        static DialogueGraphData s_graphData;

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

        public static bool TrySaveData(DialogueGraphView graphView, string filename, out string message)
        {
            message = string.Empty;

            if (FilenameInvalidWarning(filename))
            {
                message = errorMessage;
                return false;
            }

            if (FilenameDuplicateWarning(filename))
            {
                bool replaceFiles = ReplaceFilesWarning();
                if (!replaceFiles)
                {
                    message = errorMessage;
                    return false;
                }
            }

            s_nodes = graphView.nodes.ToList().Cast<DialogueNode>().ToList();
            s_edges = graphView.edges.ToList();

            if (EmptyGraphWarning())
            {
                message = errorMessage;
                return false;
            }
            
            if (MultipleEntryNodesWarning())
            {
                message = errorMessage;
                return false;
            }

            s_graphData = ScriptableObject.CreateInstance<DialogueGraphData>();

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

            if (!AssetDatabase.IsValidFolder("Assets/DialogueSystem/SaveFiles"))
                AssetDatabase.CreateFolder("Assets/DialogueSystem", "SaveFiles");

            AssetDatabase.CreateAsset(s_graphData, $"Assets/DialogueSystem/SaveFiles/{filename}.asset");
            AssetDatabase.SaveAssets();
            return true;
        }

        static bool FilenameInvalidWarning(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                errorMessage = "Save filename cannot be empty.";
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool MultipleEntryNodesWarning()
        {
            int countEntryNodes = 0;
            foreach(var node in s_nodes)
            {
                if (node.InputPort.connected == false) countEntryNodes++;
            }

            if(countEntryNodes > 1)
            {
                errorMessage = "Cannot save graph with more than one entry node: nodes with no input connection.";
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool EmptyGraphWarning()
        {
            if (s_nodes.Count == 0)
            {
                errorMessage = "Cannot save an empty dialogue graph.";
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool FilenameDuplicateWarning(string filename)
        {
            string[] files = AssetDatabase.FindAssets($"{filename}");

            if (files.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool ReplaceFilesWarning()
        {
            bool utility = EditorUtility.DisplayDialog(
                "Duplicate Filename Warning",
                "Are you sure you want to replace the previous saved file with this new file?",
                "Yes");

            if (utility)
            {
                return true;
            }
            else
            {
                errorMessage = "There's already a file with that name.";
                return false;
            }
        }

        public static bool TryLoadData(DialogueGraphView graphView, DialogueGraphData saveData, out string message)
        {
            message = "Failed Load";

            if (NullGraphDataWarning(saveData))
            {
                message = errorMessage;
                return false;
            }
            s_graphData = saveData;

            foreach (var nodeData in s_graphData.NodesData)
            {
                DialogueNode node = new DialogueNode(nodeData.ID, nodeData.Name, nodeData.Dialogue, nodeData.Portrait);
                node.CreateNode();
                node.SetPosition(nodeData.Position);
                graphView.AddElement(node);
            }
            s_nodes = graphView.nodes.ToList().Cast<DialogueNode>().ToList();

            foreach (var link in s_graphData.LinksData)
            {
                var baseNode = s_nodes.Find(x => x.NodeID.Equals(link.BaseID));
                var targetNode = s_nodes.Find(x => x.NodeID.Equals(link.TargetID));

                Edge edge = baseNode.OutputPort.ConnectTo(targetNode.InputPort);

                graphView.AddElement(edge);
            }

            message = errorMessage;
            return true;
        }

        static bool NullGraphDataWarning(DialogueGraphData graphData)
        {
            if (graphData == null)
            {
                errorMessage = "Couldn't find DialogueGraphData in load ObjectField.";
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}