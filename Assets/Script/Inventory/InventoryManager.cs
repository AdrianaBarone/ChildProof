using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance;
    public Dictionary<string, InventoryItem> Items = new Dictionary<string, InventoryItem>();
    public Transform ItemContent;


    private void Awake() {
        Instance = this;
        ListItems();
    }

    public void Add(Item item) {
        if (Items.ContainsKey(item.itemName)) {
            Items[item.itemName].quantity += 1;
        }
        else {
            Items.Add(item.itemName, new InventoryItem { item = item, quantity = 1 });
        }

        ListItems();
    }

    public void Remove(Item item) {
        if (Items.ContainsKey(item.itemName)) {
            Items[item.itemName].quantity -= 1;
            if (Items[item.itemName].quantity <= 0) {
                Items.Remove(item.itemName);
            }
        }
        ListItems();
    }


    public void ListItems() {
        var enumerator = Items.Values.GetEnumerator();

        for (int i = 0; i < ItemContent.childCount; i++) {
            Transform obj = ItemContent.GetChild(i);
            var ItemIcon = obj.transform.Find("Border/ItemIcon").GetComponent<Image>();
            var ItemQuantity = obj.transform.Find("Border/ItemQuantity").GetComponent<Text>();

            if (enumerator.MoveNext()) {
                InventoryItem entry = enumerator.Current;
                ItemIcon.sprite = entry.item.icon;
                ItemQuantity.text = "x" + entry.quantity.ToString();
            }
            else {
                // Slot vuoto
                ItemIcon.sprite = null;
                ItemQuantity.text = "";
            }
        }
    }
}

public class InventoryItem {
    public Item item;
    public int quantity;
}