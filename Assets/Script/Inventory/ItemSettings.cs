using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemSettings : MonoBehaviour
{
    public static ItemSettings Instance;
    public List<Item> Items = new List<Item>();
    public Transform ItemContent;
    public GameObject InventoryItem;

    private void Awake()
    {
        Instance = this;
    }

public void Add(Item item)
{
    foreach (Item i in Items)
    {
        if (i.id == item.id) 
        {
            i.quantity += 1;  
            return;
        }
    }

    Items.Add(item);
}

    public void Remove(Item item)
    {
        foreach (Item i in Items)
    {
        if (i.id == item.id) 
        {
            i.quantity -= 1;   
            return;
        }
    }
        Items.Remove(item);
    }
    

    public void ListItems()
    {
    // Pulisce l'inventario prima di ogni restart gioco
    foreach (Transform item in ItemContent)
    {
        Destroy(item.gameObject);
    }

    // Aggiungi gli oggetti all'inventario
    foreach (Item item in Items)
    {
        GameObject obj = Instantiate(InventoryItem, ItemContent);
        var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
        var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
        var itemQuantity = obj.transform.Find("ItemQuantity").GetComponent<Text>();

        itemName.text = item.itemName;
        itemIcon.sprite = item.icon;
        itemQuantity.text = "x" + item.quantity.ToString();
    }
    }
}