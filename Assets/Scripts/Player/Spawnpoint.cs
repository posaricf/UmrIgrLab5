using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [SerializeField] private GameObject _visuals;

    private void Awake()
    {
        _visuals.SetActive(false);
    }

}
