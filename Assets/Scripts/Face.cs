using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    [SerializeField] GameObject eyes;
    [SerializeField] SpriteRenderer faceSpriteRenderer;
    [SerializeField] Sprite[] faceSprites; //0 happy, 1 wink, 2 angry, 3 fear, 4 pain, 5 busy, 6 fascinated
    public float angle;
    private IEnumerator winkCoroutine;
    private IEnumerator changeFaceCoroutine;
    [SerializeField] float winkCooldown = 3.5f;

    void Start()
    {
        CurrentFaceState = FaceState.Happy;
    }

    IEnumerator Wink()
    {
        StartCoroutine(ChangeFace(FaceState.Wink, 0.1f, eyes));
        int randomNumber = Random.Range(0, 4);
        if (randomNumber == 0)
        {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(ChangeFace(FaceState.Wink, 0.1f, eyes));
        }
    }
    private void Update()
    {
        if(winkCooldown > 0)
        {
            winkCooldown -= Time.deltaTime;
            winkCooldown = Mathf.Clamp(winkCooldown, 0, winkCooldown);
        }
        if(winkCooldown <= 0)
        {
            CurrentFaceState = FaceState.Wink;
        }

        if (angle < 90 && angle > -180)
        {
            float eachDegree = 0.25f / 45;
            float newValue = 0.5f;
            transform.localPosition = new Vector3(newValue - (eachDegree * -angle), transform.localPosition.y, transform.localPosition.z);
        }
        else
        {
            float eachDegree = 0.25f / 45;
            float newValue = -1.5f;
            transform.localPosition = new Vector3(newValue + (eachDegree * angle), transform.localPosition.y, transform.localPosition.z);
        }
    }

    public enum FaceState
    {
        Happy,
        Wink,
        Angry,
        Fear,
        Pain,
        Busy,
        Fascinated
    }
    private FaceState currentFaceState;
    public FaceState CurrentFaceState
    {
        get { return currentFaceState; }
        set
        {
            currentFaceState = value;
            OnStateChanged(value);
        }
    }
    void OnStateChanged(FaceState state)
    {
        winkCooldown = 3.5f;

        if (state == FaceState.Wink)
        {
            winkCoroutine = Wink();
            StartCoroutine(winkCoroutine);
            return;
        }

        StopAllCoroutines();

        if(state == FaceState.Pain)
        {
            changeFaceCoroutine = ChangeFace(state, 0.4f, eyes);
            StartCoroutine(changeFaceCoroutine);
            return;
        }

        if(state == FaceState.Fascinated)
        {
            changeFaceCoroutine = ChangeFace(state, 0.05f, eyes);
            StartCoroutine(changeFaceCoroutine);
            return;
        }

        eyes.SetActive(true);

        if(state == FaceState.Fear)
        {
            changeFaceCoroutine = ChangeFace(state, 3f);
            StartCoroutine(changeFaceCoroutine);
            return;
        }

        if(state == FaceState.Busy)
        {
            changeFaceCoroutine = ChangeFace(state, 2f);
            StartCoroutine(changeFaceCoroutine);
            return;
        }

        changeFaceCoroutine = ChangeFace(state, 1f);
        StartCoroutine(changeFaceCoroutine);
    }

    IEnumerator ChangeFace(FaceState state, float time)
    {
        faceSpriteRenderer.sprite = faceSprites[(int)state];
        yield return new WaitForSeconds(time);
        faceSpriteRenderer.sprite = faceSprites[0];
    }
    IEnumerator ChangeFace(FaceState state, float time, GameObject eyes)
    {
        faceSpriteRenderer.sprite = faceSprites[(int)state];
        eyes.SetActive(false);
        yield return new WaitForSeconds(time);
        eyes.SetActive(true);
        faceSpriteRenderer.sprite = faceSprites[0];
    }
}
