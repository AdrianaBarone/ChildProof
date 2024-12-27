using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject player;
    private PlayerManager playerManager;

    void Start()
    {
        playerManager = player.GetComponent<PlayerManager>();
        InventoryMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Verifica se la fotocamera fissa è attiva
        if (playerManager.IsInStateExploration())
        {
            // Confinare il cursore quando la fotocamera fissa è attiva
            Cursor.lockState = CursorLockMode.None;  // Sblocca il cursore
        }
        else
        {
            // Restituisce il cursore al gioco quando la fotocamera fissa non è attiva
            ItemSettings.Instance.ListItems();  // Probabilmente gestisce l'inventario
            Cursor.lockState = CursorLockMode.Locked;  // Blocca il cursore al centro
        }
    }
}
