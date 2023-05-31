using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using HttpClientProgress;
using System;

public class ModelDownloadHandler
{
    public static Action<float> UpdateProgressBar;

    private static HttpClient client = new HttpClient();
    private static Progress<float>  progress = new Progress<float>();

    private static string assetsFolder = "Assets";
    private static string modelsFolder = "Models";
    private static string projectDirectory;

    /// <summary>
    /// Gets an API response to the API request given 
    /// </summary>
    /// <param name="api">API url</param>
    public static async Task GetApiResponse(string api)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync(api);
            response.EnsureSuccessStatusCode();
            string responsebody = await response.Content.ReadAsStringAsync();
            Debug.Log(responsebody);
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message :{0} " + e.Message);
        }
    }

    /// <summary>
    /// Downloads a model from url into destination path (this approach is not recommended)
    /// </summary>
    /// <param name="url">The model URL on server</param>
    /// <param name="destinationPath">The path to which the model is downloaded</param>
    public static async Task GetModel(string url, string destinationPath)
    {
        try
        {
            using Stream streamToReadFrom = await client.GetStreamAsync(url);
            string filename = "Modeltest.fbx";
            string filePath = Path.Combine(destinationPath,filename);
            using Stream streamToWriteTo = File.Open(filePath, FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite);
            await streamToReadFrom.CopyToAsync(streamToWriteTo);
            await Task.Delay(5000);
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message :{0} " + e.Message);
        }
    }

    /// <summary>
    /// Downloads the model from a url to the destination folder (best approach)
    /// </summary>
    /// <param name="url">Url to model on server</param>
    /// <param name="destinationPath">The download pth</param>
    public static async Task DownloadModel(string url, string destinationPath)
    {
        try
        {
            progress.ProgressChanged += OnProgressChanged;

            await client.DownloadDataAsync(url, destinationPath, progress);
            await Task.Delay(5000);
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message :{0} " + e.Message);
        }
        progress.ProgressChanged -= OnProgressChanged;
    }

    /// <summary>
    /// Download Progress reporting, invokes an event to notify subscribers about the download progress
    /// </summary>
    /// <param name="progress">progress value</param>
    private static void OnProgressChanged(object sender, float progress)
    {
        UpdateProgressBar?.Invoke(progress);
    }
}
