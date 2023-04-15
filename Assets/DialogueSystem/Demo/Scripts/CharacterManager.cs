using MarianaTeixeira.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] DialogueGraphData[] _dialogueDatas;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            DialogueInteractions.onDialogueUpdate?.Invoke(_dialogueDatas[0]);
    }
}
