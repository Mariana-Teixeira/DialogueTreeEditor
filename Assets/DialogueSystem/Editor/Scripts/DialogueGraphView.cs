using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace MarianaTeixeira.DialogueSystem
{
    public class DialogueGraphView : GraphView
    {
        public DialogueGraphView()
        {
            AddManipulators();
            AddGridBackground();
            AddStyles();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort.node == port.node || startPort.direction == port.direction) return;
                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());

            this.AddManipulator(CreateContextualMenu());
        }

        private IManipulator CreateContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator
                (
                menuEvent => menuEvent.menu.AppendAction
                    (
                        "Add Node", actionEvent => AddElement(CreateNode())
                    )
                );

            return contextualMenuManipulator;
        }

        public DialogueNode CreateNode()
        {
            DialogueNode node = new DialogueNode();
            node.CreateNode();

            return node;
        }

        private void AddGridBackground()
        {
            GridBackground grid = new GridBackground();
            grid.StretchToParentSize();

            Insert(0, grid);
        }

        private void AddStyles()
        {
            StyleSheet graphViewStyle = (StyleSheet)EditorGUIUtility.Load("WindowStyle.uss");
            StyleSheet nodeStyle = (StyleSheet)EditorGUIUtility.Load("NodeStyle.uss");
            styleSheets.Add(graphViewStyle);
            styleSheets.Add(nodeStyle);
        }
    }

}