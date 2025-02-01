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

    public GameObject InfoArea;

    private void Awake() {
        Instance = this;
        ListItems();
        InfoArea.SetActive(false);
    }

    public void HandleInventory() {
        if (isItemSelected) {
            CursorManager.Instance.PointingMoveableWithItem();
            ItemData itemData = ItemSelected.GetComponent<Item>()?.data ?? ItemSelected.GetComponent<Moveable>()?.GetItemData();
            ItemSelected.transform.position = GetMouseScreenPosition();

            DropZone dropZone = PlayerManager.Instance.playerInteraction.RaycastForDropZone();
            if (dropZone != null) {
                // TODO: solo se la dropzone è del currentInspectable
                dropZone.OnHoverWithItem(itemData);
            }


            if (Input.GetMouseButtonDown(0)) {

                if (PlayerManager.Instance.playerInteraction.TryDragAndDrop(itemData)) {
                    if (!moveableItem) {
                        Remove(ItemSelected.GetComponent<Item>());
                    }
                }

                ClearSelection();
            }

        }
    }

    void Update() {
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
            CreateSelectedItem(entry.item.gameObject);
        }
    }

    public void SelectItem(GameObject itemObject) {
        moveableItem = itemObject;
        itemObject.SetActive(false);

        CreateSelectedItem(itemObject);
        ItemSelected.GetComponent<Animator>().enabled = false;
    }

    public void CreateSelectedItem(GameObject gameObject) {
        ItemSelected = Instantiate(gameObject, GetMouseScreenPosition(), Quaternion.identity, selectedItemParent);
        ItemSelected.transform.localScale *= 10f;
        ItemSelected.layer = 5; // UI layer
        foreach (Transform child in ItemSelected.transform) {
            child.gameObject.layer = 5;
        }
        ItemSelected.SetActive(true);
        isItemSelected = true;
    }

    public void ClearSelection() {
        if (moveableItem) {
            moveableItem.GetComponent<Moveable>().Restore();
            moveableItem = null;
        }
        Destroy(ItemSelected);
        isItemSelected = false;
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