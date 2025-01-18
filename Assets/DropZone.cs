using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour {
// TO DO: definire l'achievement (nome) che viene sbloccato da questo successo

    [SerializeField] private ItemData acceptedItem;
    public Inspectable parentInspectable;

    private void Start() {
        // TODO: reference to the parent
    }
    public bool AcceptsItem(ItemData itemData) {
        return itemData  == acceptedItem;
    }

    public void OnDrop() {
        GetComponent<Renderer>().material.color = Color.green;
        // TODO: animazione di successo

        parentInspectable.Resolve();
    }
}
