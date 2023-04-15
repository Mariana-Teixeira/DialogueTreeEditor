using System;

namespace MarianaTeixeira.DialogueSystem
{
    public static class DialogueInteractions
    {
        public static Action<DialogueGraphData> onDialogueUpdate;
        public static Action onDialogueEnd;
    }
}
