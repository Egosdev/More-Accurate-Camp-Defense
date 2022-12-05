using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    #region Singleton
    public static CameraManager Instance;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        CalculateCamBound();
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

    [SerializeField] CinemachineVirtualCamera[] cameras;
    public int camBound;

    public void ChangeCamera(string camName)
    {
        foreach (CinemachineVirtualCamera vcam in cameras)
        {
            vcam.Priority = 1;
        }

        if (camName == "Standing")
            cameras[0].Priority = 2;
        else if (camName == "Moving")
            cameras[1].Priority = 2;
    }

    private void CalculateCamBound()
    {
        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(camBound, camBound);
        points[1] = new Vector2(camBound, -camBound);
        points[2] = new Vector2(-camBound, -camBound);
        points[3] = new Vector2(-camBound, camBound);

        GetComponent<PolygonCollider2D>().SetPath(0, points);
    }

    // CAUTION!! AWAKE METHOD FORCES RESOLUTION TO 1920x1080
}
