using System.Collections.Generic;
using UnityEngine;

public class WeaponDataLoader
{
    private readonly IDataSource _dataSource;

    public WeaponDataLoader(IDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Awaitable<List<WeaponData>> LoadAsync()
    {

        
        // Parse
        // WeaponData 생성

        return null;
    }
}