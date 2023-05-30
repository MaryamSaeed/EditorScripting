using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using HttpClientProgress;
using System;

public class ModelDownloadHandler
{
    public static Action<float> UpdateProgressBar;
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

    public static async Task GetModel(string url, string destinationPath)
    {
        try
        {
            using Stream streamToReadFrom = await client.GetStreamAsync(url);
            string filename = "Modeltest.fbx";
            string filePath = Path.Combine(destinationPath,filename);
            using Stream streamToWriteTo = File.Open(filePath, FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite);
            await streamToReadFrom.CopyToAsync(streamToWriteTo);
            await Task.Delay(120000);
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message :{0} " + e.Message);
        }
    }

    public static async Task DownloadModel(string url, string destinationPath)
    {
        try
        {
            var progress = new Progress<float> ();
            progress.ProgressChanged += OnProgressChanged;

            await client.DownloadDataAsync(url, destinationPath, progress);
            await Task.Delay(5000);
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message :{0} " + e.Message);
        }
    }

    private static void OnProgressChanged(object sender, float progress)
    {
        // Do something with your progress
        Debug.Log(progress);
        UpdateProgressBar?.Invoke(progress);
    }
}
