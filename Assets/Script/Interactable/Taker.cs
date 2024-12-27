using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taker : Interactable
{
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    protected override void Interact()
    {
        Destroy(gameObject);
    }
}
