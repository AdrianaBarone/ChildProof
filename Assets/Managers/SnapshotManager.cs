using UnityEngine;
using UnityEngine.Audio;

public enum SnapshotState {
    Player,
    SFX,
    Voice
}

public class SnapshotManager : MonoBehaviour
{
    [Header("Info Mixer Audio")]
    public AudioMixer audioMixer;
    public AudioMixerSnapshot sfx;
    public AudioMixerSnapshot player;
    public AudioMixerSnapshot voice;

    private SnapshotState currentState;

    private void Start() {
        // Impostiamo lo stato predefinito su "player" al start
        currentState = SnapshotState.Player;
        ChangeSnapshot(currentState, 0); // Iniziamo con lo snapshot "player"
    }

    // Metodo per cambiare snapshot
    public void ChangeSnapshot(SnapshotState state, float transitionTime) {
        switch (state) {
            case SnapshotState.Player:
                player.TransitionTo(transitionTime);
                Debug.Log($"Snapshot attivo: Player (transizione in {transitionTime}s)");
                break;
            case SnapshotState.SFX:
                sfx.TransitionTo(transitionTime);
                Debug.Log($"Snapshot attivo: SFX (transizione in {transitionTime}s)");
                break;
            case SnapshotState.Voice:
                voice.TransitionTo(transitionTime);
                Debug.Log($"Snapshot attivo: Voice (transizione in {transitionTime}s)");
                break;
            default:
                Debug.LogWarning("Snapshot non valido.");
                break;
        }

        // Impostiamo lo stato corrente
        currentState = state;
    }
}
