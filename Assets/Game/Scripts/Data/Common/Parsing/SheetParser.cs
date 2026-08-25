using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;

public static class SheetParser 
{
    public static List<T> Parse<T>(
        string content,
        string sheetName)
        where T : new()
    {
        GoogleSheetResponse response =
            JsonConvert.DeserializeObject<GoogleSheetResponse>(content);
        

        var result = new List<T>();

        if (response?.valueRanges == null)
            return result;

        GoogleSheetValueRange sheet = null;

        foreach (GoogleSheetValueRange valueRange in response.valueRanges)
        {
            if (valueRange.range.StartsWith(sheetName + "!"))
            {
                sheet = valueRange;
                break;
            }
        }

        if (sheet?.values == null ||
            sheet.values.Count <= 1)
        {
            return result;
        }

        List<string> headers = sheet.values[0];

        Dictionary<string, FieldInfo> fields =
            GetFields<T>();

        // 첫 번째 행은 Header
        for (int rowIndex = 1;
             rowIndex < sheet.values.Count;
             rowIndex++)
        {
            List<string> row =
                sheet.values[rowIndex];

            T dto = new T();

            for (int columnIndex = 0;
                 columnIndex < headers.Count;
                 columnIndex++)
            {
                if (columnIndex >= row.Count)
                    continue;

                string header =
                    headers[columnIndex];

                if (!fields.TryGetValue(
                        header,
                        out FieldInfo field))
                {
                    continue;
                }

                string rawValue =
                    row[columnIndex];

                object value =
                    ConvertValue(
                        rawValue,
                        field.FieldType);

                field.SetValue(dto, value);
            }

            result.Add(dto);
        }

        return result;
    }

    private static Dictionary<string, FieldInfo>
        GetFields<T>()
    {
        var result =
            new Dictionary<string, FieldInfo>(
                StringComparer.OrdinalIgnoreCase);

        FieldInfo[] fields =
            typeof(T).GetFields(
                BindingFlags.Instance |
                BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
            result[field.Name] = field;
        }

        return result;
    }

    private static object ConvertValue(
        string value,
        Type targetType)
    {
        if (targetType == typeof(string))
            return value;

        if (targetType.IsEnum)
        {
            return Enum.Parse(
                targetType,
                value,
                true);
        }

        return Convert.ChangeType(
            value,
            targetType,
            CultureInfo.InvariantCulture);
    }
}