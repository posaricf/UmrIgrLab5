using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollider : MonoBehaviour
{
    private PlayerController _player;

    private void Awake()
    {
        _player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6) return;
        _player.OnFloor(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 6) return;
        _player.OnFloor(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != 6) return;
        _player.OnFloor(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 6) return;
        _player.OnFloor(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != 6) return;
        _player.OnFloor(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer != 6) return;
        _player.OnFloor(true);
    }
}
