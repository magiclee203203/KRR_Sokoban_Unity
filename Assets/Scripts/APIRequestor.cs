using System;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;

[Serializable]
public class PostBody
{
    public string contents;
}

[Serializable]
public class MoveCommand
{
    public string direction;
    public int time;
}

[Serializable]
public class MoveCommandResp
{
    public bool solvable;
    public List<MoveCommand> commands;
}

[Serializable]
public class LevelDataResp
{
    public string levelData;
}

public class APIRequestor : MonoBehaviour
{
    [Header("API Endpoint")] public string endpoint;

    public void PostGridState(string gridState, Action<MoveCommandResp> onCompleteCallback)
    {
        RestClient.Post<MoveCommandResp>($"{endpoint}/solve/", new PostBody { contents = gridState })
            .Then(res => { onCompleteCallback?.Invoke(res); })
            .Catch(err => { Debug.Log("Request failed" + err); });
    }

    public void GetLevelData(Action<string> onCompleteCallback)
    {
        RestClient.Get<LevelDataResp>($"{endpoint}/level")
            .Then(res => { onCompleteCallback?.Invoke(res.levelData); })
            .Catch(err => { Debug.Log("Request failed" + err); });
    }
}