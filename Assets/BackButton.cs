using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public TMP_Text theText;

    public void OnPointerEnter(PointerEventData eventData) {
        theText.color = new Color(0.078f, 0.278f, 0.467f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        theText.color = new Color(0.839f, 0.918f, 0.972f);
    }
}