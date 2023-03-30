using System;
using UnityEngine;

namespace MarianaTeixeira.DialogueSystem
{
    [Serializable]
    public class DialogueNodeData
    {
        public string ID;
        public string Name;
        public string Dialogue;
        public Texture Portrait;
        public Rect Position;
    }
}