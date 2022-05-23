using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Inventory : MonoBehaviourPun
{
    public static Inventory Instance;

    public bool InventoryOpen;

    [SerializeField] private GameObject menu;

    void Start()
    {
        Instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            menu.SetActive(!menu.activeSelf);

            InventoryOpen = menu.activeSelf;

            if(InventoryOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
