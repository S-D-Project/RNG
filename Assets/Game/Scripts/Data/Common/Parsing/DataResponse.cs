using Sirenix.Serialization;

public struct DataResponse
{
    public string Content { get; }
    public DataFormat Format { get; }

    public DataResponse(string content, DataFormat format)
    {
        Content = content;
        Format = format;
    }
}