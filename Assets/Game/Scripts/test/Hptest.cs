using Sirenix.OdinInspector;
using UnityEngine;

public class HpTest : MonoBehaviour
{
    [Title("HP Test")]
    [SerializeField, Min(0)]
    private int _currentHp = 100;

    [SerializeField, Min(1)]
    private int _damage = 10;

    [Title("Game Over")]
    [SerializeField, Required]
    private GameObject _gameOverUI;

    [Button(ButtonSizes.Large)]
    private void DamageTest()
    {
        if (_currentHp <= 0)
            return;

        _currentHp = Mathf.Max(0, _currentHp - _damage);

        Debug.Log($"Current HP : {_currentHp}");

        if (_currentHp == 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");

        if (_gameOverUI != null)
        {
            _gameOverUI.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}