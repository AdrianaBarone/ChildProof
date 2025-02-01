using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour {
    // TO DO: definire l'achievement (nome) che viene sbloccato da questo successo

    [SerializeField] private ItemData acceptedItem;
    public Inspectable parentInspectable;


    bool resolved = false;

    [SerializeField] ParticleSystem hoverWithCorrectItemParticles;

    public bool AcceptsItem(ItemData itemData) {
        return (itemData == acceptedItem) && !resolved;
    }

    public void OnHoverWithItem(ItemData itemData) {
        // TODO: solo se la dropzone è del currentInspectable
        if (parentInspectable == PlayerManager.Instance.currentInspectable) {
            return;
        }
        if (AcceptsItem(itemData) && !resolved) {
            hoverWithCorrectItemParticles.Play();
        }
    }

    void PlaySuccessAnimation() {
        GetComponent<Animator>().SetTrigger("resolveTrigger");
    }

    public void OnDrop() {
        if (resolved) return;

        PlaySuccessAnimation();
        resolved = true;

        parentInspectable.Resolve();
    }
}
