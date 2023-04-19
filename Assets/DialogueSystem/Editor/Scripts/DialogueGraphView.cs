using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MarianaTeixeira.DialogueSystem
{
    public class DialogueGraphView : GraphView
    {
        public DialogueGraphView()
        {
            AddGridBackground();
            AddManipulators();
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
                        "Add Node", actionEvent => AddElement(CreateNode(
                            GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
                    )
                );

            return contextualMenuManipulator;
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition)
        {
            return contentViewContainer.WorldToLocal(mousePosition);
        }

        public DialogueNode CreateNode(Vector2 position)
        {
            DialogueNode node = new DialogueNode(position);
            node.CreateNode();

            return node;
        }

        private void AddGridBackground()
        {
            GridBackground grid = new GridBackground();
            grid.StretchToParentSize();

            Insert(0, grid);
        }
    }

}