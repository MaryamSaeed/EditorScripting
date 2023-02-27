using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ReplaceGameObjectData))]
public class ReplaceGameObjectInspector : Editor
{
    public GameObject prefabField;
    [SerializeField]
    private VisualElement rootElement;
    private SerializedObject replaceGameObjectData;
    private SerializedProperty replacingGameObject;
    private SerializedProperty toBeReplacedGameObjects;

    public void OnEnable()
    {
        rootElement = new VisualElement();

        // Load in UXML template and USS styles, then apply them to the root element.
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ReplaceGameObjectInspector.uxml");
        visualTree.CloneTree(rootElement);

        StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/ReplaceGameObjectInspector.uss");
        rootElement.styleSheets.Add(stylesheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
        var inspectorData = CreateInstance<ReplaceGameObjectData>();
        replaceGameObjectData = new SerializedObject(inspectorData);
        replacingGameObject = replaceGameObjectData.FindProperty("replacingPrefab");
        toBeReplacedGameObjects = replaceGameObjectData.FindProperty("toBeReplaced");

        ObjectField replacingPrefab = new ObjectField("replacing object");
        replacingPrefab.objectType = typeof(GameObject);
        replacingPrefab.BindProperty(replacingGameObject);
        rootElement.Add(replacingPrefab);

        //ObjectField toBeRplaced = new ObjectField("To Be Replaced");
        //toBeRplaced.objectType = typeof(List<GameObject>);
        //toBeRplaced.BindProperty(toBeReplacedGameObjects);
        //rootElement.Add(toBeRplaced);

        return rootElement;
    }
}
