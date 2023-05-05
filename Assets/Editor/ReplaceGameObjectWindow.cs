using System;
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
    private VisualElement listFieldContainer;
    private Button replaceObjectsButton;
    private HelpBox windowHelpBox;
    private ScriptableObject listObject;
    private string oldVersionWarning = "Upgrade to unity 2022.2 or newer to be able to drop multiple objects on the list, in this version list object should be assigned one by one.";
 
    [MenuItem("Tools/ReplaceGameObjectWindow")]
    public static void ShowWindow()
    {
        ReplaceGameObjectWindow wnd = GetWindow<ReplaceGameObjectWindow>();
        wnd.titleContent = new GUIContent("ReplaceGameObjectWindow");
    }

    public void CreateGUI()
    {
        windowRoot = rootVisualElement;
        VisualElement windowVisualTree = m_VisualTreeAsset.Instantiate();
        windowRoot.Add(windowVisualTree);

        InitializeWindowControls(windowRoot);
    }

    private void InitializeWindowControls(VisualElement root)
    {
        replacingObject = root.Query<ObjectField>("ReplacingObjectField");
        replacingObject.objectType = typeof(GameObject);

        listFieldContainer = root.Query<VisualElement>("ListFieldContainer");
        DrawListViaPropertyDrawer(listFieldContainer);

        replaceObjectsButton = root.Query<Button>("ReplaceGameObjectsButton");
        replaceObjectsButton.clicked += OnReplaceObjectsButton;

        windowHelpBox = root.Query<HelpBox>("HelpBox");
        windowHelpBox.messageType = HelpBoxMessageType.Error;
        windowHelpBox.visible = false;

#if !UNITY_2022_2_OR_NEWER
        ShowOldVersionWarning();
#endif
    }

    private void ShowOldVersionWarning()
    {
        HelpBox WarningBox = new HelpBox(oldVersionWarning, HelpBoxMessageType.Warning);
        WarningBox.visible = true;
        windowRoot.Add(WarningBox);
    }

    private void OnReplaceObjectsButton()
    {
        var replacingObje = (GameObject) replacingObject.value;
        ObjectsList obj = (ObjectsList)listObject;
        foreach (var item in obj.ObjectsToBeReplaced)
        {
            Replace(replacingObje, item);
        }
        obj.ObjectsToBeReplaced.Clear();
        Debug.Log("Objects replaced successfully");
    }

    private void DrawListViaPropertyDrawer(VisualElement container)
    {
        listObject = CreateInstance(typeof(ObjectsList));
        SerializedObject serializedListObject = new SerializedObject(listObject);
        SerializedProperty sproperty = serializedListObject.FindProperty("ObjectsToBeReplaced");
        ObjectsListDrawerUIE drawer = new ObjectsListDrawerUIE();
        var visualElement = drawer.CreatePropertyGUI(sproperty);
        container.Add(visualElement);
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

