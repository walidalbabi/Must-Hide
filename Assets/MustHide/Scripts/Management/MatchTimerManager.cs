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

    //Canvasas
    [HideInInspector]
    public CanvasGroup exitGameCanvas;
    [HideInInspector]
    public CanvasGroup startPanelCanvas;
    [HideInInspector]
    public CanvasGroup finishPanellCanvas;

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

        exitGameCanvas = ExitGamePanel.GetComponent<CanvasGroup>();
        startPanelCanvas = StartPanel.GetComponent<CanvasGroup>();
        finishPanellCanvas = FinishPanel.GetComponent<CanvasGroup>();
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
            {
                ActivatePanel(false, ExitGamePanel, exitGameCanvas, 0.3f);
            }
            else
                ActivatePanel(true, ExitGamePanel, exitGameCanvas , 0.3f);
        }
    }

    //FOr UI Animations
    public void ActivatePanel(bool activate, GameObject panel, CanvasGroup canvas, float fadeTime)
    {
        panel.SetActive(activate);
        if (activate)
        {
            canvas.alpha = 0;
            canvas.LeanAlpha(1, fadeTime);
        }
        else
        {
            canvas.alpha = 1;
            canvas.LeanAlpha(0, fadeTime);
            StartCoroutine(DisablePanel(panel));
        }
    }

    public IEnumerator DisablePanel (GameObject panel)
    {
        yield return new WaitForSeconds(0.23f);
        panel.SetActive(false);
    }

    public void ResumeGameBtn()
    {
        ActivatePanel(false, ExitGamePanel, exitGameCanvas, 0.3f);
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
