using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KDCounter : MonoBehaviour
{
    private int _kills = 0;
    private int _deaths = 0;
    [SerializeField] private TMP_Text _killsText;
    [SerializeField] private TMP_Text _deathsText;

    public void Deaths(Player player)
    {
        if (player.IsLocal)
        {
            _deaths++;
            _deathsText.text = _deaths.ToString();
        }
    }

    public void Kills(Player player)
    {
        if (player.IsLocal)
        {
            _kills++;
            _killsText.text = _kills.ToString();
        }
    }
}
