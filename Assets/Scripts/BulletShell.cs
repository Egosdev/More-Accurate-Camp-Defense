using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    public float startYPosition;
    public float xVnot, yVnot;
    public float acceleration = -9.8f;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;

    private float offset = -0.5f;
    private float velocityTime;
    private float rotationSpeed = 600f;
    private float value = 1;

    [SerializeField] float lifeTime;
    [SerializeField] AudioClip[] shellClips;

    private void Start()
    {
        startYPosition = transform.position.y;
        rb.velocity = new Vector2(xVnot, yVnot);
        transform.SetParent(PoolManager.Instance.shellPoolTransform);
        //GetComponent<AudioSource>().clip = shellClips[Random.Range(0, 2)];
        SoundManager.Instance.PlaySoundPitchRandomizer(GetComponent<AudioSource>(), shellClips[Random.Range(0, 2)], 0.15f);

    }

    private void FixedUpdate()
    {
        Acceleration();
        if(rb.velocity.magnitude == 0)
        {
            lifeTime -= Time.deltaTime;
            GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
        if(lifeTime < 0)
        {
            value = Mathf.MoveTowards(value, 0, Time.deltaTime * 2);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, value);
            if(value <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Acceleration()
    {
        velocityTime += Time.fixedDeltaTime;

        if (rb.velocity.magnitude < 0.5f)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        transform.Rotate(0f, 0f, rotationSpeed * Time.fixedDeltaTime);
        if(transform.position.y <= startYPosition + offset && rb.velocity.y <0)
        {
            float yVelocity = -rb.velocity.y * 0.25f;
            float xVelocity = rb.velocity.x * 0.25f;

            rb.velocity = new Vector2(xVelocity, yVelocity);
            velocityTime = 0;
        }
        else
        {
            float yVelocity = rb.velocity.y + acceleration * velocityTime;
            rb.velocity = new Vector2(rb.velocity.x, yVelocity);
        }
    }
}
