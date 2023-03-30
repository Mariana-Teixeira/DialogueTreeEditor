using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarianaTeixeira.DialogueSystem
{
    public class DialogueCanvasManager : MonoBehaviour
    {
        [SerializeField] TMP_Text _name;
        [SerializeField] TMP_Text _dialogue;
        [SerializeField] RawImage _portrait;

        public void ToggleCanvasVisibility()
        {
            if (gameObject.activeSelf == true) gameObject.SetActive(false);
            else gameObject.SetActive(true);
        }

        public void UpdateCanvas(string name, string dialogue, Texture portrait)
        {
            _name.text = name;
            _dialogue.text = dialogue;
            _portrait.texture = portrait;
        }

        public void ResetCanvas()
        {
            _name.text = null;
            _dialogue.text = null;
            _portrait.texture = null;
        }
    }

}