using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MarianaTeixeira.DialogueSystem
{
    public class DialogueNode : Node
    {

        public string NodeID { get; set; }
        public string CharacterName { get; set; }
        public string DialogueText { get; set; }
        public Texture CharacterPortrait { get; set; }

        // Uncertain if ports should be a member variable.
        public Port InputPort { get; set; }
        public Port OutputPort { get; set; }

        public DialogueNode()
        {
            NodeID = Guid.NewGuid().ToString();
            DialogueText = "Default Text";
            CharacterName = "Character Name";
        }

        public DialogueNode(string id, string name, string text, Texture portrait)
        {
            NodeID = id;
            DialogueText = text;
            CharacterName = name;
            CharacterPortrait = portrait;
        }

        public void CreateNode()
        {
            TextField characterNameField = new TextField() { value = CharacterName };
            characterNameField.RegisterValueChangedCallback(evt => CharacterName = evt.newValue);
            titleContainer.Insert(0, characterNameField);

            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            InputPort.portName = "";
            OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            OutputPort.portName = "";
            inputContainer.Add(InputPort);
            outputContainer.Add(OutputPort);

            ObjectField portraitField = new ObjectField("Character Portrait")
            {
                objectType = typeof(Texture),
            };
            portraitField.RegisterValueChangedCallback(evt => CharacterPortrait = evt.newValue as Texture);
            extensionContainer.Add(portraitField);

            Foldout foldout = new Foldout();
            TextField foldoutText = new TextField() { value = DialogueText };
            foldoutText.RegisterValueChangedCallback(evt => DialogueText = evt.newValue);

            foldout.Add(foldoutText);
            extensionContainer.Add(foldout);

            RefreshExpandedState();
        }
    }

}
