using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    //every gun will leave an impact where the bullet hits
    public GameObject _bulletHitPrefab;
    public abstract override void Use();
}
