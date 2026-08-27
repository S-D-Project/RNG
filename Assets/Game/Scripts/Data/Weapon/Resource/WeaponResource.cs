using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game/Data/Weapon Resource")]
public class WeaponResource : ScriptableObject
{
    [Title("Common Resource")]
    [SerializeField]
    private string _id;

    [SerializeField]
    private string _weaponName;

    [SerializeField]
    private GameObject _weaponObjectPrefab;

    [SerializeField]
    private Sprite _icon;

    [SerializeField]
    private WeaponType _weaponType;
    
    [Title("Type Resource")]
    [SerializeReference]
    private WeaponResourceData _weaponResourceData;
    

    [Title("Weapon Behaviour")]
    [SerializeReference]
    private FirePatternResourceData _firePattern;
    [SerializeReference]
    private TargetingResourceData _targeting;



    public string Id => _id;
    public string WeaponName => _weaponName;
    public GameObject WeaponObjectPrefab => _weaponObjectPrefab;
    public Sprite Icon => _icon;
    
    public WeaponType WeaponType => _weaponType;
    
    public WeaponResourceData WeaponResourceData => _weaponResourceData;
    public FirePatternResourceData FirePattern => _firePattern;
    
    public TargetingResourceData Targeting => _targeting;

    /**
     * Editor/생성용 초기화 메서드
     */
    public void Initialize(
        string id,
        string weaponName,
        GameObject weaponObjectPrefab,
        Sprite icon,
        WeaponType weaponType,
        FirePatternResourceData firePattern,
        TargetingResourceData targeting,
        WeaponResourceData weaponResourceData)
    {
        _id = id;
        _weaponName = weaponName;
        _weaponObjectPrefab = weaponObjectPrefab;
        _icon = icon;
        _weaponType = weaponType;
        _firePattern = firePattern;
        _targeting = targeting;
        _weaponResourceData = weaponResourceData;
    }
}