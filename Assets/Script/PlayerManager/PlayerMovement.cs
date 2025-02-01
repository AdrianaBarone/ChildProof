using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    // Parametri movimento FPS
    public Camera playerCamera;
    Rigidbody rb;
    public float speed;
    public float sensitivity;
    private float rotationX = 0;

    void Start() {
        playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public void HandleRotation() {
        // Rotazione del mouse
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void HandleMovement() {
        // Movimento del giocatore
        Vector3 playerMovement = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
        //rb.MovePosition(transform.position + playerMovement.normalized * speed * Time.fixedDeltaTime);
        rb.linearVelocity = playerMovement * speed;
    }
}
