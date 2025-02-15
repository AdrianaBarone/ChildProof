
using System.Collections;
using UnityEngine;


public class Item : MonoBehaviour, IPickable {
    public ItemData data;
    [Header("Audio Animazione")]
    private AudioClip[] audioClips;
    public AudioSource[] audioSources;

    private void Start()
    {
        // Rimuove AudioSource esistenti per evitarne la duplicazione
        foreach (var source in GetComponents<AudioSource>())
        {
            Destroy(source);
        }

        audioSources = new AudioSource[audioClips.Length];

        for (int i = 0; i < audioClips.Length; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].clip = audioClips[i];
        }

        if (audioClips.Length == 0)
        {
            Debug.LogWarning("Nessun audio clip assegnato.");
        }
    }

    public void OnPick() {
        InventoryManager.Instance.Add(this);
        // NOTE: non distruggo l'oggetto dato che serve il riferimento per l'inventario
        gameObject.SetActive(false);
    }
    public void PlayAudioOnAnimation()
    {
        foreach (var source in audioSources)
        {
            if (source.clip != null && !source.isPlaying)
            {
                source.Play();
            }
        }
    }
}
