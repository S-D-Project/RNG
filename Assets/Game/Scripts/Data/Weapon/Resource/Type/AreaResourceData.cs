using System;
using UnityEngine;

[Serializable]
public class AreaResourceData : WeaponTypeResourceData
{
    [SerializeField]
    private GameObject _areaPrefab;
    public GameObject AreaPrefab => _areaPrefab;
}