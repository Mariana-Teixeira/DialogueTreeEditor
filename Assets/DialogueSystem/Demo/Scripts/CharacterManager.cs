using MarianaTeixeira.DialogueSystem;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] DialogueGraphData[] _dialogueDatas;

    private void Start()
    {
        DialogueInteractions.onDialogueLoad?.Invoke(_dialogueDatas[0]);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            DialogueInteractions.onDialogueUpdate?.Invoke();
    }
}
