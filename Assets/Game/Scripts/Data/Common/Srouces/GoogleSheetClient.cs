using System;
using UnityEngine;
using UnityEngine.Networking;

public sealed class GoogleSheetClient
{
    private const string BaseUrl =
        "https://sheets.googleapis.com/v4/spreadsheets";

    private readonly string _apiKey;

    public GoogleSheetClient(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Awaitable<string> GetValueAsync(
        string spreadSheetId,
        string range)
    {
        string encodedRange = UnityWebRequest.EscapeURL(range);

        string url =
            $"{BaseUrl}/{spreadSheetId}/values/{encodedRange}?key={_apiKey}";

        Debug.Log($"Google Sheets Request: {url}");

        using var request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            throw new Exception(
                $"Google Sheets API request failed. " +
                $"Status: {request.responseCode}, " +
                $"Error: {request.error}, " +
                $"Body: {request.downloadHandler.text}");
        }

        return request.downloadHandler.text;
    }
}