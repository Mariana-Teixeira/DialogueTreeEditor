using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MarianaTeixeira.DialogueSystem
{
    public class DialogueEditorWindow : EditorWindow
    {
        DialogueGraphView _graphView;
        string _fileName = "Default";

        [MenuItem("Tools/Dialogue Window")]
        public static void ShowWindow()
        {
            GetWindow<DialogueEditorWindow>("DialogueWindow");
        }

        public void CreateGUI()
        {
            #region Instantiate DialogueGraph Data

            var containerGuids = AssetDatabase.FindAssets("t:DialogueContainer");
            var containerList = new List<DialogueGraphData>();

            foreach (var guid in containerGuids)
            {
                containerList.Add(AssetDatabase.LoadAssetAtPath<DialogueGraphData>(AssetDatabase.GUIDToAssetPath(guid)));
            }

            #endregion

            #region Create Toolbar

            Toolbar toolbar = new Toolbar();

            TextField filenameField = new TextField() { value = _fileName };
            filenameField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);

            Button clearButton = new Button(() => ClearData()) { text = "Clear" };
            Button saveButton = new Button(() => SaveData()) { text = "Save" };
            Button loadButton = new Button(() => LoadData()) { text = "Load" };

            toolbar.Add(filenameField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);

            rootVisualElement.Add(toolbar);

            #endregion

            #region Create SplitView
            var splitView = new TwoPaneSplitView(0, 150, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(splitView);

            // Create Dialogue Data List View

            var listView = new ListView();
            splitView.Add(listView);

            listView.makeItem = () => new Label();
            listView.bindItem = (item, index) => { (item as Label).text = containerList[index].name; };
            listView.itemsSource = containerList;

            listView.onSelectionChange += (IEnumerable<object> selectedItems) =>
            {
                var selectedFile = selectedItems.First() as DialogueGraphData;
                filenameField.value = selectedFile.name;
            };

            // Create Dialogue GraphView

            _graphView = new DialogueGraphView();
            splitView.Add(_graphView);
            #endregion
        }

        #region Saving and Loading Functions

        private void ClearData()
        {
            DialogueSaveData.ClearGraph(_graphView);
        }

        private void LoadData()
        {
            DialogueSaveData.ClearGraph(_graphView);
            DialogueSaveData.LoadGraphData(_graphView, _fileName);
        }

        private void SaveData()
        {
            DialogueSaveData.SaveGraphData(_graphView, _fileName);
        }

        #endregion
    }
}