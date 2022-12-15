using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float attackTime = -1;
    [SerializeField] List<Collider2D> ObjectsInAttackableAreaList;
    [SerializeField] Animator anim;
    public int attackDamage;
    public float attackCooldown = 1;

    private void Update()
    {
        if(attackTime >= 0)
        {
            attackTime -= Time.deltaTime;
            attackTime = Mathf.Clamp(attackTime, 0, attackTime);
        }
        if(attackTime == 0)
        {
            if (ObjectsInAttackableAreaList.Count == 0)
            {
                //Debug.Log("0");
            }

            for (int i = 0; i < ObjectsInAttackableAreaList.Count; i++)
            {
                //Debug.Log(ObjectsInAttackableAreaList[i].transform.name);
                ObjectsInAttackableAreaList[i].GetComponent<HealthSystem>().Damage(attackDamage,new Vector3(0,0,0),0, transform.parent.gameObject);
                if(ObjectsInAttackableAreaList[i].CompareTag("Barricade"))
                    SoundManager.Instance.PlaySoundPitchRandomizer(gameObject.GetComponentInParent<EnemyBrain>().listener.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(SoundManager.Instance.hitSFX), 0.15f);
                else
                    SoundManager.Instance.PlaySoundPitchRandomizer(gameObject.GetComponentInParent<EnemyBrain>().listener.GetComponent<AudioSource>(), SoundManager.Instance.GiveRandomClip(SoundManager.Instance.painSFX), 0.15f);

            }
            attackTime = -1;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!ObjectsInAttackableAreaList.Contains(collision))
        {
            ObjectsInAttackableAreaList.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (ObjectsInAttackableAreaList.Contains(collision))
        {
            ObjectsInAttackableAreaList.Remove(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (attackTime <= 0)
        {
            attackTime = attackCooldown; // make random later
            //attackTime = 2;
            //anim.SetFloat("fitAnim", 0.34f);
            anim.SetTrigger("Attack");
            //Debug.Log("attack time = 3");
        }
    }
}
