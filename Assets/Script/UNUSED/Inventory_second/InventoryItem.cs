using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoyItem
{
    string Name {get; }
    Sprite Image {get; }  
    void OnPickUp(); 
}

public class InventoryEventArgs: EventArgs
{
    public InventoryEventArgs(IInventoyItem item)
    {
        Item = item;
    }

    public IInventoyItem Item;
}
