using UnityEngine;

public class Moveable : Inspectable {
    [SerializeField] private ItemData itemData;
    public ItemData GetItemData() {
        return itemData;
    }

    public void Restore() {
        if (!IsResolved()) {
            gameObject.SetActive(true);
        }
    }


    public new void Resolve() {
        base.Resolve();
        gameObject.SetActive(false);
    }

}