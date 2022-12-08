using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    #region Singleton
    public static PoolManager Instance;

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

    public Transform shellPoolTransform;
    public Transform enemyPoolTransform;
    public Transform corpsePoolTransform;
    public Transform NPCPoolTransform;
    public Transform materialPoolTransform;
    public Transform bloodPoolTransform;
}
