using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Npc"))
        {
            sr.color = new Color(0.85f, 0.85f, 0.85f, 0.35f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Npc"))
        {
            sr.color = new Color(0.85f, 0.85f, 0.85f, 1);
        }
    }
}
