using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    private PhotonView _photonView;
    private GameObject _player;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        /*
         * Za svakog igraca stvara se objekt kojega on moze kontrolirati. No, u sceni ce se nalaziti vise
         * igraca, a trenutni korisnik smije imati kontrolu samo nad svojim objektom. Potrebno je provjeriti
         * da li je trenutni PlayerManager u vlasnistvu trenutnog korisnika, a ako je onda pozvati metodu 
         * CreateController koja ce stvoriti upravo taj objekt. 
         */
        if (_photonView.IsMine)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        //create player controller
        Transform spawnPoint = PlayerSpawnManager.Instance.getSpawnPoint();
        //ovdje je samo potrebno putem mreze instancirati prefab Player koristeci poziciju u rotaciju dohvacenog mjesta stvaranja (spawnPoint), za grupu se postavlja 0, a podaci koji se proslijeduju je viewID svojstvo _photonViewa. U ovu svrhu se koristi  metoda PhotonNetwork.Instantiate
        object[] photonData =
        {
            _photonView.ViewID
        };
        _player = PhotonNetwork.Instantiate("NetworkedPrefabs/Player", spawnPoint.position, spawnPoint.rotation, 0, photonData);
    }

    public void Die()
    {
        PhotonNetwork.Destroy(_player);
        CreateController();
    }

    public void DieAndLeave()
    {
        PhotonNetwork.Destroy(_player);
    }
}
