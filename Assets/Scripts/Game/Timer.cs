using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviourPunCallbacks, IOnEventCallback
{

    [SerializeField] private TMP_Text _time;
    private int _matchLength = 240; //4 minutes
    private int _currentMatchTime;
    private Coroutine _timeCoroutine;


    public enum EventCodes : byte
    {
        RefreshTimer,
        EndMatch
    }

    private void Awake()
    {
        _currentMatchTime = _matchLength;
        RefreshTime();
        if (PhotonNetwork.IsMasterClient)
        {
            _timeCoroutine = StartCoroutine(Counter());
        }

    }


    private IEnumerator Counter()
    {
        yield return new WaitForSeconds(1f);
        _currentMatchTime--;
        if (_currentMatchTime <= 0)
        {
            _timeCoroutine = null;
            UpdateState_Send();
        }
        else
        {
            RefreshTime_Send();
            _timeCoroutine = StartCoroutine(Counter());
        }
    }

    private void RefreshTime()
    {
        string mins = (_currentMatchTime / 60).ToString("00");
        string secs = (_currentMatchTime % 60).ToString("00");
        _time.text = $"{mins}:{secs}";
    }

    private void UpdateState_Send()
    {
        object[] package = new object[] { _currentMatchTime };
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.EndMatch,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }

    public void RefreshTime_Send()
    {
        object[] timePackage = new object[] { _currentMatchTime };
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.RefreshTimer,
            timePackage,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }

    public void RefreshTime_Receive(object[] data)
    {
        _currentMatchTime = (int)data[0];
        RefreshTime();
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200) return;
        int code = (int)photonEvent.Code;
        object[] obj = (object[])photonEvent.CustomData;
        switch (code)
        {
            case ((int)EventCodes.RefreshTimer):
                RefreshTime_Receive(obj);
                break;
            case ((int)EventCodes.EndMatch):
                EndGame();
                break;
            default:
                break;

        }
    }

    private void EndGame()
    {
        gameObject.GetComponent<GameUIManager>().EndGame();
    }
}
