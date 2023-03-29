using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

internal sealed class FbxProcessor : AssetPostprocessor
{
    public static bool processModel = false;

    private static readonly string assetsFolder = "Assets";
    private static readonly string materialsFolder = "Materials";
    private static readonly string texturesFolder = "Textures";
    private static ModelImporter modelImporter;

    private void OnPreprocessModel()
    {
        modelImporter = assetImporter as ModelImporter;
    }

    private static void OnPostprocessAllAssets(string[] importedAssets,
                                               string[] deletedAssets,
                                               string[] movedAssets,
                                               string[] movedFromAssetPaths,
                                               bool didDomainReload)
    {
        if (processModel)
        {
            if (importedAssets.Length > 0)
            {
                var materialsPath = Path.Combine(assetsFolder, materialsFolder);
                var texturesPath = Path.Combine(assetsFolder, texturesFolder);

                foreach (var asset in importedAssets)
                {
                    var success = modelImporter.ExtractTextures(asset);
                    if (!success)
                    {
                        Debug.Log("failed to extract textures");
                    }
                    else
                    {
                        Debug.Log("textures extracted successfully");
                    }

                    ExtractMaterialsFromAsset(asset, materialsPath);
                }
            }
            processModel = false;
        }
    }

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
