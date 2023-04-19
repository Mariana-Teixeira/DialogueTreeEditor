using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
using UnityEditor;
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

        public DialogueNode(Vector2 position)
        {
            NodeID = Guid.NewGuid().ToString();
            DialogueText = "Default Text";
            CharacterName = "Character Name";

            SetPosition(new Rect(position, Vector2.zero));
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
            #region Character Name Field

            TextField characterNameField = new TextField() { value = CharacterName };
            characterNameField.RegisterValueChangedCallback(evt => CharacterName = evt.newValue);
            titleContainer.Insert(0, characterNameField);
            characterNameField.name = "CharacterName";
            titleContainer.AddToClassList("Container");

            #endregion

            #region IO Ports

            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            InputPort.portName = "";
            OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            OutputPort.portName = "";
            inputContainer.Add(InputPort);
            inputContainer.AddToClassList("Container");
            outputContainer.Add(OutputPort);
            outputContainer.AddToClassList("Container");

            #endregion

            #region Portrait Foldout

            Foldout portraitFoldout = new Foldout();

            Image portraitImage = new Image()
            {
                scaleMode = ScaleMode.ScaleToFit,
            };
            portraitImage.name = "PortraitImage";

            ObjectField portraitField = new ObjectField("Character Portrait")
            {
                objectType = typeof(Texture2D)
            };

            portraitField.RegisterValueChangedCallback(evt =>
            {
                CharacterPortrait = evt.newValue as Texture;
                portraitImage.image = evt.newValue as Texture;
            });

            portraitFoldout.Add(portraitField);
            portraitFoldout.Add(portraitImage);
            extensionContainer.Add(portraitFoldout);

            #endregion

            #region Dialogue Field

            TextField dialogueText = new TextField() { value = DialogueText, multiline = true };
            dialogueText.RegisterValueChangedCallback(evt => DialogueText = evt.newValue);
            dialogueText.name = "DialogueText";

            extensionContainer.Add(dialogueText);
            extensionContainer.AddToClassList("Container");

            #endregion

            RefreshExpandedState();
        }
    }

}
