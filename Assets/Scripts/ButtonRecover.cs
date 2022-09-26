using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRecover : MonoBehaviour
{
    [SerializeField] private Button _buttonRecover;

    private Coroutine _passwordResetCoroutine;
    [SerializeField] private GameObject recoverMessage;
    private void Reset()
    {
        _buttonRecover = GetComponent<Button>();
    }
    private void Start()
    {
        _buttonRecover.onClick.AddListener(HandleResetPasswordButtonClicked);
    }
    private void HandleResetPasswordButtonClicked()
    {
        string email = GameObject.Find("InputEmail").GetComponent<InputField>().text;
        _passwordResetCoroutine = StartCoroutine(ResetPassword(email));
        recoverMessage.SetActive(true);
    }
    private IEnumerator ResetPassword(string emailAddress)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var user = auth.CurrentUser;
        var passwordResetTask = auth.SendPasswordResetEmailAsync(emailAddress);
        
        yield return new WaitUntil(() => passwordResetTask.IsCompleted);

        if (user != null)
        {
            if (passwordResetTask.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
            }
            if (passwordResetTask.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + passwordResetTask.Exception);
            }
            Debug.Log("Password reset email sent successfully.");
        }
    }

}

