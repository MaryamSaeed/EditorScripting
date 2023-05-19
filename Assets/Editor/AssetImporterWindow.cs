using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
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
    private RadioButtonGroup importOptionsRadioGroup;
    private TextField assetLink;

    private readonly string assetsFolder = "Assets";
    private readonly string modelsFolder = "Models";
    private readonly string materialsFolder = "Materials";
    private readonly string texturesFolder = "Textures";

    private string projectDirectory;
    private string materialsFolderPath;
    private string texturesFolderPath;

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
        FbxProcessor.processModel = false;
        windowRoot.Add(visualtree);
        InitWindowButtons();
        InitWindowList();
        InitWindowTextField();
        InitRadioButtonGroup();
        InitializeFolders();
    }

    private void InitRadioButtonGroup()
    {
        importOptionsRadioGroup = windowRoot.Query<RadioButtonGroup>("ImportOptions");
        importOptionsRadioGroup.choices = new List<string>() { "Import from desk", "Import from link" };
        importOptionsRadioGroup.SetValueWithoutNotify(0);
        importOptionsRadioGroup.RegisterValueChangedCallback(evt => OnRadiobuttonvalueChanges(evt.newValue));
    }
    private void InitWindowButtons()
    {
        selectAssetsButton = windowRoot.Query<Button>("SelectAssetsFromDeskButton");
        selectAssetsButton.clicked += OnSelectAssetsButtonClicked;

        importButton = windowRoot.Query<Button>("ImportSelectedAssetsButton");
        importButton.clicked += OnImportButtonClicked;
        importButton.style.display = DisplayStyle.None;
    }
    private void InitWindowList()
    {
        fileList = new List<string>();
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = Path.GetFileName(fileList[i]);
        selectedAssetsList = windowRoot.Query<ListView>("SelectedAssetsList");
        selectedAssetsList.itemsSource = fileList;
        selectedAssetsList.showAddRemoveFooter = true;
        selectedAssetsList.makeItem = makeItem;
        selectedAssetsList.bindItem = bindItem;
        selectedAssetsList.fixedItemHeight = 20;
        selectedAssetsList.style.flexGrow = 1;
        selectedAssetsList.style.display = DisplayStyle.None;
    }
    private void InitWindowTextField()
    {
        assetLink = windowRoot.Query<TextField>("AssetLink");
        assetLink.style.display = DisplayStyle.None;
        assetLink.RegisterValueChangedCallback(evt => { Debug.Log(evt.newValue); });
    }
    private void InitializeFolders()
    {
        if (string.IsNullOrEmpty(projectDirectory))
            projectDirectory = Directory.GetCurrentDirectory();
        var modlesFolderPath = Path.Combine(projectDirectory, assetsFolder, modelsFolder);
        if (!Directory.Exists(modlesFolderPath))
            AssetDatabase.CreateFolder(assetsFolder, modelsFolder);
        materialsFolderPath = Path.Combine(projectDirectory, assetsFolder, materialsFolder);
        if (!Directory.Exists(materialsFolderPath))
            AssetDatabase.CreateFolder(assetsFolder, materialsFolder);
        texturesFolderPath = Path.Combine(projectDirectory, assetsFolder, texturesFolder);
        if (!Directory.Exists(texturesFolderPath))
            AssetDatabase.CreateFolder(assetsFolder, texturesFolder);
    }
    private void OnSelectAssetsButtonClicked()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File","", "fbx", true);

        if (paths.Length > 0)
        {
            fileList.AddRange(paths.ToList());
            selectedAssetsList.Rebuild();
            importOptionsRadioGroup.SetEnabled(false);
            selectedAssetsList.style.display = DisplayStyle.Flex;
            importButton.style.display = DisplayStyle.Flex;
        }
    }
    private async void OnImportButtonClicked()
    {
        try
        {
            if (importOptionsRadioGroup.value == 0)
            {
                if (fileList.Count > 0)
                    FbxProcessor.processModel = true;

                foreach (var path in fileList)
                {
                    var filename = Path.GetFileName(path);
                    var destinationFolder = Path.Combine(projectDirectory, assetsFolder, modelsFolder,filename);
                    File.Copy(path, destinationFolder, true);
                    AssetDatabase.Refresh(ImportAssetOptions.Default);
                }

                fileList.Clear();
                selectedAssetsList.Rebuild();
                selectedAssetsList.style.display = DisplayStyle.None;
            }
            else
            {
                assetLink.SetEnabled(false);

                var destinationFolder = Path.Combine(projectDirectory, assetsFolder, modelsFolder);
                await ModelDownloadHandler.GetModel(assetLink.value, destinationFolder);
                FbxProcessor.processModel = true;
                AssetDatabase.Refresh(ImportAssetOptions.Default);

                assetLink.value = string.Empty;
                assetLink.SetEnabled(true);
                assetLink.style.display = DisplayStyle.None;
            }

            FbxProcessor.processModel = false;
        }
        catch (Exception e)
        {
            Debug.LogError("Faild to import model");
            Debug.LogError(e.Message);
        }

        importButton.style.display = DisplayStyle.None;
        importOptionsRadioGroup.SetEnabled(true);
        importOptionsRadioGroup.SetValueWithoutNotify(0);
        selectAssetsButton.style.display = DisplayStyle.Flex;
    }
    private void OnRadiobuttonvalueChanges(int value)
    {
        if (value == 0)
        {
            selectAssetsButton.style.display = DisplayStyle.Flex;
            assetLink.style.display = DisplayStyle.None;
            importButton.style.display = DisplayStyle.None;
        }
        else
        {
            selectAssetsButton.style.display = DisplayStyle.None;
            assetLink.style.display = DisplayStyle.Flex;
            importButton.style.display = DisplayStyle.Flex;
        }
    }

    private void OnDisable()
    {
        selectAssetsButton.clicked -= OnSelectAssetsButtonClicked;
        importButton.clicked -= OnImportButtonClicked;
    }

}