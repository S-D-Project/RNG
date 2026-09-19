using System;
using Unity.VisualScripting;
using UnityEngine;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField]
    private UIButton _startButton;

    [SerializeField]
    private UIButton _settingsButton;

    [SerializeField]
    private UIButton _exitButton;

    private void OnEnable()
    {
        _startButton.Clicked += HandleStartClicked;
        _settingsButton.Clicked += HandleSettingsClicked;
        _exitButton.Clicked += HandleExitClicked;
    }

    private void OnDisable()
    {
        _startButton.Clicked -= HandleStartClicked;
        _settingsButton.Clicked -= HandleSettingsClicked;
        _exitButton.Clicked -= HandleExitClicked;
    }

    private void HandleStartClicked()
    {
        StartGame();
    }

    private void HandleSettingsClicked()
    {
        OpenSettings();
    }

    private void HandleExitClicked()
    {
        ExitGame();
    }

    private void StartGame()
    {
        
    }

    private void OpenSettings()
    {
        
    }

    private void ExitGame()
    {
        Application.Quit();
    }
    
}