using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private float _damage;
    private Player _owner;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_owner.IsLocal) return;
        collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(_damage, _owner);
        Destroy(this.gameObject);
    }

    internal void SetInfo(float damage, Player owner)
    {
        _damage = damage;
        _owner = owner;
    }
}
