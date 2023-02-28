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
    private Button replaceObjectsButton;
    private HelpBox windowHelpBox;
    private InspectorElement listInspector;
    private ScriptableObject listObject;
    private SerializedObject serializedListObject;

    [SerializeField]
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

        DrawListViaDefaultInspector();

        replaceButton = root.Query<Button>("ReplaceButton");
        replaceButton.clicked += OnReplaceButtonClicked;

        replaceObjectsButton = root.Query<Button>("ReplaceGameObjectsButton");
        replaceObjectsButton.clicked += OnReplaceObjectsButton;

        windowHelpBox = root.Query<HelpBox>("HelpBox");
        windowHelpBox.messageType = HelpBoxMessageType.Error;
        windowHelpBox.visible = false;

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

    private void DrawListAsScriptableObject()
    {
        ScriptableObject listObject = CreateInstance(typeof(ObjectsList));
        SerializedObject serializedListObject = new SerializedObject(listObject);
        SerializedProperty sproperty = serializedListObject.FindProperty("test");
        ObjectsListDrawerUIE drawer = new ObjectsListDrawerUIE();
        var visualElement = drawer.CreatePropertyGUI(sproperty);
        windowRoot.Add(visualElement);
    }

    private void DrawListViaDefaultInspector()
    {
        listObject = CreateInstance(typeof(ObjectsList));
        serializedListObject = new SerializedObject(listObject);
        listInspector = windowRoot.Query<InspectorElement>("ObjectsListInspector");
        listInspector.Bind(serializedListObject);
    }

    private void OnReplaceButtonClicked()
    {
        if (windowHelpBox.visible)
            HideHelpBox();
        var replacingObje = (GameObject) replacingObject.value;
        var tobeReplacedObj = (GameObject) toBeReplacedObject.value;
        Replace(replacingObje, tobeReplacedObj);
        toBeReplacedObject.value = tobeReplacedObj;
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
            //toBeReplacedObject.value = newObj;
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

[Serializable]
public class ObjectsList : ScriptableObject
{
    public List<GameObject> ObjectsToBeReplaced;
}

// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(ObjectsList))]
public class ObjectsListDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var listContainer = new VisualElement();

        var listField = new PropertyField(property.FindPropertyRelative("ObjectsToBeReplaced"));

        listContainer.Add(listField);

        return listContainer;
    }
}

