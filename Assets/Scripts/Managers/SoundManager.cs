using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager Instance;

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
    public AudioClip[] bloodSFX, deathSFX, headShotSFX, bodyFallSFX, hitSFX, painSFX, zombieSpawnSFX, zombieMoanSFX;
    public AudioClip flagSFX, gunEmptySFX, waveStartSFX, levelUpSFX, waveFinishedSFX, dayTimeSFX, pageSFX, stampDownSFX, stampUpSFX, UIClickSFX, weddingSFX, recruitSFX, pillsSFX, craftingSFX, collectSFX;
    public AudioClip[] ambience; //0 day, 1 night
    bool isSettingsOpen;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] AudioMixer mixer;

    private void Start()
    {
        SetEffectLevel(0.05f);
        SetMusicLevel(0.07f);
    }

    public void PlaySingleSoundAtOnce(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PlaySoundPitchRandomizer(AudioSource audioSource, AudioClip clip, float minusPlus)
    {
        audioSource.PlayOneShot(clip);
        audioSource.pitch = Random.Range(1 - minusPlus, 1 + minusPlus);
    }
    public void PlaySoundPitchRandomizer(AudioSource audioSource, AudioClip clip, float minusPlus, float waitTime)
    {
        StartCoroutine(WaitForPlaySound(audioSource,clip,minusPlus,waitTime));
    }
    IEnumerator WaitForPlaySound(AudioSource audioSource, AudioClip clip, float minusPlus, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        audioSource.PlayOneShot(clip);
        audioSource.pitch = Random.Range(1 - minusPlus, 1 + minusPlus);
    }

    public void PlaySound(AudioSource audioSource, AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public AudioClip GiveRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public void AudioSettingsButton()
    {
        if(isSettingsOpen)
            settingsPanel.SetActive(false);
        else
            settingsPanel.SetActive(true);
        isSettingsOpen = !isSettingsOpen;
    }

    public void SetEffectLevel(float value)
    {
        mixer.SetFloat("exposeEffect", Mathf.Log10(value) * 20);
    }
    public void SetMusicLevel(float value)
    {
        mixer.SetFloat("exposeMusic", Mathf.Log10(value) * 20);
    }
}
