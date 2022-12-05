using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    #region Singleton
    public static LightManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    //public GameObject[] allLights;
    [SerializeField] Light2D sun;
    [SerializeField] Color nightColor;
    float target = 1;
    Color targetColor = Color.white;

    public void TurnAllLights(bool On)
    {
        GameObject[] allLights = GameObject.FindGameObjectsWithTag("Light");
        if (On)
        {
            target = 0.5f;
            targetColor = nightColor;
        }
        else
        {
            target = 1f;
            targetColor = Color.white;
        }

        for (int i = 0; i < allLights.Length; i++)
        {
            allLights[i].GetComponent<ILightSetter>().SetLight(On);
        }
    }

    void Update()
    {
        sun.intensity = Mathf.MoveTowards(sun.intensity, target, Time.deltaTime / 5);
        sun.color = Color.Lerp(sun.color, targetColor, Time.deltaTime);
    }

    private void Start()
    {
        TurnAllLights(false);
    }

    public interface ILightSetter
    {
        public void SetLight(bool turnOn);
    }
}
