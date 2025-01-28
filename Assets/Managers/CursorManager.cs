using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour {
    private Image cursorImage;
    private Sprite defaultEXSprite;

    public Texture2D defaultInspectorCursor;
    public Texture2D movableHoverCursor;
    public Texture2D movableWithItemCursor;

    //public Texture2D dfaultINsprite;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public static CursorManager Instance;

    private void Awake() {
        Instance = this;
        defaultEXSprite = PlayerManager.Instance.playerInteraction.defaultSprite;
    }

    private void Start() {
        cursorImage = GetComponentInChildren<Image>();
    }

    public void ExplorationCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateExplorationCursor(defaultEXSprite);
    }

    public void InspectionCursor() {
        Cursor.lockState = CursorLockMode.Confined;
        cursorImage.enabled = false;

        Cursor.visible = true;
        Cursor.SetCursor(defaultInspectorCursor, hotSpot, cursorMode);
    }

    public void UpdateExplorationCursor(Sprite sprite) {
        if (cursorImage != null && sprite != null) {
            cursorImage.sprite = sprite;
            cursorImage.enabled = true;
        }
    }

    public void PointingMoveable() {
        Cursor.SetCursor(movableHoverCursor, hotSpot, cursorMode);
    }

    public void PointingMoveableWithItem() {
        Cursor.SetCursor(movableWithItemCursor, hotSpot, cursorMode);
    }

    public void PointingDefault() {
        Cursor.SetCursor(defaultInspectorCursor, hotSpot, cursorMode);
    }
}
