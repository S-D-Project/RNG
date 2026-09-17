using System;

public class EnemyData
{
    public string Id { get; }
    public float MaxHp { get; }
    public float MoveSpeed { get; }
    public float ContactDamage { get; }
    public EnemyGrade Grade { get; }
    
    public EnemyResource Resource { get; }
    
    public float HitRadius { get; private set; }

    public EnemyData(EnemyDto dto, EnemyResource resource)
    {
        Validate(dto,resource);
        Id = dto.Id;

        MaxHp = dto.MaxHp;
        MoveSpeed = dto.MoveSpeed;
        ContactDamage = dto.ContactDamage;
        Grade = dto.Grade;
        
        Resource = resource;
    }

    private void Validate(EnemyDto dto, EnemyResource resource)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }
        
        if(resource == null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        if (dto.Id != resource.Id)
        {
            throw new ArgumentException($"Enemy Id missmatch. Dto : {dto.Id}, Resource : {resource.Id}");
        }
    }

    public void SetRadius(float radius)
    {
        HitRadius = radius;
    }
}