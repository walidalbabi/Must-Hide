using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

public class VivoxCredentials
{
    public VivoxUnity.Client client;

    public Uri server;
    public string issuer;
    public  string domain;
    public string tokenKey;

    public Uri serverPC = new Uri("https://mt1s.www.vivox.com/api2");
    public string issuerPC = "eterna9479-mu19-dev";
    public string domainPC = "mt1s.vivox.com";
    public string tokenKeyPC = "kelp280";

    public Uri serverMobile= new Uri("https://mt1s.www.vivox.com/api2");
    public string issuerMobile = "eterna9479-mu53-dev";
    public string domainMobile = "mt1s.vivox.com";
    public string tokenKeyMobile = "puck985";

    public TimeSpan timeSpan = TimeSpan.FromSeconds(90);


    public ILoginSession loginSession;
    public IChannelSession channelSession;

    public List<IFailedDirectedTextMessage> failedMessages = new List<IFailedDirectedTextMessage>();
}
