using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HttpManager : MonoBehaviour
{
    [SerializeField] private string URL;
    private string Token;
    private string Username;
    
#pragma warning disable CS0618
    private void Start()
    {
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");
        Debug.Log("TOKEN: " + Token);

        StartCoroutine(GetPerfil());
    }
    public void ClickSignUp()
    {
        string postData = GetInputData();
        StartCoroutine(SignUp(postData));
    }
    public void ClickLogIn()
    {
        string postData = GetInputData();
        StartCoroutine(LogIn(postData));
    }
    private static string GetInputData()
    {
        AuthData data = new AuthData();
        data.username = GameObject.Find("InputFieldUsername").GetComponent<InputField>().text;
        data.password = GameObject.Find("InputFieldPassword").GetComponent<InputField>().text;
        string postData = JsonUtility.ToJson(data);
        return postData;
    }
    IEnumerator SignUp(string postData)
    {
        Debug.Log("SIGN UP: " + postData);
        
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");
        
        yield return www.SendWebRequest();
        
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            Debug.Log("Registrado " + resData.usuario.username + ", id:" + resData.usuario._id);
            StartCoroutine(LogIn(postData));
        }
        else  
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }
    IEnumerator LogIn(string postData)
    {
        Debug.Log("LOG IN: " + postData);
        
        string url = URL + "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");
        
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            
            Debug.Log("Auntenticado " + resData.usuario.username + ", id:" + resData.usuario._id);
            Debug.Log("TOKEN: " + resData.token);
            
            PlayerPrefs.SetString("token", resData.token);
            PlayerPrefs.SetString("username", resData.usuario.username);
            
            SceneManager.LoadScene("Game");
        }
        else  
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }
    IEnumerator GetPerfil()
    {
        string url = URL + "/api/usuarios/" + Username;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", Token);
        
        yield return www.SendWebRequest();
        
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            Debug.Log("Token valido " + resData.usuario.username + ", id:" + resData.usuario._id);
            SceneManager.LoadScene("Game");
        }
        else  
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }
}
[Serializable] public class AuthData
{
    public string username;
    public string password;
    public FlappyUserData usuario;
    public string token;
}
[Serializable] public class FlappyUserData
{
    public string _id;
    public string username;
    public bool estado;
    public int score;
}

