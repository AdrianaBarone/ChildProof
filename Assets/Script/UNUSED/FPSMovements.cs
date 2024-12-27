/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    // Parametri movimento FPS
    public Camera playerCamera;
    public float speed = 20f;
    public float sensitivity = 5f;
    private float rotationX = 0;

    // Parametri per il controllo visibilità
    [SerializeField] private float distance = 2f;
    [SerializeField] private LayerMask layerMask;

    // Parametri cursore
    [SerializeField] private Image cursorImage;
    [SerializeField] private Sprite defaultCursor;
    [SerializeField] private Sprite interactCursor;
    [SerializeField] private Sprite grabbableCursor;

    // Oggetto Interactable
    private Interactable pointingInteractable;
    private Draggable pointingGrabbable;

    //Parametro di movimento
    private bool canMove = true;

    void Start() {
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

        // Raycast per l'interazione
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hitInfo;
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red); // Visualizza il raycast in scena

        if (Physics.Raycast(ray, out hitInfo, distance, layerMask)) {
            pointingInteractable = hitInfo.collider.GetComponent<Interactable>();
            pointingGrabbable = hitInfo.collider.GetComponent<Draggable>();
            if (pointingInteractable) {
                cursorImage.sprite = interactCursor; 

                if (Input.GetMouseButtonDown(0)) {
                    pointingInteractable.BaseInteract();
                }
            }
            else {
                cursorImage.sprite = defaultCursor;
            }
        }
        else {
            cursorImage.sprite = defaultCursor;
        }
        }
    }

    public void EnablePlayerMovement(bool canMove)
    {
        this.canMove = canMove;
    }
}
*/
