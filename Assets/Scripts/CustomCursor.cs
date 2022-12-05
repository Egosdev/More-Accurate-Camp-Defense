using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    #region Singleton
    public static CustomCursor Instance;

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

    [SerializeField] Vector3[] cursorParts;
    [SerializeField] float multiplier;
    PlayerAim playerAimScript;
    public Color aimColor;
    public bool isPrecision;

    private void Start()
    {
        playerAimScript = GetComponentInParent<PlayerAim>();
    }

    private void Update()
    {
        if (isPrecision == false) return;

        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i).transform.localPosition = cursorParts[i] * multiplier;
        }
        UpdateCursorDistance(1 + (15 * playerAimScript.currentSpread));
    }

    private void UpdateCursorDistance(float target)
    {
        multiplier = Mathf.MoveTowards(multiplier, target, Time.deltaTime * 30);
        multiplier = Mathf.Clamp(multiplier, 1, 5);
    }
    public void ChangeColor(Color color)
    {
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i).transform.GetComponent<SpriteRenderer>().color = color;
        }
    }
    public void ChangePrecision()
    {
        isPrecision = !isPrecision;

        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i).transform.localPosition = cursorParts[i];
        }
    }
}
