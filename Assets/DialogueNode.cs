using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNode : Node
{

    public string _NodeID { get; set; }
    public string _CharacterName { get; set; }
    public string _DialogueText { get; set; }
    public Texture _CharacterPortrait { get; set; }

    // Uncertain if ports should be a member variable.
    public Port _InputPort { get; set; }
    public Port _OutputPort { get; set; }

    public DialogueNode()
    {
        _NodeID = Guid.NewGuid().ToString();
        _DialogueText = "Default Text";
        _CharacterName = "Character Name";
    }

    public DialogueNode(string id, string name, string text, Texture portrait)
    {
        _NodeID = id;
        _DialogueText = text;
        _CharacterName = name;
        _CharacterPortrait = portrait;
    }

    public void CreateNode()
    {
        // Adding a Character Name to Title
        TextField characterNameField = new TextField() { value = _CharacterName };
        characterNameField.RegisterValueChangedCallback(evt => _CharacterName = evt.newValue);
        titleContainer.Insert(0, characterNameField);

        // Adding Ports
        _InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        _InputPort.portName = "";
        _OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof (bool));
        _OutputPort.portName = "";
        inputContainer.Add(_InputPort);
        outputContainer.Add(_OutputPort);

        // Adding Character Portrait
        ObjectField portraitField = new ObjectField("Character Portrait")
        {
            objectType = typeof(Texture),
        };
        portraitField.RegisterValueChangedCallback(evt => _CharacterPortrait = evt.newValue as Texture);
        extensionContainer.Add(portraitField);

        // Adding Dialogue Text
        Foldout foldout = new Foldout();
        TextField foldoutText = new TextField() { value = _DialogueText };
        foldoutText.RegisterValueChangedCallback(evt => _DialogueText = evt.newValue);

        foldout.Add(foldoutText);
        extensionContainer.Add(foldout);

        RefreshExpandedState();
    }
}
