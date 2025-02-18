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
        currentState = SnapshotState.Player;
        ChangeSnapshot(currentState, 0);
    }

    public void ChangeSnapshot(SnapshotState state, float transitionTime) {
        switch (state) {
            case SnapshotState.Player:
                player.TransitionTo(transitionTime);
                break;
            case SnapshotState.SFX:
                sfx.TransitionTo(transitionTime);
                break;
            case SnapshotState.Voice:
                voice.TransitionTo(transitionTime);
                break;
            default:
                Debug.LogWarning("Snapshot non valido.");
                return;
        }
        currentState = state;
    }

    public SnapshotState GetCurrentSnapshot() {
        return currentState;
    }
}
