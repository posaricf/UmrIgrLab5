using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform _list;
    [SerializeField] GameObject _scoreboardIetmPrefab;
    private Dictionary<Player, ScoreboardItem> _playersItems = new Dictionary<Player, ScoreboardItem>();

    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddPlayer(player);
        }
    }

    private void AddPlayer(Player player)
    {
        ScoreboardItem si = Instantiate(_scoreboardIetmPrefab, _list).GetComponent<ScoreboardItem>();
        si.Init(player);
        _playersItems[player] = si;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //base.OnPlayerEnteredRoom(newPlayer);
        AddPlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //base.OnPlayerLeftRoom(otherPlayer);
        RemovePlayer(otherPlayer);
    }

    private void RemovePlayer(Player player)
    {
        Destroy(_playersItems[player].gameObject);
        _playersItems.Remove(player);
    }

    public void PlayerDied(Player player)
    {
        _playersItems[player].UpdateDeaths();
    }

    public void PlayerKills(Player player)
    {
        _playersItems[player].UpdateKills();
    }
}
