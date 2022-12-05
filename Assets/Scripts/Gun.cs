using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    public float walkingNormalSpread;
    public float standingNormalSpread;
    public float range;
    public float fireRate;
    public float reloadTime;
    public int damage;
    public int currentClip;
    public int clipCapacity;
    //public bool isReloading;
    public AmmoType ammoType;
    public enum AmmoType
    {
        shortBullet = 0,
        longBullet = 1,
        redShell = 2,
        infinite = 3,
    }

    public Transform gunEndPointTransform;

    public AudioClip fireSoundClip;
    public AudioClip reloadSoundClip;
    public AudioClip swapSoundClip;
}
