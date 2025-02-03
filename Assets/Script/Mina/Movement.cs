using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody rb;
    Vector2 rawRotation;
    Vector2 rotation;
    public Camera cam;
    public float lookSensitivity = 5f;
    public float moveSpeed = 2f;
    public float jumpStrength = 5f;

    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        Vector2 move = Vector2.zero;
        int w,a,s,d;
        if (Input.GetKey("w")) w = 1; else w = 0;
        if (Input.GetKey("a")) a = 1; else a = 0;
        if (Input.GetKey("s")) s = 1; else s = 0;
        if (Input.GetKey("d")) d = 1; else d = 0;

        move.y = w - s;
        move.x = d - a;

        rawRotation.y = Input.GetAxis("Mouse X");
        rawRotation.x = Input.GetAxis("Mouse Y");
        rotation = Vector2.Lerp(rotation, rawRotation, Time.deltaTime * 10f);
        cam.transform.eulerAngles = new Vector3(0, rotation.y * lookSensitivity);
        cam.transform.position = Vector3.Lerp(cam.transform.position, transform.position + new Vector3(0,1,0), 1f); //smooth cam

        transform.eulerAngles = new Vector2(0, rotation.y * lookSensitivity);
        Vector3 desiredVelocity = transform.TransformDirection(moveSpeed * new Vector3(move.x, 0, move.y));
        rb.linearVelocity = new Vector3(desiredVelocity.x * moveSpeed, rb.linearVelocity.y, desiredVelocity.z * moveSpeed);
        
        if (Input.GetButtonDown("Jump")) Jump();
    }

    void Jump(){
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 1.2f))
        {
            rb.AddForce(transform.up * jumpStrength, ForceMode.Impulse);
        }
    }
}
