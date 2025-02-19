using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;


public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance;
    public Dictionary<string, Item> Items = new();
    public Item[] startingItems;
    public Transform ItemContent;
    private GameObject moveableItem;
    public GameObject ItemSelected;
    public Transform selectedItemParent;
    public Camera itemCamera;
    public bool isItemSelected = false;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        foreach (Item item in startingItems) {
            Items.Add(item.data.name, item);
        }
        ListItems();
    }

    public void ShowInventory() {
        ItemContent.gameObject.SetActive(true);
    }

    public void HideInventory() {
        ItemContent.gameObject.SetActive(false);
    }

    public Item GetItem(int index) {
        if (Items.Count > index) {
            var enumerator = Items.Values.GetEnumerator();
            for (int i = 0; i <= index; i++) {
                enumerator.MoveNext();
            }
            Item entry = enumerator.Current;
            return entry;
        }

        return null;
    }

    public void HandleInventory() {
        if (isItemSelected) {
            ItemData itemData = ItemSelected.GetComponent<Item>()?.data ?? ItemSelected.GetComponent<Moveable>()?.GetItemData();
            ItemSelected.transform.position = GetMouseScreenPosition();

            DropZone dropZone = PlayerManager.Instance.playerInteraction.RaycastForDropZone();
            if (dropZone != null) {
                dropZone.OnHoverWithItem(itemData);
            }


            if (Input.GetMouseButtonDown(0)) {
                PlayerManager.Instance.playerInteraction.TryDragAndDrop(itemData);
                ClearSelection();
            }

        }
    }

    public void Add(Item item) {
        Items.Add(item.data.name, item);
        UIManager.Instance.ShowInfo(item);
        ListItems();
    }

    public void SelectItemFromInventorySlot(int index) {
        // if the index is valid (an actual item is present) instantiate the item as the selcted
        if (Items.Count > index) {
            var enumerator = Items.Values.GetEnumerator();
            for (int i = 0; i <= index; i++) {
                enumerator.MoveNext();
            }
            Item entry = enumerator.Current;

            CreateSelectedItem(entry.gameObject);
        }
    }

    public void SelectItem(GameObject itemObject) {
        moveableItem = itemObject;
        itemObject.SetActive(false);

        CreateSelectedItem(itemObject);
        ItemSelected.GetComponent<Animator>().enabled = false;
    }

    public void CreateSelectedItem(GameObject gameObject) {
        ItemSelected = Instantiate(gameObject, GetMouseScreenPosition(), gameObject.transform.rotation, selectedItemParent);
        ItemSelected.transform.localScale *= 10f;
        ItemSelected.layer = 5; // UI layer
        foreach (Transform child in ItemSelected.transform) {
            child.gameObject.layer = 5;
        }
        ItemSelected.SetActive(true);
        isItemSelected = true;
        CursorManager.Instance.PointingMoveableWithItem();
    }

    public void ClearSelection() {
        if (moveableItem) {
            moveableItem.GetComponent<Moveable>().Restore();
            moveableItem = null;
        }
        Destroy(ItemSelected);
        isItemSelected = false;
        CursorManager.Instance.PointingDefault();
    }

    Vector3 GetMouseScreenPosition() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10;
        return itemCamera.ScreenToWorldPoint(mousePosition);
    }


    public void ListItems() {
        var enumerator = Items.Values.GetEnumerator();
        int itemCount = 0;

        for (int i = 0; i < ItemContent.childCount; i++) {
            Transform obj = ItemContent.GetChild(i);
            var ItemIcon = obj.transform.Find("Border/ItemIcon").GetComponent<Image>();

            if (enumerator.MoveNext()) {
                Item entry = enumerator.Current;
                ItemIcon.color = new Color(ItemIcon.color.r, ItemIcon.color.g, ItemIcon.color.b, 255);
                ItemIcon.sprite = entry.data.icon;
                itemCount++;
            }
            else {
                // Slot vuoto
                ItemIcon.sprite = null;
            }
        }

    
        if (itemCount == 9) {
            if (TutorialManager.Instance) {
                Debug.Log("shpara");
                TutorialManager.Instance.OnReceiveEvent("InventarioPieno");
            }
        }
    }


}