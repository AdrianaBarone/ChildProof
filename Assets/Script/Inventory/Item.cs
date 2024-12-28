using UnityEngine;

//[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : MonoBehaviour, IPickable {
    

    public string itemName;
    public Sprite icon;
    // NOTE: qui tutti gli altri campi che possono essere utili

    public void OnPick() {
        InventoryManager.Instance.Add(this);
        Destroy(gameObject);
    }
}
