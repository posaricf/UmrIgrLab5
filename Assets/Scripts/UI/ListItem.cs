using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using Photon.Pun;

public class ListItem : MonoBehaviour
{
    //sets the text to the name of the room and when clicked, player should join the room
    [SerializeField] TMP_Text _roomName;
    public RoomInfo _roomInfo;

    public void SetUp(RoomInfo info)
    {
        _roomName.text = info.Name;
        _roomInfo = info;
    }

    public void onClick()
    {
        ConnectToServices.Instance.JoinRoom(_roomInfo);
    }
}
