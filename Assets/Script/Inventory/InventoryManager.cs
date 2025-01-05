using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance;
    public Dictionary<string, InventoryItem> Items = new();
    public Transform ItemContent;
    public GameObject ItemSelected;
    public Transform selectedItemParent;
    public Camera itemCamera;
    bool isItemSelected = false;
    [SerializeField] private PlayerInteraction playerInteraction;


    private void Awake() {
        Instance = this;
        ListItems();
    }

    void Update() {
        // if an item is selected make it follow the mouse mosition

        if (isItemSelected) {
            ItemSelected.transform.position = GetMouseScreenPosition();


            if (Input.GetMouseButtonDown(0)) {
                if (playerInteraction.TryDragAndDrop(ItemSelected.GetComponent<Item>())){
                    Remove(ItemSelected.GetComponent<Item>());
                }

                Destroy(ItemSelected);
                isItemSelected = false;
            }

        }


    }

    public void Add(Item item) {
        if (Items.ContainsKey(item.data.name)) {
            Items[item.data.name].quantity += 1;
        }
        else {
            Items.Add(item.data.name, new InventoryItem { item = item, quantity = 1 });
        }

        ListItems();
    }

    public void Remove(Item item) {
        if (Items.ContainsKey(item.data.name)) {
            Items[item.data.name].quantity -= 1;
            if (Items[item.data.name].quantity <= 0) {
                Items.Remove(item.data.name);
            }
        }
        ListItems();
    }

    public void SelectItem(int index) {
        // if the index is valid (an actual item is present) instantiate the item as the selcted
        if (Items.Count > index) {
            var enumerator = Items.Values.GetEnumerator();
            for (int i = 0; i <= index; i++) {
                enumerator.MoveNext();
            }
            InventoryItem entry = enumerator.Current;

            ItemSelected = Instantiate(entry.item.gameObject, GetMouseScreenPosition(), Quaternion.identity, selectedItemParent);
            ItemSelected.layer = 5; // UI layer
            ItemSelected.SetActive(true);
            isItemSelected = true;
        }
    }

    Vector3 GetMouseScreenPosition() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10;
        return itemCamera.ScreenToWorldPoint(mousePosition);
    }


    public void ListItems() {
        var enumerator = Items.Values.GetEnumerator();

        for (int i = 0; i < ItemContent.childCount; i++) {
            Transform obj = ItemContent.GetChild(i);
            var ItemIcon = obj.transform.Find("Border/ItemIcon").GetComponent<Image>();
            var ItemQuantity = obj.transform.Find("Border/ItemQuantity").GetComponent<Text>();

            if (enumerator.MoveNext()) {
                InventoryItem entry = enumerator.Current;
                ItemIcon.sprite = entry.item.data.icon;
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