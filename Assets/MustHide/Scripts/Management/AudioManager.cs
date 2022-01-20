using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Photon.Pun;
public class AudioManager : MonoBehaviourPun
{
    public enum Sound
    {
        MP5Shoot,
        AK47,
        G36,
        MP445,
        SA80,
        MP5Reload,
        Running,
        HideIncreaseSound,
        MonsterGetShot,
        MonsterDead,
        HunterDead,
        RightProp,
        MonsterTransform,
        NightVision,
        GrenadeLauncher,
    }

    private Dictionary<Sound, float> soundTimerDictionary;
    private List<AudioSource> audioPool = new List<AudioSource>();

    private void Start()
    {
        photonView.RPC("SetupAudioPoolList", RpcTarget.All);
    }

    public void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.MP5Reload] = 0f;
        soundTimerDictionary[Sound.Running] = 0f;
    }

    public void PlaySound(Sound sound, float MaxDistance , int target, float Volume, float SpatialBlend, bool isNetwork) 
    {
        if (GetComponent<Health>().isDead)
            return;

        if (camPlaySound(sound))
        {
            if (isNetwork)
            {
                GetComponent<PhotonView>().RPC("RPC_PlaySound", RpcTarget.AllBuffered, sound, MaxDistance, target, Volume, SpatialBlend);
            }
            else
            {
                RPC_PlaySound(sound, MaxDistance, target, Volume, SpatialBlend);
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
    private void SetupAudioPoolList()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            soundGameObject.AddComponent<DestroyAfterDelay>();
            soundGameObject.AddComponent<FollowTarget>();
            audioPool.Add(audioSource);
            soundGameObject.SetActive(false);
        }
    }

    [PunRPC]
    private void RPC_PlaySound(Sound sound, float MaxDistance, int target, float Volume, float SpatialBlend)
    {
        foreach (var item in audioPool)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                item.gameObject.SetActive(true);
                var DestroyTime = item.gameObject.GetComponent<DestroyAfterDelay>();
                item.outputAudioMixerGroup = InGameManager.instance.audioMixer[1];
                item.spatialBlend = SpatialBlend;
                item.rolloffMode = AudioRolloffMode.Custom;
                item.maxDistance = MaxDistance;
                item.volume = Volume;
                item.transform.position = transform.position;
                item.clip = GetAudioClip(sound);
                if (target != 0)
                {
                    FollowTarget followTarget = item.gameObject.GetComponent<FollowTarget>();
                    followTarget.SetTarget(target);
                }

                DestroyTime.Delay = GetAudioClip(sound).length;
                item.Play();
                break;
            }
        }
        
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
