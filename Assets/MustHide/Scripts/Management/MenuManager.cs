using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CreateCustomMatchPanel;
    [SerializeField]
    private GameObject JoinRoomPanel;
    [SerializeField]
    private GameObject CurrentRoomPanel;
    [SerializeField]
    private GameObject StartPanel;
    [SerializeField]
    private GameObject PlayPanel;
    [SerializeField]
    private GameObject FriendListPanel;

    [SerializeField]
    private GameObject LoginPanel;
    [SerializeField]
    private GameObject RegisterPanel;
    [SerializeField]
    private GameObject RecoveryPanel;


    public Transform _ChatContent;
    public InvitationScript invitation;



    public static MenuManager instance;


    private void Awake()
    {
        if (instance != null && instance != this)//if already exist
            gameObject.SetActive(false);
        else
        {
            //set the instance
            instance = this;
        }
    }


    public void ShowHome()
    {
        StartPanel.SetActive(true);
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);
    }


    public void Play()
    {
        PlayPanel.SetActive(true);
        StartPanel.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Back(string state)
    {
        if(state == "Menu")
        {
            StartPanel.SetActive(true);
            PlayPanel.SetActive(false);
            CreateCustomMatchPanel.SetActive(false);
            JoinRoomPanel.SetActive(false);
        }
        
        if(state == "Play")
        {
            StartPanel.SetActive(false);
            PlayPanel.SetActive(true);
            CreateCustomMatchPanel.SetActive(false);
            JoinRoomPanel.SetActive(false);
            FriendListPanel.SetActive(false);
        }

    }

    public void CreateCustomMatch()
    {
        StartPanel.SetActive(false);
        CreateCustomMatchPanel.SetActive(true);
    }


    public void Public()
    {
        StartPanel.SetActive(false);
        JoinRoomPanel.SetActive(true);
    }

    public void ShowFriendsPanel()
    {
        PlayPanel.SetActive(false);
        FriendListPanel.SetActive(true);
    }

    public void ShowCurrentRoomPanel()
    {
        CreateCustomMatchPanel.SetActive(false);
        JoinRoomPanel.SetActive(false);
        CurrentRoomPanel.SetActive(true);
    }

    public void ExitCurrentRoomPanel()
    {
        StartPanel.SetActive(true);
        CreateCustomMatchPanel.SetActive(false);
        JoinRoomPanel.SetActive(false);
        CurrentRoomPanel.SetActive(false);
        NetworkManager.instance.LeaveRoom();
    }

    public void GoToLogin()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }

    public void GoToRegister()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }

    public void GoToForgetPassword()
    {
        LoginPanel.SetActive(false);
        RecoveryPanel.SetActive(true);
    }


}
