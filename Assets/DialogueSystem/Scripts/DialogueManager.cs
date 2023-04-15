using UnityEngine;
using UnityEngine.UI;

namespace MarianaTeixeira.DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] DialogueCanvasManager _canvasManager;
        DialogueCanvasElements[] _dialogueArray;
        int _dialogueArrayIndex;

        private void OnEnable()
        {
            DialogueInteractions.onDialogueUpdate += OnDialoguePress;
            DialogueInteractions.onDialogueEnd += OnDialogueEnd;
        }

        private void OnDisable()
        {
            DialogueInteractions.onDialogueUpdate -= OnDialoguePress;
            DialogueInteractions.onDialogueEnd -= OnDialogueEnd;
        }

        #region Loading DialogueArray
        void LoadDialogueArray(DialogueGraphData currentSaveData)
        {
            _dialogueArray = new DialogueCanvasElements[currentSaveData.NodesData.Count];

            // Finding the EntryPointNode
            string nodeID = FindEntryNode(currentSaveData);
            var nodeData = currentSaveData.NodesData.Find(x => x.ID == nodeID);
            AddDataToStruct(nodeData, ref _dialogueArray[0]);

            // Finding Connected Nodes
            for (int i = 1; i < _dialogueArray.Length; i++)
            {
                nodeID = FindConnectedNode(currentSaveData, nodeID);
                nodeData = currentSaveData.NodesData.Find(x => x.ID == nodeID);
                AddDataToStruct(nodeData, ref _dialogueArray[i]);
            }
        }

        void AddDataToStruct(DialogueNodeData dt, ref DialogueCanvasElements str)
        {
            str.Name = dt.Name;
            str.Dialogue = dt.Dialogue;
            str.Portrait = dt.Portrait;
        }

        string FindEntryNode(DialogueGraphData saveData)
        {
            foreach (var link in saveData.LinksData)
            {
                var baseID = link.BaseID;
                var inputLink = saveData.LinksData.Find(x => x.TargetID == baseID);

                if (inputLink == null) return baseID;
            }

            return null;
        }

        string FindConnectedNode(DialogueGraphData currentSaveData, string currentNodeID)
        {
            var inputLink = currentSaveData.LinksData.Find(x => x.BaseID == currentNodeID);
            if (inputLink == null) return null;
            else return inputLink.TargetID;
        }
        #endregion

        public void OnLoadDialogue(DialogueGraphData saveData)
        {
            LoadDialogueArray(saveData);
        }


        void OnDialoguePress(DialogueGraphData saveData)
        {
            if (_dialogueArray == null) OnDialogueStart(saveData);

            bool haveDialogueLeft = _dialogueArrayIndex < _dialogueArray.Length;

            if (haveDialogueLeft)
                CallUpdateCanvas();
            else
                OnDialogueEnd();

            _dialogueArrayIndex++;
        }

        void OnDialogueStart(DialogueGraphData saveData)
        {
            _canvasManager.ToggleCanvasVisibility(true);
            _dialogueArrayIndex = 0;
            LoadDialogueArray(saveData);
        }

        void OnDialogueEnd()
        {
            _canvasManager.ToggleCanvasVisibility(false);
            _canvasManager.ResetCanvas();
            _dialogueArray = null;
        }

        void CallUpdateCanvas()
        {
            string name = _dialogueArray[_dialogueArrayIndex].Name;
            string dialogue = _dialogueArray[_dialogueArrayIndex].Dialogue;
            Texture portrait = _dialogueArray[_dialogueArrayIndex].Portrait;
            _canvasManager.UpdateCanvas(name, dialogue, portrait);
        }
    }
}