using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour {
    [SerializeField] private Image cursorImage;
    [SerializeField] private Sprite defaultEXSprite;

    //public Texture2D dfaultINsprite;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public void ExplorationCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateExplorationCursor(defaultEXSprite);
    }

    public void InspectionCursor() {
        Cursor.lockState = CursorLockMode.Confined;
        cursorImage.enabled = false;

        Cursor.visible = true;
        Cursor.SetCursor(null, hotSpot, cursorMode);
    }

    public void UpdateExplorationCursor(Sprite sprite) {
        if (cursorImage != null && sprite != null) {
            cursorImage.sprite = sprite;
            cursorImage.enabled = true;
        }
    }
}
