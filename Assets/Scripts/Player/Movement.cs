using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D _rb;
    Vector2 _moveDirection;
    float _moveX, _moveY;
    PlayerAim playerAimScript;
    [SerializeField] float zoomBackStandingPoseTimer;
    int mapBound;

    [Header("Movement")]
    public float speed;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerAimScript = GetComponent<PlayerAim>();
        mapBound = CameraManager.Instance.camBound;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.visible = false;
        }
        if (GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.DayActions)
        {
            _moveDirection = Vector2.zero;
            return;
        }

        _moveX = Input.GetAxisRaw("Horizontal");
        _moveY = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector2(_moveX, _moveY).normalized;

        if (zoomBackStandingPoseTimer >= 0)
        {
            CameraManager.Instance.ChangeCamera("Moving");
            zoomBackStandingPoseTimer -= Time.deltaTime;
        }
        else if (zoomBackStandingPoseTimer < 0)
        {
            CameraManager.Instance.ChangeCamera("Standing");
        }

        if (playerAimScript.aimCrossTimer >= 0)
        {
            playerAimScript.aimCrossTimer -= Time.deltaTime;
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -mapBound + 0.5f, mapBound - 0.5f), Mathf.Clamp(transform.position.y, -mapBound + 1f, mapBound - 1f), 0);
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(_moveDirection.x * (speed * playerAimScript.walkSpeedMultiplier), _moveDirection.y * (speed * playerAimScript.walkSpeedMultiplier));

        if (_rb.velocity.magnitude >= (speed * playerAimScript.walkSpeedMultiplier))
        {
            zoomBackStandingPoseTimer = 4;
            playerAimScript.aimCrossTimer = playerAimScript.waitForSecondsBackToBestAim;
            playerAimScript.CalculateSpread(playerAimScript.handSocket.GetComponent<Gun>().walkingNormalSpread);
        }
        if (_rb.velocity.magnitude == 0)
        {
            if (playerAimScript.aimCrossTimer < 0)
            {
                playerAimScript.CalculateSpread(0);
                return;

            }
            playerAimScript.CalculateSpread(playerAimScript.handSocket.GetComponent<Gun>().standingNormalSpread);
        }
    }
}
