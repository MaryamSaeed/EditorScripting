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
        importButton.visible = false;
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
        selectedAssetsList.visible = false;
    }
    private void InitWindowTextField()
    {
        assetLink = windowRoot.Query<TextField>("AssetLink");
        assetLink.visible = false;
        assetLink.RegisterValueChangedCallback(evt => { Debug.Log(IsValidURL(evt.newValue)); });
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
            selectedAssetsList.visible = true;
            importButton.visible = true;
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
            }
            else
            {
                Debug.Log("here we go");
                if (!string.IsNullOrEmpty(assetLink.value))
                    FbxProcessor.processModel = true;

                var destinationFolder = Path.Combine(projectDirectory, assetsFolder, modelsFolder);
                await ModelDownloadHandler.GetModel(assetLink.value, destinationFolder);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Faild to import model");
            Debug.LogError(e.Message);
        }

        fileList.Clear();
        selectedAssetsList.Rebuild();
        selectedAssetsList.visible = false;
        importButton.visible = false;
        importOptionsRadioGroup.SetEnabled(true);
    }
    private void OnRadiobuttonvalueChanges(int value)
    {
        if (value == 0)
        {
            selectAssetsButton.visible = true;
            assetLink.visible = false;
            importButton.visible = false;
        }
        else
        {
            selectAssetsButton.visible = false;
            assetLink.visible = true;
            importButton.visible = true;
        }
    }
    private bool IsValidURL(string URL)
    {
        string Pattern = @"^(?:http?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\]+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+[=]+(?:\.[\w\.-])";
        Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return Rgx.IsMatch(URL);
    }
}