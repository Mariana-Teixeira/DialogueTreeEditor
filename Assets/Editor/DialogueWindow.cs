using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueWindow : EditorWindow
{
    DialogueGraphView _graphView;
    string _fileName = "Default";

    [MenuItem("Tools/Dialogue Window")]
    public static void ShowWindow()
    {
        GetWindow<DialogueWindow>("DialogueWindow");
    }

    public void CreateGUI()
    {
        // LOAD ASSETS
        var containerGuids = AssetDatabase.FindAssets("t:DialogueContainer");
        var containerList = new List<DialogueContainer>();

        foreach(var guid in containerGuids)
        {
            containerList.Add(AssetDatabase.LoadAssetAtPath<DialogueContainer>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        // TOOLBAR
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

        // SPLIT VIEW
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        rootVisualElement.Add(splitView);

        // SPLIT LIST
        var listView = new ListView();
        splitView.Add(listView);

        listView.makeItem = () => new Label();
        listView.bindItem = (item, index) => { (item as Label).text = containerList[index].name; };
        listView.itemsSource = containerList;

        listView.onSelectionChange += (IEnumerable<object> selectedItems) =>
        {
            var selectedFile = selectedItems.First() as DialogueContainer;
            filenameField.value = selectedFile.name;
        };

        // SPLIT GRAPH
        _graphView = new DialogueGraphView();
        splitView.Add(_graphView);
    }

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
}