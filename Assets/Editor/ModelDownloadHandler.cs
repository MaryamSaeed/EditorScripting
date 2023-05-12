using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Extensions;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using UnityEditor;
using System;

public class ModelDownloadHandler
{
    static readonly HttpClient client = new HttpClient();

    private static string assetsFolder = "Assets";
    private static string modelsFolder = "Models";

    private static string projectDirectory;

    public static async Task GetApiResponse(string url)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responsebody = await response.Content.ReadAsStringAsync();
            Debug.Log(responsebody);
            //byte[] responsebody = await response.Content.ReadAsByteArrayAsync();
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message :{0} " + e.Message);
        }
    }

    public static async Task GetModel(string url)
    {
        if (string.IsNullOrEmpty(projectDirectory))
            projectDirectory = Directory.GetCurrentDirectory();
        var modlesFolderPath = Path.Combine(projectDirectory, assetsFolder, modelsFolder);
        if (!Directory.Exists(modlesFolderPath))
            AssetDatabase.CreateFolder(assetsFolder, modelsFolder);
        try
        {
            using Stream streamToReadFrom = await client.GetStreamAsync(url);
            string filename = "Model.fbx";
            string  destinationFolder = Path.Combine(projectDirectory, assetsFolder, modelsFolder,filename);
            using Stream streamToWriteTo = File.Open(destinationFolder, FileMode.Create);
            await streamToReadFrom.CopyToAsync(streamToWriteTo);
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message :{0} " + e.Message);
        }
    }
}
