using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUsername : MonoBehaviour
{
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private TMP_Text _usernameText;
    private Camera _facingCamera;


    private void Start()
    {
        if (_photonView.IsMine) Destroy(_usernameText.gameObject);
        _usernameText.text = _photonView.Owner.NickName;
    }

    private void Update()
    {
        if (_facingCamera == null)
        {
            _facingCamera = FindObjectOfType<Camera>();
        }
        if (_facingCamera == null) return;
        transform.LookAt(_facingCamera.transform);
        //flip on vertical axis
        transform.Rotate(Vector3.up * 180);
    }
}
