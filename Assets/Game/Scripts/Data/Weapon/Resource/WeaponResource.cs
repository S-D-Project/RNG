using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Weapon Resource")]
public class WeaponResource : ScriptableObject
{
    [Title("Common Resource")]
    [SerializeField]
    private string _id;

    [SerializeField]
    private string _weaponName;

    [SerializeField]
    private Sprite _icon;

    [SerializeField]
    private WeaponType _weaponType;
        
    [Title("Type Resource")]
    [SerializeReference]
    private WeaponTypeResourceData _typeResource;
    

    [Title("Weapon Behaviour")]
    [SerializeReference]
    private FirePatternResourceData _firePattern;
    [SerializeReference]
    private TargetingResourceData _targetingResource;



    public string Id => _id;
    public string WeaponName => _weaponName;
    public Sprite Icon => _icon;
    public WeaponType WeaponType => _weaponType;
    
    public WeaponTypeResourceData TypeResource => _typeResource;
    public FirePatternResourceData FirePattern => _firePattern;
    
    public TargetingResourceData TargetingResource => _targetingResource;

    /**
     * Editor/생성용 초기화 메서드
     */
    public void Initialize(
        string id,
        string weaponName,
        Sprite icon,
        WeaponType weaponType,
        FirePatternResourceData firePattern,
        TargetingResourceData targeting,
        WeaponTypeResourceData typeResource)
    {
        _id = id;
        _weaponName = weaponName;
        _icon = icon;
        _weaponType = weaponType;
        _firePattern = firePattern;
        _targetingResource = targeting;
        _typeResource = typeResource;
    }
}