using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class WorldManager : MonoBehaviourPunCallbacks
{
    public static WorldManager Instance;

    /*
     * U ovoj skripti potrebno je dodati po jednu liniju koda u OnEnable i OnDisable metode.
     * Ta linija koda oznacava da se svaki put prilikom ucitavanja scene treba pozvati metoda OnSceneLoaded.
     * Nakon toga, u metodi OnSceneLoaded potrebno je provjeriti da li je ucitana scena za igru i ako je, pomocu
     * Photona instancirati prefab PlayerManager. PlayerManager nalazi se u direktoriju Resources posto je to 
     * direktorij iz kojega Photon  instancira objekte koji se sinkroniziraju preko mreze. Proucite na koji nacin
     * se referenciraju objekti koji se nalaze u poddirektorijima direktorija Resources.
     */

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name != "Game")
        {
            return;
        }
        PhotonNetwork.Instantiate("NetworkedPrefabs/PlayerManager", Vector3.zero, Quaternion.identity);
    }

}
