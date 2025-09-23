using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    public AudioSource musicSrc;
    public AudioSource conveyorBeltSrc;
    public AudioSource soundSrc;
    public AudioSource onFireSrc;

    public AudioClip button;
    public AudioClip goldReward;
    public AudioClip shuffle;
    public AudioClip magnet;
    public AudioClip addMove;
    public AudioClip addBox;
    public AudioClip shipperArrives;
    public AudioClip shipperGo;
    public AudioClip onDropShippersBox;
    public AudioClip onDropConveyorBelt;
    public AudioClip onDropConveyorBeltByMagnet;
    public AudioClip onQueue;
    public AudioClip onClickTray;

    public AudioClip win;
    public AudioClip lose;

    Coroutine onFire;

    private void Awake()
    {
        instance = this;
    }

    public void PlayMusic(bool isPlay)
    {
        musicSrc.mute = isPlay;
        conveyorBeltSrc.mute = isPlay;
    }

    public void StartOnFire()
    {
        onFire = StartCoroutine(OnFire());
    }

    public void StopOnFire()
    {
        if (onFire != null) StopCoroutine(onFire);

        onFireSrc.Stop();
    }

    IEnumerator OnFire()
    {
        while (true)
        {
            onFireSrc.Play();

            yield return new WaitForSeconds(Random.Range(3f, 5f));
        }
    }

    public void PlaySoundNVibrate(AudioClip audioClip, int strength = 0)
    {
        if (audioClip != null && GameManager.instance.IsAtiveSound)
        {
            soundSrc.PlayOneShot(audioClip);
        }
        if (GameManager.instance.IsActiveVibrate)
        {
            if (strength != 0) Duc.Vibration.Vibrate(strength);
        }
    }
}
