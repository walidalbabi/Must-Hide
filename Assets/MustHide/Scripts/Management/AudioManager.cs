using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Photon.Pun;
public class AudioManager : MonoBehaviour
{

    
    public enum Sound
    {
        MP5Shoot,
        MP5Reload,
        Running,
        HideIncreaseSound,
        MonsterGetShot,
        MonsterDead,
        HunterDead,
        RightProp,
        MonsterTransform,
    }

    private Dictionary<Sound, float> soundTimerDictionary;

    public void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.MP5Reload] = 0f;
        soundTimerDictionary[Sound.Running] = 0f;
    }

    public void PlaySound(Sound sound, float MaxDistance , int target, float Volume, bool isNetwork) 
    {
        if (GetComponent<Health>().isDead)
            return;

        if (camPlaySound(sound))
        {
            if (isNetwork)
            {
                GetComponent<PhotonView>().RPC("RPC_PlaySound", RpcTarget.AllBuffered, sound, MaxDistance, target, Volume);
            }
            else
            {
                RPC_PlaySound(sound, MaxDistance, target, Volume);
            }
        }
      
    }

    private bool camPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;
            case Sound.MP5Reload:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 3f;
                    if(lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Debug.Log("Doesn't Contain Key");
                    return false;
                }
            case Sound.Running:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 0.4f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Debug.Log("Doesn't Contain Key");
                    return false;
                }
        }
    }

    [PunRPC]
    private void RPC_PlaySound(Sound sound, float MaxDistance, int target, float Volume)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        var DestroyTime = soundGameObject.AddComponent<DestroyAfterDelay>();
        audioSource.spatialBlend = 1;
        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.maxDistance = MaxDistance;
        audioSource.volume = Volume;
        soundGameObject.transform.position = transform.position;
        audioSource.clip = GetAudioClip(sound);
        if (target != 0)
        {
            FollowTarget followTarget = soundGameObject.AddComponent<FollowTarget>();
            followTarget.SetTarget(target);
        }

        DestroyTime.Delay = GetAudioClip(sound).length;
        audioSource.Play();
    }

    private AudioClip GetAudioClip(Sound sound)
    {
        foreach (var soundAudioClip in InGameManager.instance.soundAudioClip)
        {
            if (soundAudioClip.sound == sound)
                return soundAudioClip.audioClip;
        }
        return null;
    }
}
