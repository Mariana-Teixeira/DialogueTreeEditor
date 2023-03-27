using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<DialogueNodeData> _nodesData = new List<DialogueNodeData>();
    public List<DialogueLinkData> _linkData = new List<DialogueLinkData>();
}
