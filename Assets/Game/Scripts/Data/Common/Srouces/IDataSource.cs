using UnityEngine;

public interface IDataSource
{
    Awaitable<DataResponse> LoadAsync();
}