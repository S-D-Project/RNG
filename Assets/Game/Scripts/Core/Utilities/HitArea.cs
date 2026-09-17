using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HitArea : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    [Min(0f)]
    private float _hitRadius;

    public float HitRadius => _hitRadius;


    [Button]
    private void FitToSprite()
    {
        if (_spriteRenderer == null ||
            _spriteRenderer.sprite == null)
        {
            return;
        }

        Vector2 spriteSize =
            _spriteRenderer.sprite.bounds.size;

        Vector3 scale =
            _spriteRenderer.transform.lossyScale;

        float width =
            spriteSize.x * Mathf.Abs(scale.x);

        float height =
            spriteSize.y * Mathf.Abs(scale.y);

        _hitRadius =
            Mathf.Max(width, height) * 0.5f;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_hitRadius <= 0f)
        {
            return;
        }

        Handles.DrawWireDisc(
            transform.position,
            Vector3.forward,
            _hitRadius);
    }
#endif
}