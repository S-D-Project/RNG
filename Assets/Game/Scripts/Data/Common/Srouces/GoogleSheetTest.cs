using UnityEngine;

public sealed class GoogleSheetTest : MonoBehaviour
{
    [SerializeField]
    private  string ApiKey;

    [SerializeField]
    private  string SpreadSheetId;

    private const string Range =
        "WeaponData!A:Z";

    private async void Start()
    {
        var client = new GoogleSheetClient(ApiKey);

        string json = await client.GetValueAsync(
            SpreadSheetId,
            Range);

        Debug.Log(json);
    }
}