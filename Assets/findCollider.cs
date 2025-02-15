using UnityEngine;

public class findCollider : MonoBehaviour
{
    public Collider triggerCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider triggerCollider)
    {
        // Debug.Log(gameObject.name);
    }
}
