using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerAim>().bonusIcon.SetActive(true);
        }
        if (collision.CompareTag("Npc"))
        {
            collision.GetComponent<NPCBrain>().bonusIcon.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerAim>().bonusIcon.SetActive(false);
        }
        if (collision.CompareTag("Npc"))
        {
            collision.GetComponent<NPCBrain>().bonusIcon.SetActive(false);
        }
    }
}
