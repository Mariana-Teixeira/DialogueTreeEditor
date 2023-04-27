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

        TextField _saveFileField;
        string _fileName;

        ObjectField _loadObjectField;
        DialogueGraphData _saveData;

        [MenuItem("Tools/Dialogue Window")]
        public static void ShowWindow()
        {
            GetWindow<DialogueEditorWindow>("Dialogue Editor");
        }

        public void CreateGUI()
        {
            Toolbar toolbar = new Toolbar();

            _saveFileField = new TextField() { value = _fileName };
            _saveFileField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);

            Button clearButton = new Button(() => ClearData()) { text = "Clear" };
            clearButton.name = "ClearButton";
            Button saveButton = new Button(() => SaveData()) { text = "Save" };
            Button loadButton = new Button(() => LoadData()) { text = "Load" };

            _loadObjectField = new ObjectField()
            {
                objectType = typeof(DialogueGraphData),
            };

            _loadObjectField.RegisterValueChangedCallback(evt =>
            {
                _saveData = evt.newValue as DialogueGraphData;
            });

            VisualElement save = new VisualElement();
            save.Add(saveButton);
            save.Add(_saveFileField);

            VisualElement load = new VisualElement();
            load.Add(loadButton);
            load.Add(_loadObjectField);

            toolbar.Add(save);
            toolbar.Add(load);
            toolbar.Add(clearButton);
            rootVisualElement.Add(toolbar);

            _graphView = new DialogueGraphView();
            _graphView.StretchToParentSize();           
            
            rootVisualElement.Add(_graphView);
            rootVisualElement.Add(toolbar);

            AddStyles();
        }

        #region Saving and Loading Functions

        private void ClearData()
        {
            _saveFileField.value = string.Empty;
            _loadObjectField.value = null;

            DialogueSaveData.ClearGraph(_graphView);
        }

        private void LoadData()
        {
            DialogueSaveData.ClearGraph(_graphView);
            _saveFileField.value = _loadObjectField.value.name;

            string message;
            bool isLoaded = DialogueSaveData.TryLoadData(_graphView, _saveData, out message);

            if (!isLoaded)
            {
                EditorUtility.DisplayDialog(
                    "Failed Load",
                    message,
                    "Okay");
            }
        }

        private void SaveData()
        {
            string message;
            bool isDataSave = DialogueSaveData.TrySaveData(_graphView, _fileName, out message);
            
            if (!isDataSave)
            {
                EditorUtility.DisplayDialog(
                    "Failed Save",
                    message,
                    "Okay");
            }
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