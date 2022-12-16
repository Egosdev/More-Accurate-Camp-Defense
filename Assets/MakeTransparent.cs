using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    bool kill;
    Color target;
    public void Kill()
    {
        kill = true;
    }
    void Update()
    {
        if(kill == true)
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, target, Time.deltaTime * 5);
        }
    }
}
