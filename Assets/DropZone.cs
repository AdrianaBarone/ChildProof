using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour {
    // TO DO: definire l'achievement (nome) che viene sbloccato da questo successo

    [SerializeField] private ItemData acceptedItem;
    public Inspectable parentInspectable;
    bool animating = false;
    bool resolved = false;

    private void Start() {
        // TODO: reference to the parent
    }
    public bool AcceptsItem(ItemData itemData) {
        return (itemData == acceptedItem) && !resolved;
    }

    public void OnHoverWithItem(ItemData itemData) {
        // TODO: animazione di successo?
        if (AcceptsItem(itemData) && !animating && !resolved)
            StartCoroutine(ChangeColor(Color.yellow));
    }

    IEnumerator ChangeColor(Color color) {
        animating = true;
        Color oriignalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = color;
        yield return new WaitForSeconds(0.4f);
        if(!resolved)
            GetComponent<Renderer>().material.color = oriignalColor;
        animating = false;
    }

    public void OnDrop() {
        if (resolved) return;
        GetComponent<Renderer>().material.color = Color.green;
        // TODO: animazione di successo
        resolved = true;

        parentInspectable.Resolve();
    }
}
