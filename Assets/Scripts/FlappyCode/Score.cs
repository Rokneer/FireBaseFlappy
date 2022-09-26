using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private string URL;
    [SerializeField] Text[] highScoreText;
    [SerializeField] GameObject highScoreCanvas;
    private string Token;
    public static int score = 0;
    private static int highScore = 0;
    void Start()
    {
        score = 0;
        Token = PlayerPrefs.GetString("token");
        highScore = PlayerPrefs.GetInt("High Score", highScore);
    }
    void Update()
    {
        GetComponent<Text>().text = score.ToString();
        if (score <= highScore) return;
        highScore = score;
        PlayerPrefs.SetInt("High Score", highScore);

    }
    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }
    public void HideHighScores()
    {
        highScoreCanvas.SetActive(false);
    }

    public void UpdateScores()
    {
        Token = PlayerPrefs.GetString("token");
        UserData userData = new UserData
        {
            username = PlayerPrefs.GetString("username"),
            score = PlayerPrefs.GetInt("High Score")
        };
        string postData = JsonUtility.ToJson(userData);
        StartCoroutine(PatchScore(postData));
    }
    IEnumerator GetScores()
    {
        string url = URL + "/api/usuarios" + "?limit=5&sort=true";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();
        highScoreCanvas.SetActive(true);
#pragma warning disable CS0618
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if(www.responseCode == 200){
            
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);
            int i = 0;
            foreach (UserData userScore in resData.usuarios)
            {
                Debug.Log(resData.usuarios[i].username + " | " + resData.usuarios[i].score);
                highScoreText[i].text = resData.usuarios[i].username + " | " + resData.usuarios[i].score;
                i++;
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }
    IEnumerator PatchScore(string postData)
    {
        Debug.Log("PATCH SCORE: ");
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "PATCH";
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
        }
        else  
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }
    [Serializable] public class Scores
    {
        public UserData[] usuarios;
    }
}
