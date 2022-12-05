using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectVisionRadius : MonoBehaviour
{
    public List<Collider2D> ObjectsInFollowAreaList;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ObjectsInFollowAreaList.Contains(collision))
        {
            if (collision.gameObject.layer == 6) return; // wall
            ObjectsInFollowAreaList.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (ObjectsInFollowAreaList.Contains(collision))
        {
            ObjectsInFollowAreaList.Remove(collision);
        }
    }
}
