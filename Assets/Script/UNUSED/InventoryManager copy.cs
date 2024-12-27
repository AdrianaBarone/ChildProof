/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventoryManagerCopy : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;

    //Riferimento al movimento 
    public GameObject player;
    private PlayerMovement playerInteractScript;

    void Start()
    {
        playerInteractScript = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && menuActivated)
        {
            InventoryMenu.SetActive(false);
            menuActivated = false;

            //playerInteractScript.EnablePlayerMovement(true);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetButtonDown("Inventory") && !menuActivated)
        {
            InventoryMenu.SetActive(true);
            menuActivated = true;

            ItemSettings.Instance.ListItems();

            //playerInteractScript.EnablePlayerMovement(false);  
            Cursor.lockState = CursorLockMode.Confined;      
        }
    }
}
*/