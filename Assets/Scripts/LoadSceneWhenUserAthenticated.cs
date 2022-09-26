using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using UnityEngine.SceneManagement;
public class LoadSceneWhenUserAthenticated : MonoBehaviour
{
    [SerializeField]
    private string _sceneToLoad;
    void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChange;
    }
    private void HandleAuthStateChange(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            SceneManager.LoadScene(_sceneToLoad);
        }
    }
}
