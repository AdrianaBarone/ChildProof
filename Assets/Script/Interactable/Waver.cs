using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waver : Interactable {
    public float swingAmplitude = 30f;
    public float swingSpeed = 1f;
    public float swingDuration = 2f;   // Durata totale dell'ondeggiamento (tempo per un ciclo completo)
    private Quaternion initialRotation;
    private bool isSwinging = false;
    private float swingStartTime;

    void Start() {
        initialRotation = transform.rotation;
    }

    protected override void Interact() {
        if (!isSwinging) {
            isSwinging = true;
            swingStartTime = Time.time;
        }
    }

    void Update() {
        if (isSwinging) {
            float progress = (Time.time - swingStartTime) / swingDuration;
            float rotationAmount = Mathf.Sin(progress * Mathf.PI) * swingAmplitude;
            transform.rotation = initialRotation * Quaternion.Euler(0, rotationAmount, 0);

            if (progress >= 1) {
                transform.rotation = initialRotation;
                isSwinging = false;
            }
        }
    }
}
