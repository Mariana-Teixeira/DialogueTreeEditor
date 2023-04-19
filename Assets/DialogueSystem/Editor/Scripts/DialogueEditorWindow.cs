using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MarianaTeixeira.DialogueSystem
{
    public class DialogueEditorWindow : EditorWindow
    {
        DialogueGraphView _graphView;
        List<DialogueGraphData> saveDialogueData;
        string _fileName = "Default";

        [MenuItem("Tools/Dialogue Window")]
        public static void ShowWindow()
        {
            GetWindow<DialogueEditorWindow>("Dialogue Editor");
        }

        public void CreateGUI()
        {
            #region Instantiate DialogueGraph Data

            #endregion

            #region Create Toolbar

            Toolbar toolbar = new Toolbar();

            TextField filenameField = new TextField() { value = _fileName };
            filenameField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(filenameField);

            #endregion

            #region Create SplitView

            var splitView = new TwoPaneSplitView(0, 150, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(splitView);

            // Create Dialogue GraphView

            _graphView = new DialogueGraphView();
            splitView.Add(_graphView);
            _graphView.UpdateViewTransform(UnityEngine.Vector3.zero, UnityEngine.Vector3.one);

            // Create Dialogue Data List View
            ListView listView = CreateList(filenameField);
            splitView.Insert(0, listView);

            #endregion

            #region Add Buttons to Toolbar

            Button clearButton = new Button(() => ClearData()) { text = "Clear" };

            Button saveButton = new Button(() =>
            {
                SaveData();
                rootVisualElement.Remove(splitView);
                listView = CreateList(filenameField);
                rootVisualElement.Insert(0, splitView);
            }) { text = "Save" };

            Button loadButton = new Button(() => LoadData()) { text = "Load" };

            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);

            rootVisualElement.Add(toolbar);

            #endregion

            AddStyles();
        }

        ListView CreateList(TextField filenameField)
        {
            ListView listView = new ListView();
            SetListValues(listView);

            listView.makeItem = () => new Label();
            listView.bindItem = (item, index) => { (item as Label).text = saveDialogueData[index].name; };
            listView.itemsSource = saveDialogueData;

            listView.onSelectionChange += (IEnumerable<object> selectedItems) =>
            {
                var selectedFile = selectedItems.First() as DialogueGraphData;
                filenameField.value = selectedFile.name;
            };

            return listView;
        }

        void SetListValues(ListView listView)
        {
            var containerGuids = AssetDatabase.FindAssets("t:DialogueGraphData");
            saveDialogueData = new List<DialogueGraphData>();

            foreach (var guid in containerGuids)
            {
                saveDialogueData.Add(AssetDatabase.LoadAssetAtPath<DialogueGraphData>(AssetDatabase.GUIDToAssetPath(guid)));
            }
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

        private void AddStyles()
        {
            StyleSheet windowStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DialogueSystem/Styles/WindowStyle.uss");
            rootVisualElement.styleSheets.Add(windowStyle);
            StyleSheet graphViewStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DialogueSystem/Styles/GraphStyle.uss");
            _graphView.styleSheets.Add(graphViewStyle);
            StyleSheet nodeStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DialogueSystem/Styles/NodeStyle.uss");
            _graphView.styleSheets.Add(nodeStyle);
        }
    }
}