using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MatchTimerManager : MonoBehaviour
{

    public static MatchTimerManager instance;

    [SerializeField]
    private float MatchTime;
    [SerializeField]
    private float ReadyTime;

    public GameObject HuntersPanel;
    public GameObject MonstersPanel;
    public GameObject StartPanel;
    public GameObject EscapePanel;
    public GameObject EndPanel;
    public GameObject FinishPanel;
    public GameObject ExitGamePanel;

  
    public Text GameEndText;
    public Text FinishTitle;
    public Text[] Stats;
    public Text XPText;
    public Slider XPSlider;
    public Slider XPSliderExtras;

    public float timer;

    [SerializeField]
    private Text TimerUI;
    public bool  canCount, createPlayer;
    public double StartTime;
    public double TimeInBetween;
    public double CurrentServerTime;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        canCount = true;
        StartTime = Photon.Pun.PhotonNetwork.Time;
        TimeInBetween = StartTime + ReadyTime;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentServerTime = Photon.Pun.PhotonNetwork.Time;
        Mathf.Clamp(timer,0, MatchTime);
        TimeFormat();
        CheckingForTime();
        CheclIfCanCount();


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ExitGamePanel.activeInHierarchy)
                ExitGamePanel.SetActive(false);
            else
                ExitGamePanel.SetActive(true);
        }
    }


    private void CheckingForTime()
    {


        if(CurrentServerTime >= TimeInBetween && InGameManager.instance.GameState == InGameManager.State.ChooseCharacter)
        {
            createPlayer = true;
            StartTime = Photon.Pun.PhotonNetwork.Time;
            TimeInBetween = StartTime + MatchTime;
        }

        if (CurrentServerTime >= (TimeInBetween - 180d) && InGameManager.instance.GameState == InGameManager.State.StartGame)
        {
            InGameManager.instance.GameState = InGameManager.State.EscapeTime;
        }

        if (CurrentServerTime >= TimeInBetween && InGameManager.instance.GameState == InGameManager.State.EscapeTime)
        {
            InGameManager.instance.GameState = InGameManager.State.EndGame;
        }

        if (InGameManager.instance.GameState != InGameManager.State.ChooseCharacter)
        {
            HuntersPanel.SetActive(false);
            MonstersPanel.SetActive(false);
        }
    }

    private void CheclIfCanCount()
    {

        timer = ((float)(CurrentServerTime % StartTime)- (float)(TimeInBetween % StartTime)) * -1f ;
    }

    private void SetCanCount()
    {
        //Setting It By Invoke
        canCount = true;
    }

    private void TimeFormat()
    {
    
        float minutes = Mathf.Floor(timer / 60);
        float seconds = Mathf.RoundToInt(timer % 60);
        string minutesS;
        string secondsS;
        if (minutes < 10)
        {
            minutesS = "0" + minutes.ToString();
        }
        else
        {
            minutesS = minutes.ToString();
        }
        if (seconds < 10)
        {
            secondsS = "0" + Mathf.RoundToInt(seconds).ToString();
        }
        else
        {
            secondsS = Mathf.RoundToInt(seconds).ToString();
        }
        TimerUI.text = minutesS + ":" + secondsS;
    }
}
