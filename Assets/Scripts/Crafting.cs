using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    bool isPlayerClose;
    [SerializeField] GameObject craftingPanel;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerClose = true;
            collision.GetComponent<PlayerAim>().interactIcon.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerClose = false;
            collision.GetComponent<PlayerAim>().interactIcon.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPlayerClose)
            {
                craftingPanel.SetActive(true);
                CampManager.Instance.DisplayAmmos();
            }
        }
    }
}
