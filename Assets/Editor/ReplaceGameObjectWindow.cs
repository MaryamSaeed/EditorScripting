using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

internal sealed class ReplaceGameObjectWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private VisualElement windowRoot;
    private ObjectField replacingObject;
    private ObjectField toBeReplacedObject;
    private Button replaceButton;
    private HelpBox windowHelpBox;
    public List<GameObject> toBeReplacedGameobjects;

    [MenuItem("Tools/ReplaceGameObjectWindow")]
    public static void ShowWindow()
    {
        ReplaceGameObjectWindow wnd = GetWindow<ReplaceGameObjectWindow>();
        wnd.titleContent = new GUIContent("ReplaceGameObjectWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        windowRoot = rootVisualElement;

        // Instantiate UXML
        VisualElement windowVisualTree = m_VisualTreeAsset.Instantiate();
        windowRoot.Add(windowVisualTree);

        InitializeWindowControls(windowRoot);
    }

    private void InitializeWindowControls(VisualElement root)
    {
        replacingObject = root.Query<ObjectField>("ReplacingObjectField");
        replacingObject.objectType = typeof(GameObject);

        toBeReplacedObject = root.Query<ObjectField>("ObjectToBeReplacedField");
        toBeReplacedObject.objectType = typeof(GameObject);

        replaceButton = root.Query<Button>("ReplaceButton");
        replaceButton.clicked += OnReplaceButtonClicked;

        windowHelpBox = root.Query<HelpBox>("HelpBox");
        windowHelpBox.messageType = HelpBoxMessageType.Error;
        windowHelpBox.visible = false;
    }

    private void OnReplaceButtonClicked()
    {
        if (windowHelpBox.visible)
            HideHelpBox();
        var replacingObje = (GameObject) replacingObject.value;
        var tobeReplacedObj = (GameObject) toBeReplacedObject.value;
        Replace(replacingObje, tobeReplacedObj);
    }

    private void Replace(GameObject replacingObj, GameObject tobereplacedObj)
    {
        try
        {
            var oldObj = tobereplacedObj;
            var newObj = Instantiate(replacingObj);
            newObj.transform.position = oldObj.transform.position;
            newObj.transform.localScale = oldObj.transform.localScale;
            newObj.transform.rotation = oldObj.transform.rotation;
            toBeReplacedObject.value = newObj;
            DestroyImmediate(oldObj);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            windowHelpBox.text = e.Message;
            windowHelpBox.visible = true;
        }
    }

    private void HideHelpBox()
    {
        windowHelpBox.visible = false;
    }

    private void OnSelectionChange()
    {
        HideHelpBox();
    }
}

