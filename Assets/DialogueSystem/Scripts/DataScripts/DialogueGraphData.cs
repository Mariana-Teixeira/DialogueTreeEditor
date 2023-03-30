using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarianaTeixeira.DialogueSystem
{
    [Serializable]
    public class DialogueGraphData : ScriptableObject
    {
        public List<DialogueNodeData> NodesData = new List<DialogueNodeData>();
        public List<DialogueLinkData> LinksData = new List<DialogueLinkData>();
    }
}
