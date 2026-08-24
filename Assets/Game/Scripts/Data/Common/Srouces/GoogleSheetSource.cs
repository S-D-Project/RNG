using System;
using UnityEditor;
using UnityEngine;

public class GoogleSheetSource : IDataSource
{
    private const string SpreadSheedId =
        "1bwBrkjXtTnLAf131f9p6TrfsQ5rUeQWRFEiUsJsIuho/edit?gid=678228494#gid=678228494";
    
        


    public Awaitable<DataResponse> LoadAsync(string key)
    {
        // Key를 기반으로 Google Sheet URL 결정
        // UnityWebRequest 실행
        // raw CSV 반환


        throw new Exception();
    }
}