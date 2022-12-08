using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummie : MonoBehaviour
{
    [SerializeField] Color target;
    [SerializeField] Transform bloodFx;
    [SerializeField] Transform plane;
    private void Start()
    {
        target = GetComponentInChildren<SpriteRenderer>().color;
    }
    void Update()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(GetComponentInChildren<SpriteRenderer>().color, target, Time.deltaTime * 15);

        if (Input.GetKeyDown(KeyCode.V))
        {
            ParticleSystem bloodFxClone = Instantiate(bloodFx, transform.position, Quaternion.identity).GetChild(0).GetComponent<ParticleSystem>();
            bloodFxClone.collision.SetPlane(0, plane);
            bloodFxClone.Play();
            bloodFxClone.transform.parent.SetParent(PoolManager.Instance.bloodPoolTransform);
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }
}
