using UnityEngine;

/**
 * 서버에서 가져온 데이터를 넣을 곳 (PlayerResource + PlayerDto)
 */
public class PlayerData
{
    public string Id { get; }
    public float MaxHp { get; }
    public float MoveSpeed { get; }
    
    public GameObject Prefab { get; }
    public Sprite Icon { get; }

    public PlayerData(PlayerDto playerDto, PlayerResource playerResource)
    {
        Id = playerDto.Id;
        MaxHp = playerDto.MaxHp;
        MoveSpeed = playerDto.MoveSpeed;
        
        Prefab = playerResource.Prefab;
        Icon = playerResource.Icon;
        
    }
}