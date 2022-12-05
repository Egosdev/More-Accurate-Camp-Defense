using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CampfireLight : MonoBehaviour, LightManager.ILightSetter
{
    [SerializeField] float rate;
    float timer;
    Vector3 target;
    float outerTarget;
    [SerializeField] ParticleSystem fire;
    [SerializeField] ParticleSystem spark;
    float intensityTarget;
    bool isBurning;

    private void Update()
    {
        if(!isBurning)
        {
            GetComponent<Light2D>().intensity = Mathf.MoveTowards(GetComponent<Light2D>().intensity, intensityTarget, Time.deltaTime);
            return;
        }

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

    public void SetLight(bool turnOn)
    {
        if(turnOn)
        {
            fire.Play();
            spark.Play();
            isBurning = true;
        }
        else
        {
            intensityTarget = 0;
            fire.Stop();
            spark.Stop();
            isBurning = false;
        }
    }
}
