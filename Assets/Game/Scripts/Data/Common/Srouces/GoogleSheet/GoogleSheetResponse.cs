using System.Collections.Generic;

public sealed class GoogleSheetResponse
{
    public string spreadsheetId;
    public List<GoogleSheetValueRange> valueRanges;
}

public sealed class GoogleSheetValueRange
{
    public string range;
    public string majorDimension;
    public List<List<string>> values;
}