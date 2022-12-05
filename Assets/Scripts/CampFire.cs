using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    bool isPlayerClose;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.WaveCompleted) return;
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
            if (isPlayerClose && GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.WaveCompleted)
            {
                CampManager.Instance.player.GetComponent<PlayerAim>().interactIcon.SetActive(false);
                DayActionsManager.Instance.GoToDayActions();
            }
        }
    }
}
