using UnityEngine;
using System;

[Serializable]
public class MeleeResourceData : WeaponTypeResourceData
{
    [SerializeField]
    private GameObject _meleePrefab;
    public GameObject MeleePrefab => _meleePrefab;
}