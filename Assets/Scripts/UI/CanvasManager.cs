using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject _loadingMenu;
    [SerializeField] private GameObject _titleMenu;
    [SerializeField] private GameObject _createRoomMenu;
    [SerializeField] private GameObject _roomMenu;
    [SerializeField] private GameObject _errorMenu;
    [SerializeField] private GameObject _findRoomMenu;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_Text _roomName;

    private void Awake()
    {
        TurnOff();
        _titleMenu.SetActive(true);
    }
    public void OpenTitleScreen()
    {
        TurnOff();
        _titleMenu.SetActive(true);
    }
    public void OpenLoadScreen()
    {
        TurnOff();
        _loadingMenu.SetActive(true);
    }

    public void CreateRoom()
    {
        TurnOff();
        _createRoomMenu.SetActive(true);
    }

    public void EnterRoom()
    {
        TurnOff();
        _roomName.text = PhotonNetwork.CurrentRoom.Name;
        _roomMenu.SetActive(true);
    }

    public void ShowError(string errmsg)
    {
        TurnOff();
        _errorText.text = "Creation of room failed, error message : " + errmsg;
        _errorMenu.SetActive(true);
    }
    public void FindRoom()
    {
        TurnOff();
        _findRoomMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void TurnOff()
    {
        _loadingMenu.SetActive(false);
        _titleMenu.SetActive(false);
        _createRoomMenu.SetActive(false);
        _roomMenu.SetActive(false);
        _errorMenu.SetActive(false);
        _findRoomMenu.SetActive(false);
    }
}
