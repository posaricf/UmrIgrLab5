using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : Gun
{
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Camera _camera;
    private Ray _shootingRay;
    private Vector3 _shootingDir = new Vector3(0.5f, 0.5f);
    private PhotonView _photonView;
    [SerializeField] private GameObject _bulletPrefab;

    private void Awake()
    {
        _photonView = this.GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        //shoot towards middle of the screen
        _shootingRay = _camera.ViewportPointToRay(_shootingDir);
        _shootingRay.origin = _camera.transform.position;
        _photonView.RPC("RPC_CreateBullet", RpcTarget.All);
        /*if (Physics.Raycast(_shootingRay, out RaycastHit hit))
        {
            hit.collider.gameObject.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)_itemInfo).damage, _photonView.Owner);
            _photonView.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }*/

    }

    /* [PunRPC]
     void RPC_Shoot(Vector3 hit, Vector3 hitNormal)
     {
         Collider[] colliders = Physics.OverlapSphere(hit, 0.2f);
         if (colliders.Length != 0)
         {
             GameObject bulletImp = Instantiate(_bulletHitPrefab, hit + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * _bulletHitPrefab.transform.rotation);
             bulletImp.transform.SetParent(colliders[0].transform);
             //destroy after 10 seconds
             Destroy(bulletImp, 10f);
         }
     }*/

    [PunRPC]
    void RPC_CreateBullet()
    {
        //Debug.Log("Bullet created! :");
        GameObject bullet = Instantiate(_bulletPrefab, _shootPoint.position, _shootPoint.rotation);
        bullet.GetComponent<Bullet>().SetInfo(((GunInfo)_itemInfo).damage, _photonView.Owner);
        Rigidbody brb = bullet.GetComponent<Rigidbody>();
        brb.AddRelativeForce(Vector3.forward * 3000 * Time.deltaTime, ForceMode.Impulse);
    }
}
