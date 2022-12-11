using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrisMovement : MonoBehaviour
{
    Vector3 mouseWorldPosition;
    [SerializeField] GameObject iris;
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        iris.transform.position = Vector3.ClampMagnitude(mouseWorldPosition - transform.position, 0.1f) + transform.position;
        iris.transform.position = new Vector3(iris.transform.position.x, iris.transform.position.y, 0);
    }
}
