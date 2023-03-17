using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetImporterWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private VisualElement windowRoot;
    private Button selectAssetsButton;
    private ListView selectedAssetsList;
    private Button importButton;
    private List<string> fileList;
    private string projectDirectory;
    private const string assetsFolder = "Assets";
    private const string modelsFolder = "Models";


    [MenuItem("Tools/AssetImporterWindow")]
    public static void ShowExample()
    {
        AssetImporterWindow wnd = GetWindow<AssetImporterWindow>();
        wnd.titleContent = new GUIContent("AssetImporterWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        windowRoot = rootVisualElement;

        // Instantiate UXML
        VisualElement visualtree = m_VisualTreeAsset.Instantiate();
        windowRoot.Add(visualtree);

        InitWindowButtons();
        InitWindowList();
    }

    private void InitWindowButtons()
    {
        selectAssetsButton = windowRoot.Query<Button>("SelectAssetsFromDeskButton");
        selectAssetsButton.clicked += OnSelectAssetsButtonClicked;

        importButton = windowRoot.Query<Button>("ImportSelectedAssetsButton");
        importButton.clicked += OnImportButtonClicked;
        importButton.visible = false;
    }

    private void InitWindowList()
    {
        fileList = new List<string>();
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = Path.GetFileName(fileList[i]);
        selectedAssetsList = windowRoot.Query<ListView>("SelectedAssetsList");
        selectedAssetsList.itemsSource = fileList;
        selectedAssetsList.makeItem = makeItem;
        selectedAssetsList.bindItem = bindItem;
        selectedAssetsList.fixedItemHeight = 20;
        selectedAssetsList.style.flexGrow = 1;
        selectedAssetsList.visible = false;
    }
    private void OnSelectAssetsButtonClicked()
    {
        var directory = Directory.GetCurrentDirectory();
        var path = EditorUtility.OpenFilePanel("Select asset", directory,"*.fbx");

        if (!string.IsNullOrEmpty(path))
        {
            fileList.Add(path);
            selectedAssetsList.Rebuild();
            selectedAssetsList.visible = true;
            importButton.visible = true;
        }
    }

    private void OnImportButtonClicked()
    {
        modelsAssetFolder = AssetDatabase.CreateFolder(assetsFolder, modelsFolder);

        foreach (var path in fileList)
        {
            var filename = Path.GetFileName(path);
            var modlesFolder = Path.Combine(projectDirectory, assetsFolder, modelsFolder,filename);
            File.Copy(path, modlesFolder,true);
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }
}