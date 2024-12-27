using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Inventory: MonoBehaviour{
    private const int SLOTS = 9;
    private List <IInventoyItem> mItems = new List <IInventoyItem>();

    public event EventHandler<InventoryEventArgs> ItemAdded;

    public void addItem(IInventoyItem item)
    {
        if (mItems.Count < SLOTS)
        {
            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider.enabled)
            {
                collider.enabled = false;
                mItems.Add(item);
                item.OnPickUp();
            }
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item));
            }
        }
    }
}