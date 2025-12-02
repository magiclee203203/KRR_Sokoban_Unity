using System;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;

[Serializable]
public class PostBody
{
    public string state;
}

[Serializable]
public class ServerResp
{
    public string resp;
}

public class APIRequestor : MonoBehaviour
{
    [Header("API Endpoint")] public string endpoint;

    public void PostGridState(string gridState)
    {
        RestClient.Post<ServerResp>(endpoint, new PostBody { state = gridState })
            .Then(res => { Debug.Log(res.resp); })
            .Catch(err => { Debug.Log("Request failed" + err); });
    }
}