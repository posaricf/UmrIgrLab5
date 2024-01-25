using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemInfo _itemInfo;
    public GameObject _itemGameObject;

    public abstract void Use();
}
