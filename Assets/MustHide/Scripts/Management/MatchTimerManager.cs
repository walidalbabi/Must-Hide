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
    public GameObject EndPanel;

    [SerializeField]
    private Text GameEndText;

    private float timer = 10;

    [SerializeField]
    private Text TimerUI;
    public bool isChoosePanel, canCount, createPlayer;


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

        Invoke("SetCanCount", 0.3f);


    }

    // Update is called once per frame
    void Update()
    {
    
        TimeFormat();
        CheckIfCanCount();
        if(timer <= 0)
        {
            if (isChoosePanel)
            {
                isChoosePanel = false;
                createPlayer = true;
                canCount = false;
                Invoke("SetCanCount", 0.3f);
            }
            else
            {
                //Monsters Win
                EndPanel.SetActive(true);
                timer = 0;
                GameEndText.text = "Game End Monsters Win";
            }
        }
        else
        {
            //If GameOver
            if (InGameManager.instance.GameIsOver)
            {
                EndPanel.SetActive(true);
                if(InGameManager.instance.MonstersDead >= 5)
                {
                    //Hunters Win
                    GameEndText.text = "Game End Hunters Win";
                }
                if (InGameManager.instance.HuntersDead >= 5)
                {
                    //Monsters Win
                    GameEndText.text = "Game End Monsters Win";
                }
                timer = 0;
            }
        }

        if(!isChoosePanel)
        {
            HuntersPanel.SetActive(false);
            MonstersPanel.SetActive(false);
        }

    }

    //if i can count and count state
    private void CheckIfCanCount()
    {

        if (isChoosePanel && !canCount)
        {
            timer = ReadyTime;
        }
        else if (!isChoosePanel && !canCount)
        {
            
            timer = MatchTime;
        }

        if (canCount )
        {
            timer -= Time.deltaTime;
        }

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
