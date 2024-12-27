using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Pickable
{
    public Item Item;

    protected override void Pick()
    {
        ItemSettings.Instance.Add(Item);
        Destroy(gameObject);
    }
}
