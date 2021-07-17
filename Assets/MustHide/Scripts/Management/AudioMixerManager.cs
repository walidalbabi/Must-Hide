using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class AudioMixerManager : MonoBehaviour
{

    public static AudioMixerManager instance;

    public Slider[] audioSlider;

    [SerializeField]
    private AudioMixer audioMixer;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);

    }

    private void Start()
    {
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolumeValue", -13f));
        audioMixer.SetFloat("VFXVolume", PlayerPrefs.GetFloat("VFXVolumeValue", 6f));
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolumeValue", -13f));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene , LoadSceneMode mode)
    {
        if(scene.buildIndex == 0)
        {
            if(audioSlider.Length > 0)
            {
                audioSlider[0] = MenuManager.instance.audiSlider[0];
                audioSlider[1] = MenuManager.instance.audiSlider[1];
                audioSlider[2] = MenuManager.instance.audiSlider[2];

                audioSlider[0].value = PlayerPrefs.GetFloat("MasterVolumeValue", -20f);
                audioSlider[1].value = PlayerPrefs.GetFloat("VFXVolumeValue", -20f);
                audioSlider[2].value = PlayerPrefs.GetFloat("MusicVolumeValue", -20f);
            }
            else
            {
                audioSlider = new Slider[3];

                audioSlider[0] = MenuManager.instance.audiSlider[0];
                audioSlider[1] = MenuManager.instance.audiSlider[1];
                audioSlider[2] = MenuManager.instance.audiSlider[2];

                audioSlider[0].value = PlayerPrefs.GetFloat("MasterVolumeValue", -20f);
                audioSlider[1].value = PlayerPrefs.GetFloat("VFXVolumeValue", -20f);
                audioSlider[2].value = PlayerPrefs.GetFloat("MusicVolumeValue", -20f);
            }

        }
    }

    public void UP_Master()
    {
        audioMixer.SetFloat("MasterVolume", audioSlider[0].value);
        PlayerPrefs.SetFloat("MasterVolumeValue", audioSlider[0].value);
 
    }
    public void UP_VFX()
    {

        audioMixer.SetFloat("VFXVolume", audioSlider[1].value);
        PlayerPrefs.SetFloat("VFXVolumeValue", audioSlider[1].value);

    }
    public void UP_Music()
    {
        audioMixer.SetFloat("MusicVolume", audioSlider[2].value);
        PlayerPrefs.SetFloat("MusicVolumeValue", audioSlider[2].value);
    }
}
