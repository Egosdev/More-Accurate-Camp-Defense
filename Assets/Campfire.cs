using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Campfire : MonoBehaviour
{
    [SerializeField] float rate;
    float timer;
    Vector3 target;
    float outerTarget;
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, timer);

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, Time.deltaTime * 5);
        }
        else
        {
            timer = rate;
            target = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
            outerTarget = Random.Range(6f, 6.5f);
            GetComponent<Light2D>().pointLightOuterRadius = Mathf.MoveTowards(GetComponent<Light2D>().pointLightOuterRadius, outerTarget, Time.deltaTime * 25);
            GetComponent<Light2D>().intensity = Mathf.MoveTowards(GetComponent<Light2D>().intensity, Random.Range(0.5f, 2f), Time.deltaTime * 20);
        }
    }
}
