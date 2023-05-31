using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This class extracts the materials and textures embedded in an fbx model
/// this can only be done after the import process finishes, that's why its done in the postprocess
/// </summary>
internal sealed class FbxProcessor : AssetPostprocessor
{
    public static bool processModel = false;

    //destination files names
    private static readonly string assetsFolder = "Assets";
    private static readonly string materialsFolder = "Materials";
    private static readonly string texturesFolder = "Textures";

    /// <summary>
    /// extract all materials on post process 
    /// cause this means the file is already imported unity and be processed
    /// </summary>
    /// <param name="importedAssets">List of recently imported assets</param>
    private static void OnPostprocessAllAssets(string[] importedAssets,
                                               string[] deletedAssets,
                                               string[] movedAssets,
                                               string[] movedFromAssetPaths,
                                               bool didDomainReload)
    {
        //note: in order for fbx processor to process the recently imported models
        //make sure your script sets processModel to true
        if (processModel)
        {
            if (importedAssets.Length > 0)
            {
                var materialsPath = Path.Combine(assetsFolder, materialsFolder);
                var texturesPath = Path.Combine(assetsFolder, texturesFolder);

                foreach (var asset in importedAssets)
                {
                    var assetTextures = AssetImporter.GetAtPath(asset) as ModelImporter;
                    var success  = assetTextures.ExtractTextures(texturesPath);
                    if (success)
                    {
                        Debug.Log("textures extracted successfully");
                    }
                    else
                    {
                        Debug.Log("failed to extract textures");
                    }

                    ExtractMaterialsFromAsset(asset, materialsPath);
                }
                processModel = false;
            }
        }
    }

    /// <summary>
    /// Extracts the material embedded in the mported model
    /// </summary>
    /// <param name="asset">the path to this asset in the assets folder</param>
    /// <param name="materialsPath">the destination path to which we extracts the materials</param>
    private static void ExtractMaterialsFromAsset(string asset, string materialsPath)
    {
        var subAssets = AssetDatabase.LoadAllAssetsAtPath(asset);
        var materials = Array.FindAll(subAssets, x => x.GetType() == typeof(Material));

        for (int i = 0; i < materials.Length; i++)
        {
            var extractionPath = Path.Combine(materialsPath, $"{materials[i].name}.mat");
            var error = AssetDatabase.ExtractAsset(materials[i], extractionPath);

            if (string.IsNullOrEmpty(error))
            {
                //rewrite import settings and re import the asset
                AssetDatabase.WriteImportSettingsIfDirty(asset);
                AssetDatabase.ImportAsset(asset, ImportAssetOptions.ForceUpdate);
            }
            else
            {
                Debug.LogError($"Could not extract material {materials[i].name} due to the following error {error}");
            }
        }
    }
}
