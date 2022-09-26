using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;
public class FB_ScoreController : MonoBehaviour
{
    DatabaseReference mDatabase;
    private string UserId;

    [SerializeField] Text[] highScoreText;
    [SerializeField] GameObject highScoreCanvas;
    
    UserData data = new UserData();

    public static int score;
    private static int highScore;

    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        score = data.score;
        highScore = score;
    }

    private void Update()
    {
        GetComponent<Text>().text = score.ToString();
    }
    public void WriteNewScore()
    {
        //if (highScore <= score) return;
        score = int.Parse(GameObject.Find("ScoreText").GetComponent<Text>().text);
        mDatabase.Child("users").Child(UserId).Child("score").SetValueAsync(score);
    }
    public void GetUserScore()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users/"+ UserId)
            .GetValueAsync().ContinueWithOnMainThread(task => {
                 if (task.IsFaulted)
                 {
                    Debug.Log(task.Exception);
                 }
                 else if (task.IsCompleted)
                 { 
                    DataSnapshot snapshot = task.Result;
                    var data = (Dictionary<string, object>)snapshot.Value;
                    Debug.Log("Puntaje: " + data["score"]);
                 }
            });
    }
    public void GetUsersHighestScores()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").LimitToLast(6)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log(snapshot);
                    int i = 0;
                    foreach (var userDoc in (Dictionary<string, object>)snapshot.Value)
                    { 
                        var userObject = (Dictionary<string,object>)userDoc.Value;

                        highScoreText[i].text = userObject["username"] + ": " + userObject["score"];
                        Debug.Log(userObject["username"] + ": " + userObject["score"]);
                        i++;
                    }

                    highScoreCanvas.SetActive(true);
                }                
            });
    }
    public void HideHighScores()
    {
        highScoreCanvas.SetActive(false);
    }
}

public class UserData
{
   public int score;
   public string username;
}
