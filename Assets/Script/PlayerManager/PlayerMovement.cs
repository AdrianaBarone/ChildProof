using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Parametri movimento FPS
    public Camera playerCamera;
    public float speed = 20f;
    public float sensitivity = 5f;
    private float rotationX = 0;

    // Parametro di movimento
    private bool canMove = true;

    void Start() {
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (canMove)
        {
            // Rotazione del mouse
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);

            // Movimento del giocatore
            Vector3 playerMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            transform.Translate(playerMovement * speed * Time.deltaTime);
        }
    }


    public void EnablePlayerMovement(bool canMove)
    {
        this.canMove = canMove;
    }
    
}
