using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NPCBrain : MonoBehaviour
{
    ObjectVisionRadius ObjectVisionRadiusScript;
    [SerializeField] GameObject visionRadiusObject;
    [SerializeField] GameObject npcAim;
    [SerializeField] GameObject target;
    [SerializeField] Animator aimAnim;
    public Transform designatedCoords;
    [SerializeField] LayerMask shootable;
    [SerializeField] bool canShoot;
    [SerializeField] float shootTimer;
    [SerializeField] float shootRate;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject npcFlag;
    [SerializeField] GameObject shellPref;
    [SerializeField] Transform fireSound;
    [SerializeField] Face faceScript;
    public GameObject[] selectShadows;
    public GameObject deadBody;
    public Transform listener;
    public bool isAngry;
    public bool isSoldier;
    Vector3 bulletTraceVector;
    Vector3 moveTargetCoords;
    float gunPointAngle;
    Gun gunStatScript;
    AIPath aiPath;
    AIDestinationSetter aiDestinationSetterScript;

    // T moves npc, M marry

    private void Start()
    {
        ObjectVisionRadiusScript = visionRadiusObject.GetComponent<ObjectVisionRadius>();
        aiDestinationSetterScript = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        target = npcFlag;
    }

    private void Update()
    {
        if (!isSoldier)
        {
            LookFlag();
            return;
        }
        Aiming();

        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
            shootTimer = Mathf.Clamp(shootTimer, 0, shootTimer);
        }
        if (lineRenderer.enabled)
        {
            var step = Time.deltaTime * 80;

            bulletTraceVector = Vector3.MoveTowards(bulletTraceVector, lineRenderer.GetPosition(1), step);
            lineRenderer.SetPosition(0, bulletTraceVector);
        }

        if (ObjectVisionRadiusScript.ObjectsInFollowAreaList.Count == 0) return;
        if (target == transform) return;

        if(shootTimer == 0)
        {
            canShoot = true;
        }

        target = GetClosestObject(ObjectVisionRadiusScript.ObjectsInFollowAreaList).gameObject;
    }

    public void MoveToCoords()
    {
        moveTargetCoords = CustomCursor.Instance.transform.position;
        npcFlag.transform.position = moveTargetCoords;
        aiDestinationSetterScript.target = npcFlag.transform;
        SoundManager.Instance.PlaySoundPitchRandomizer(npcFlag.GetComponent<AudioSource>(), SoundManager.Instance.flagSFX, 0.2f);
    }
    public void MoveToCoords(Transform coordTransform)
    {
        GetComponent<AIDestinationSetter>().target = coordTransform;
    }

    void LookFlag() //needs refactor
    {
        Vector3 aimDir = (target.transform.position - transform.position).normalized;
        gunPointAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(gunPointAngle, Vector3.forward);
        npcAim.transform.rotation = Quaternion.Slerp(npcAim.transform.rotation, q, Time.deltaTime * 5);
        faceScript.angle = npcAim.transform.rotation.eulerAngles.z;
    }

    void Aiming()
    {
        if (target != null)
        {
            Vector3 aimDir = (target.transform.position - transform.position).normalized;
            gunPointAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

            if (target == npcFlag && aiPath.reachedDestination)
            {
                if (npcAim.transform.rotation.eulerAngles.z > 90 && npcAim.transform.rotation.eulerAngles.z < 270)
                    gunPointAngle = 180;
                else
                    gunPointAngle = 0;
            }
            Quaternion q = Quaternion.AngleAxis(gunPointAngle, Vector3.forward);
            npcAim.transform.rotation = Quaternion.Slerp(npcAim.transform.rotation, q, Time.deltaTime * 5);
        }
        Vector3 localScale = Vector3.one;
        if (npcAim.transform.rotation.eulerAngles.z > 90 && npcAim.transform.rotation.eulerAngles.z < 270)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = +1f;
        }
        npcAim.transform.localScale = localScale;
        faceScript.angle = npcAim.transform.rotation.eulerAngles.z;
    }

    Transform GetClosestObject(List<Collider2D> objects)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = visionRadiusObject.transform.position;
        foreach (Collider2D t in objects)
        {
            if (t.CompareTag("Player"))
            {
                if(isAngry)
                {
                    target = t.gameObject;
                }
                else
                {
                    if (objects.Count > 1)
                        continue;
                    else
                        return npcFlag.transform;
                }
            }
            if(target != null)
            {
                TryFire();
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

    private void TryFire()
    {
        Vector3 shotDir = (target.transform.position - new Vector3(0, 0.25f, 0) - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(gunStatScript.gunEndPointTransform.position, shotDir, gunStatScript.range, shootable);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
            {
                if (canShoot)
                {
                    shotDir = shotDir.normalized;
                    shotDir.x += Random.Range(-gunStatScript.walkingNormalSpread, gunStatScript.walkingNormalSpread);
                    shotDir.y += Random.Range(-gunStatScript.walkingNormalSpread, gunStatScript.walkingNormalSpread);
                    shotDir = shotDir.normalized;

                    RaycastHit2D spreadHit = Physics2D.Raycast(gunStatScript.gunEndPointTransform.position, shotDir, gunStatScript.range, shootable);
                    if (spreadHit.collider != null && !spreadHit.transform.CompareTag("Wall"))
                    {
                        spreadHit.collider.gameObject.GetComponent<HealthSystem>().Damage(gunStatScript.damage,shotDir,aimAnim.transform.eulerAngles.z, gameObject);
                        lineRenderer.SetPosition(1, shotDir * hit.distance);
                    }
                    else
                        lineRenderer.SetPosition(1, shotDir * gunStatScript.range);
                    canShoot = false;
                    shootTimer = shootRate + Random.Range(-0.1f,0.1f);
                    aimAnim.SetTrigger("Pistol");
                    lineRenderer.transform.position = gunStatScript.gunEndPointTransform.position;
                    StartCoroutine(BulletTrail());
                    EjectShell();
                    fireSound.position = gunStatScript.gunEndPointTransform.position;
                    //fireSound.GetComponent<AudioSource>().clip = gunStatScript.fireSoundClip;
                    SoundManager.Instance.PlaySoundPitchRandomizer(fireSound.GetComponent<AudioSource>(), gunStatScript.fireSoundClip, 0.15f);
                }
            }
        }
    }

    IEnumerator BulletTrail()
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        bulletTraceVector = Vector3.zero;
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.2f);
        lineRenderer.enabled = false;

        lineRenderer.SetPosition(1, new Vector3(5, 0, 0));
    }
    private void EjectShell()
    {
        GameObject ejectedShell = Instantiate(shellPref, npcAim.transform.GetChild(0).transform.position, Quaternion.identity);
        float xVnot = Random.Range(-6.5f, -9.5f);
        float yVnot = Random.Range(6.5f, 9.5f);
        if (npcAim.transform.rotation.eulerAngles.z > 90 && npcAim.transform.rotation.eulerAngles.z < 270)
        {
            xVnot *= -1;
        }
        ejectedShell.GetComponent<BulletShell>().xVnot = xVnot;
        ejectedShell.GetComponent<BulletShell>().yVnot = yVnot;

    }
    public void HighlightShadow(bool value,int index)
    {
        foreach (GameObject shadow in selectShadows)
        {
            shadow.SetActive(false);
        }

        selectShadows[index].SetActive(value);
    }
    public void Recruit()
    {
        SoundManager.Instance.PlaySound(listener.GetComponent<AudioSource>(), SoundManager.Instance.recruitSFX);
        isSoldier = true;
        npcAim.SetActive(true);
        gunStatScript = GetComponentInChildren<Gun>();
    }
    public void Flirt()
    {
        int randomValue = Random.Range(0, 101);
        int relation = transform.parent.GetComponent<NPCStat>().playerRelationship;
        if(randomValue + relation >= 175)
        {
            if(transform.parent.GetComponent<NPCStat>().marriedPersonWith != null && transform.parent.GetComponent<NPCStat>().marriedPersonWith != CampManager.Instance.player)
            {
                transform.parent.GetComponent<NPCStat>().marriedPersonWith.GetComponent<NPCStat>().ChangeRelationship(-100);
                transform.parent.GetComponent<NPCStat>().marriedPersonWith.GetComponent<NPCStat>().marriedPersonWith = null;
                transform.parent.GetComponent<NPCStat>().marriedPersonWith.GetComponentInChildren<NPCBrain>().DriveCrazy();
            }
            transform.parent.GetComponent<NPCStat>().marriedPersonWith = CampManager.Instance.player;
            CampManager.Instance.player.GetComponent<PlayerAim>().Marry();
        }
        else
        {
            transform.parent.GetComponent<NPCStat>().ChangeRelationship(-40);
        }
    }
    public void DriveCrazy()
    {
        isAngry = true;
        if (isSoldier)
        {
            transform.parent.GetComponent<NPCStat>().statusHolder.SetActive(true);
            transform.parent.GetComponent<NPCStat>().statusHolder.GetComponent<SpriteRenderer>().sprite = CampManager.Instance.statusIcons[1];
            MoveToCoords(CampManager.Instance.player.transform);
            aiPath.endReachedDistance = 4;
        }
        else
            MoveToCoords(CampManager.Instance.leaveCampPoint);
    }

    public void MakeRebel()
    {
        if (isSoldier)
        {
            isAngry = true;
            transform.parent.GetComponent<NPCStat>().statusHolder.SetActive(true);
            transform.parent.GetComponent<NPCStat>().statusHolder.GetComponent<SpriteRenderer>().sprite = CampManager.Instance.statusIcons[1];
            MoveToCoords(CampManager.Instance.player.transform);
            aiPath.endReachedDistance = 4;
        }
    }
}
