using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

public sealed class GoogleSheetClient : IDataSource
{
    private const string BaseUrl =
        "https://sheets.googleapis.com/v4/spreadsheets";

    private readonly string _apiKey;
    
    [Required]
    private string _spreadSheetId;
    
    private IReadOnlyList<string> _ranges;

    public GoogleSheetClient(string spreadSheetId,string apiKey)
    {
        _apiKey = apiKey;
        _spreadSheetId = spreadSheetId;
        _ranges = new[]
        {
            "PlayerData",
            "EnemyData",
            "WeaponData"
        };
    }

    public async Awaitable<DataResponse> LoadAsync()
    {
        string url = BuildBatchGetUrl(
            _spreadSheetId,
            _ranges);
        

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

        return new DataResponse(
            request.downloadHandler.text,
            DataFormat.Json);
    }

    private string BuildBatchGetUrl(
        string spreadSheetId,
        IReadOnlyList<string> ranges)
    {
        if (ranges == null || ranges.Count == 0)
        {
            throw new ArgumentException(
                "At least one range is required.",
                nameof(ranges));
        }

        var builder = new StringBuilder();

        builder.Append(BaseUrl);
        builder.Append('/');
        builder.Append(spreadSheetId);
        builder.Append("/values:batchGet?");

        for (int i = 0; i < ranges.Count; i++)
        {
            if (i > 0)
            {
                builder.Append('&');
            }

            builder.Append("ranges=");
            builder.Append(
                UnityWebRequest.EscapeURL(ranges[i]));
        }

        builder.Append("&key=");
        builder.Append(_apiKey);

        return builder.ToString();
    }
}