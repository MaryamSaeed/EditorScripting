using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Extensions;
using System.Net.Http;
using System.Threading.Tasks;


public class ModelDownloadHandler
{
    static readonly HttpClient client = new HttpClient();

    public static async Task GetModel(string url)
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
            Debug.Log("Message :{0} "+ e.Message);
        }
    }
}
