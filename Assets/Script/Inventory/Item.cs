using UnityEngine;


public class Item : MonoBehaviour, IPickable
{
    public ItemData data;

    public void OnPick()
    {
        InventoryManager.Instance.Add(this);
        if (TutorialManager.Instance && gameObject.tag == "Tutorial")
        {
            TutorialManager.Instance.OnReceiveEvent("RaccogliOggetto");
        }
        // NOTE: non distruggo l'oggetto dato che serve il riferimento per l'inventario
        gameObject.SetActive(false);
    }
}
