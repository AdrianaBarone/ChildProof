using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AudioClip hoverSound;
    private AudioSource audioSource;
    private bool hasPlayed = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!hasPlayed && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
            hasPlayed = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hasPlayed = false;
    }
}
