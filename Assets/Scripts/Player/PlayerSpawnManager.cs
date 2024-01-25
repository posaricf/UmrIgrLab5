using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;
    Spawnpoint[] _spawnPoints;

    private void Awake()
    {
        Instance = this;
        _spawnPoints = GetComponentsInChildren<Spawnpoint>();
    }

    public Transform getSpawnPoint()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Length)].transform;
    }
}
