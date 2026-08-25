using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private Transform _spawnPoint;

    public GameObject SpawnPlayer()
    {
        Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : Vector3.zero;
        
        return Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);
    }

    public GameObject SpawnPlayer(Vector3 position)
    {
        Vector3 spawnPosition = new Vector3(position.x, position.y, 0f);
        return Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);
    }
}