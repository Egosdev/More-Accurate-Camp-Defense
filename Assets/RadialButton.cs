using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialButton : MonoBehaviour
{
    [SerializeField] Color[] colors;
    public int buttonID; //0 up //1 right // 2 left

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(1))
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
            GetComponent<SpriteRenderer>().color = colors[1];
        }
    }
    private void OnMouseExit()
    {
        transform.localScale = new Vector3(0.45f, 0.45f, 1);
        GetComponent<SpriteRenderer>().color = colors[0];
        Debug.Log("test");
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            transform.localScale = new Vector3(0.45f, 0.45f, 1);
            GetComponent<SpriteRenderer>().color = colors[0];
            if(buttonID == 0)
            {
                UIManager.Instance.displayWhichNpc.GetComponentInChildren<NPCBrain>().FollowPlayer(true);
                UIManager.Instance.displayWhichNpc.GetComponentInChildren<NPCBrain>().WalkAround(false);
            }
            if (buttonID == 1)
            {
                UIManager.Instance.displayWhichNpc.GetComponentInChildren<NPCBrain>().WalkAround(true);
                UIManager.Instance.displayWhichNpc.GetComponentInChildren<NPCBrain>().FollowPlayer(false);
            }
            if (buttonID == 2)
            {
                UIManager.Instance.displayWhichNpc.GetComponentInChildren<NPCBrain>().WalkAround(false);
                UIManager.Instance.displayWhichNpc.GetComponentInChildren<NPCBrain>().FollowPlayer(false);
                UIManager.Instance.displayWhichNpc.GetComponentInChildren<NPCBrain>().MoveToCoords();
            }
        }
    }
}
