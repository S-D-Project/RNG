using UnityEngine;

public static class HitRadiusUtility
{
    public static float CalculateMax(
        SpriteRenderer spriteRenderer,
        float ratio = 1f)
    {
        Vector2 size = GetWorldSize(spriteRenderer);

        float diameter =
            Mathf.Max(size.x, size.y);

        return diameter * 0.5f * ratio;
    }

    public static float CalculateWidth(
        SpriteRenderer spriteRenderer,
        float ratio = 1f)
    {
        Vector2 size = GetWorldSize(spriteRenderer);

        return size.x * 0.5f * ratio;
    }

    private static Vector2 GetWorldSize(
        SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null ||
            spriteRenderer.sprite == null)
        {
            return Vector2.zero;
        }

        Vector2 spriteSize =
            spriteRenderer.sprite.bounds.size;

        Vector3 scale =
            spriteRenderer.transform.lossyScale;

        return new Vector2(
            spriteSize.x * Mathf.Abs(scale.x),
            spriteSize.y * Mathf.Abs(scale.y));
    }
}