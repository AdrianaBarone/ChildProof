using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : Interactable
{
    public float jumpHeight = 1f; 
    public float jumpDuration = 2f; 
    private Vector3 initialPosition; 
    private bool isJumping = false; 
    private float jumpStartTime; 

    void Start()
    {
        initialPosition = transform.position; 
    }

    protected override void Interact()
    {
        if (!isJumping)
        {
            isJumping = true;
            jumpStartTime = Time.time;
        }
    }

    void Update()
    {
        if (isJumping)
        {
            float progress = (Time.time - jumpStartTime) / jumpDuration;

            if (progress < 0.5f)
            {
                float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
                transform.position = new Vector3(transform.position.x, initialPosition.y + height, transform.position.z);
            }
            else
            {
                float height = Mathf.Sin((1 - progress) * Mathf.PI) * jumpHeight;
                transform.position = new Vector3(transform.position.x, initialPosition.y + height, transform.position.z);
            }

            if (progress >= 1)
            {
                transform.position = initialPosition;
                isJumping = false;
            }
        }
    }
}
