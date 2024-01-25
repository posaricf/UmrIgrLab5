using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    private Player _player;
    [SerializeField] private TMP_Text _text;

    public void SetUp(Player p)
    {
        _player = p;
        _text.text = p.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //check if this player left the room and remove him from the lsit if yes
        //base.OnPlayerLeftRoom(otherPlayer);
        if (otherPlayer.Equals(_player))
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        // base.OnLeftRoom();
        Destroy(gameObject);
    }
}
