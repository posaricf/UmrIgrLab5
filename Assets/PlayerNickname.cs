using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerNickname : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameInput;
    private string _defaultNickname;

    private void Awake()
    {
        PhotonNetwork.NickName = "Player" + Random.Range(0, 999).ToString();
    }
    public void ChangeUsername()
    {
        if (_usernameInput.text.Trim().ToString().Length < 1) PhotonNetwork.NickName = "Player" + Random.Range(0, 999).ToString();
        else PhotonNetwork.NickName = _usernameInput.text;
    }
}
