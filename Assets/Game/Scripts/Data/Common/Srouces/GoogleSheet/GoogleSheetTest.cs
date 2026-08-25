using System.Collections.Generic;
using UnityEngine;

public sealed class GoogleSheetTest : MonoBehaviour
{
    public string ApiKey;
    public string SpreadSheetId;
    
    private async void Start()
    {
        GoogleSheetClient client = new GoogleSheetClient(SpreadSheetId, ApiKey);
        
        
        DataResponse json = await client.LoadAsync();

        List<PlayerDto> players = SheetParser.Parse<PlayerDto>(json.Content, "PlayerData");

    }
}