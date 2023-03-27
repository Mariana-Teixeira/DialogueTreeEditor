using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] DialogueContainer container;
    Text _name;
    Text _dialogue;
    RawImage _portrait;

    DialogueStruct[] _dialogueArray;
    int index;

    private void Start()
    {
        FindObjectsOnScene();
        LoadDialogueArray();
    }

    void FindObjectsOnScene()
    {
        _name = GameObject.Find("ds_name").GetComponent<Text>();
        _dialogue = GameObject.Find("ds_dialogue").GetComponent<Text>();
        _portrait = GameObject.Find("ds_portrait").GetComponent<RawImage>();
    }

    void LoadDialogueArray()
    {
        _dialogueArray = new DialogueStruct[container._nodesData.Count];

        // Finding the EntryPointNode
        string nodeID = FindEntryNode();
        var nodeData = container._nodesData.Find(x => x._id == nodeID);
        AddDataToStruct(nodeData, ref _dialogueArray[0]);

        // Finding Connected Nodes
        for (int i = 1; i < _dialogueArray.Length; i++)
        {
            nodeID = FindConnectedNode(nodeID);
            nodeData = container._nodesData.Find(x => x._id == nodeID);
            AddDataToStruct(nodeData, ref _dialogueArray[i]);
        }
    }

    void AddDataToStruct(DialogueNodeData dt, ref DialogueStruct str)
    {
        str._name = dt._name;
        str._dialogue = dt._dialogue;
        str._portrait = dt._portrait;
    }

    string FindEntryNode()
    {
        foreach (var link in container._linkData)
        {
            var baseID = link.baseID;
            var inputLink = container._linkData.Find(x => x.targetID == baseID);

            if (inputLink == null) return baseID;
        }

        return null;
    }

    string FindConnectedNode(string currentNodeID)
    {
        var inputLink = container._linkData.Find(x => x.baseID == currentNodeID);
        if (inputLink == null) return null;
        else return inputLink.targetID;
    }

    public void NextDialogue()
    {
        _name.text = _dialogueArray[index]._name;
        _dialogue.text = _dialogueArray[index]._dialogue;
        _portrait.texture = _dialogueArray[index]._portrait;
        index++;

        if (index >= _dialogueArray.Length) ResetDialogue();
    }

    void ResetDialogue()
    {
        _name.text = null;
        _dialogue.text = null;
        _portrait.texture = null;
        index = 0;
    }
}
