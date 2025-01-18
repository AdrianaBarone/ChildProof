using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance;
    public Dictionary<string, InventoryItem> Items = new();
    public Transform ItemContent;
    private GameObject moveableItem;
    public GameObject ItemSelected;
    public Transform selectedItemParent;
    public Camera itemCamera;
    bool isItemSelected = false;
    [SerializeField] private PlayerInteraction playerInteraction;

    public GameObject InfoArea;

    private void Awake() {
        Instance = this;
        ListItems();
        InfoArea.SetActive(false);
    }

    void Update() {
        // if an item is selected make it follow the mouse mosition

        if (isItemSelected) {
            ItemData itemData = ItemSelected.GetComponent<Item>()?.data ?? ItemSelected.GetComponent<Moveable>()?.GetItemData();
            ItemSelected.transform.position = GetMouseScreenPosition();

            DropZone dropZone = playerInteraction.RaycastForDropZone();
            if (dropZone != null) {
                dropZone.OnHoverWithItem(itemData);
            }


            if (Input.GetMouseButtonDown(0)) {
                
                if (playerInteraction.TryDragAndDrop(itemData)) {
                    if(!moveableItem) {
                        Remove(ItemSelected.GetComponent<Item>());
                    }
                }

                if (moveableItem){
                    moveableItem.GetComponent<Moveable>().Restore();
                    moveableItem = null;
                }
                Destroy(ItemSelected);
                isItemSelected = false;
            }

        }

        if (Input.GetKeyDown(KeyCode.P)) {
            if (InfoArea.activeSelf) {
                CloseInfoPanel();
            }
        }
    }

    public void Add(Item item) {
        if (Items.ContainsKey(item.data.name)) {
            Items[item.data.name].quantity += 1;
        }
        else {
            Items.Add(item.data.name, new InventoryItem { item = item, quantity = 1 });
            ShowInfo(item);
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

    public void SelectItemFromInventorySlot(int index) {
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

    // TODO: se è selezionato un oggetto e si clicca su un altro aoggetto dell'inventario, sostituire l'oggetto selezionato

    public void SelectItem(GameObject itemObject) {
        moveableItem = itemObject;
        itemObject.SetActive(false);
        
        ItemSelected = Instantiate(itemObject, GetMouseScreenPosition(), Quaternion.identity, selectedItemParent);
        ItemSelected.layer = 5; // UI layer
        ItemSelected.SetActive(true);
        isItemSelected = true;
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

    public void ShowInfo(Item item) {
        Time.timeScale = 0;
        var itemNameText = InfoArea.transform.Find("InfoPanel/NamePanel/Name").GetComponent<Text>();
        var itemDescriptionText = InfoArea.transform.Find("InfoPanel/DescriptionPanel/Description").GetComponent<Text>();
        var itemImage = InfoArea.transform.Find("InfoPanel/NamePanel/Image").GetComponent<Image>();

        itemNameText.text = item.data.name;
        itemDescriptionText.text = item.data.description;
        itemImage.sprite = item.data.icon;

        InfoArea.SetActive(true);
    }

    public void CloseInfoPanel() {
        // Nascondi il pannello e riprendi il gioco
        Time.timeScale = 1;
        InfoArea.SetActive(false);
    }
}

public class InventoryItem {
    public Item item;
    public int quantity;
}