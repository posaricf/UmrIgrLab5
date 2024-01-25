using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardItem : MonoBehaviour
{
    [SerializeField] public TMP_Text _usernameText;
    [SerializeField] public TMP_Text _deathsText;
    [SerializeField] public TMP_Text _killsText;

    public void Init(Player player)
    {
        _usernameText.text = player.NickName;
    }

    public void UpdateKills()
    {
        _killsText.text = (int.Parse(_killsText.text) + 1).ToString();
    }

    public void UpdateDeaths()
    {
        _deathsText.text = (int.Parse(_deathsText.text) + 1).ToString();
    }
}
