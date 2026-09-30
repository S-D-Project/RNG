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

    [SerializeField]
    private ConfirmPopup _confirmPopup;

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
        ConfirmPopup popup =
            PopupManager.Instance.Open(_confirmPopup);

        popup.SetAsyncData(
            title: "Start",
            message: "게임을 시작하시겠습니까?",
            okText: "확인",
            cancelText: "취소",
            onConfirm: () =>
                SceneFlowManager.Instance.LoadSceneAsync(
                    SceneNames.GameScene));
    }

    private void OpenSettings()
    {
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}