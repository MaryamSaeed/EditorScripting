using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ReplaceGameObjectWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private ObjectField replacingObject;
    private ObjectField toBeReplacedObject;
    private Button replaceButton;

    [MenuItem("Tools/ReplaceGameObjectWindow")]
    public static void ShowWindow()
    {
        ReplaceGameObjectWindow wnd = GetWindow<ReplaceGameObjectWindow>();
        wnd.titleContent = new GUIContent("ReplaceGameObjectWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement windowVisualTree = m_VisualTreeAsset.Instantiate();
        root.Add(windowVisualTree);

        replacingObject = root.Query<ObjectField>("ReplacingObjectField");
        replacingObject.objectType = typeof(GameObject);

        toBeReplacedObject = root.Query<ObjectField>("ObjectToBeReplacedField");
        toBeReplacedObject.objectType = typeof(GameObject);

        replaceButton = root.Query<Button>("ReplaceButton");
        replaceButton.clicked += OnReplaceButtonClicked;
    }

    private void OnReplaceButtonClicked()
    {
        var replacingObje = (GameObject) replacingObject.value;
        var tobeReplacedObj = (GameObject) toBeReplacedObject.value;
        Replace(replacingObje, tobeReplacedObj);
    }

    private void Replace(GameObject replacingObj, GameObject tobereplacedObj)
    {
        try
        {
            var newObj = Instantiate(replacingObj);
            var oldObj = tobereplacedObj;
            newObj.transform.position = oldObj.transform.position;
            newObj.transform.localScale = oldObj.transform.localScale;
            newObj.transform.rotation = oldObj.transform.rotation;
            toBeReplacedObject.value = newObj;
            DestroyImmediate(oldObj);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
