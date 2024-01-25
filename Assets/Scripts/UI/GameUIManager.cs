using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _endCanvas;
    [SerializeField] private GameObject _gameMenuCanvas;
    [SerializeField] private CanvasGroup _gameCanvas;
    [SerializeField] private GameObject _buttons;

    private PhotonView _playerPV;
    private bool _endGame;

    private void Awake()
    {
        CursorLock(true);
        _endCanvas.alpha = 0;
        _gameMenuCanvas.SetActive(false);
        _gameCanvas.gameObject.SetActive(true);
        _endGame = false;
        _buttons.SetActive(false);
    }

    private void Update()
    {
        if (!_endGame && Input.GetKeyUp(KeyCode.Escape))
        {
            if (_gameMenuCanvas.activeInHierarchy)
            {
                //close canvass
                _gameMenuCanvas.SetActive(false);
                CursorLock(true);
            }
            else
            {
                //open canvas
                _endCanvas.GetComponent<CanvasGroup>().alpha = 0;
                _gameMenuCanvas.SetActive(true);
                CursorLock(false);
            }
        }
    }

    private void CursorLock(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void onContinueButtonPressed()
    {
        _gameMenuCanvas.SetActive(false);
        CursorLock(true);
    }

    public void onReturnFromGameButtonPressed()
    {
        PhotonView.Find((int)_playerPV.InstantiationData[0]).GetComponent<PlayerManager>().DieAndLeave();
        Destroy(WorldManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }

    public void onExitButtonPressed()
    {
        Application.Quit();
    }

    public void SetPlayerPV(PhotonView pv)
    {
        _playerPV = pv;
    }

    public void EndGame()
    {
        _endGame = true;
        _gameCanvas.gameObject.SetActive(false);
        _endCanvas.alpha = 1;
        _buttons.SetActive(true);
        CursorLock(false);
    }
}
