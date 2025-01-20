using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour {
    // TO DO: definire l'achievement (nome) che viene sbloccato da questo successo

    [SerializeField] private ItemData acceptedItem;
    public Inspectable parentInspectable;

    bool resolved = false;

    ParticleSystem hoverWithCorrectItemParticles;

    private void Start() {
        // TODO: reference to the parent
        hoverWithCorrectItemParticles = GetComponentInChildren<ParticleSystem>();
    }
    public bool AcceptsItem(ItemData itemData) {
        return (itemData == acceptedItem) && !resolved;
    }

    public void OnHoverWithItem(ItemData itemData) {
        // TODO: animazione di successo?
        if (AcceptsItem(itemData) && !resolved) {
            hoverWithCorrectItemParticles.Play();
        }
    }

    public void OnDrop() {
        if (resolved) return;
        GetComponent<Renderer>().material.color = Color.green;
        // TODO: animazione di successo
        resolved = true;

        parentInspectable.Resolve();
    }
}
