using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyBrain : MonoBehaviour
{
    ObjectVisionRadius ObjectVisionRadiusScript;
    AIDestinationSetter aiDestinationSetter;
    AIPath aiPath;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject followRadiusObject;
    [SerializeField] GameObject enemyAim;
    [SerializeField] GameObject attackRange;
    [SerializeField] Transform targetTransform;
    public GameObject deadBody;
    public Transform listener;
    [SerializeField] Transform moanListener;
    public int expAmount;
    public EnemyType enemyType;
    Vector2 facingDir;
    public float moanRate;
    float moanTimer;
    public enum EnemyType
    {
        Normal = 0,
        Speedy = 1,
        Fat = 2,
    }

    private void Start()
    {
        moanRate = Random.Range(3f, 5f);
        ObjectVisionRadiusScript = followRadiusObject.GetComponent<ObjectVisionRadius>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        if(PoolManager.Instance.NPCPoolTransform.childCount != 0)
            aiDestinationSetter.target = CampManager.Instance.campCenter.transform;
        else
            aiDestinationSetter.target = CampManager.Instance.player.transform;
    }

    private void Update()
    {
        FaceVelocity();
        Moaning();
        if (ObjectVisionRadiusScript.ObjectsInFollowAreaList.Count == 0)
        {
            aiDestinationSetter.target = CampManager.Instance.player.transform;
            return;
        }
        //Debug.DrawRay(followRadiusObject.transform.position, GetClosestObject(ObjectVisionRadiusScript.ObjectsInFollowAreaList).position - followRadiusObject.transform.position, Color.blue, 0.05f);
        targetTransform = GetClosestObject(ObjectVisionRadiusScript.ObjectsInFollowAreaList);
        aiDestinationSetter.target = targetTransform;
    }
    Transform GetClosestObject(List<Collider2D> objects)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = followRadiusObject.transform.position;
        foreach (Collider2D t in objects)
        {
            if (t.CompareTag("Player"))
            {
                tMin = t.transform;
                return tMin;
            }
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }

        }
        return tMin;
    }

    void FaceVelocity()
    {
        facingDir = aiPath.desiredVelocity;
        enemyAim.transform.right = facingDir;
        if (targetTransform != null)
        {
            attackRange.transform.right = (targetTransform.position - transform.position).normalized;
        }
        Aiming();
    }

    private void Aiming()
    {
        Vector3 localScale = Vector3.one;
        if (enemyAim.transform.rotation.eulerAngles.z > 90 && enemyAim.transform.rotation.eulerAngles.z < 270)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = +1f;
        }
        enemyAim.transform.localScale = localScale;
    }

    public void Moaning()
    {
        if (moanTimer > 0)
        {
            moanTimer -= Time.deltaTime;
            moanTimer = Mathf.Clamp(moanTimer, 0, moanTimer);
        }
        else
        {
            moanTimer = moanRate;
            SoundManager.Instance.PlaySoundPitchRandomizer(moanListener.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(SoundManager.Instance.zombieMoanSFX), 0.15f);
        }
    }
}
