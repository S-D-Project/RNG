using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform _target;
    
    [SerializeField]
    private Vector3 _offset = new Vector3(0f,0f,-10f);
    
    [SerializeField]
    private float _followSpeed = 10f;

    private bool _isInitialized = false;
    public void Initialize(Transform target)
    {
        _target = target;
        _isInitialized = true;
    }
    
    private void LateUpdate()
    {
        if (!_isInitialized)
        {
            return;
        }

        Vector3 targetPosition = _target.position + _offset;

        float t = 1f - Mathf.Exp(
            -_followSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            t);
    }
}
