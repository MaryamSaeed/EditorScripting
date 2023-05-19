using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using UnityEditor;

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

    public static async Task GetModel(string url, string destinationPath)
    {
        try
        {
            using Stream streamToReadFrom = await client.GetStreamAsync(url);
            string filename = "Modeltest.fbx";
            string filePath = Path.Combine(destinationPath,filename);
            using Stream streamToWriteTo = File.Open(filePath, FileMode.Create);
            await streamToReadFrom.CopyToAsync(streamToWriteTo);
            await Task.Delay(120000);
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message :{0} " + e.Message);
        }
    }
}
