using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ButtonLogin : MonoBehaviour
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private InputField _emailInputField;
    [SerializeField] private InputField _emailPasswordField;

    private Coroutine _loginCoroutine;

    public event Action<FirebaseUser> OnLoginSucceeded;
    public event Action<string> OnLoginFailed;
    
    private void Reset()
    {
        _loginButton = GetComponent<Button>();
        _emailInputField = GameObject.Find("InputEmail").GetComponent<InputField>();
        _emailPasswordField = GameObject.Find("InputPassword").GetComponent<InputField>();
    }
    void Start()
    {
        _loginButton.onClick.AddListener(HandleLoginButtonClicked);
    }
    private void HandleLoginButtonClicked()
    {
        if(_loginCoroutine == null)
        {
            _loginCoroutine = StartCoroutine(LoginCoroutine(_emailInputField.text, _emailPasswordField.text));
        } 
    }
    private IEnumerator LoginCoroutine(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email,password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if(loginTask.Exception != null)
        {
            Debug.LogWarning($"Login Failed with {loginTask.Exception}");
            OnLoginFailed?.Invoke($"Login Failed with {loginTask.Exception}");
        }
        else
        {
            Debug.Log($"Login succeeded with {loginTask.Result}");
            OnLoginSucceeded?.Invoke(loginTask.Result);
            SceneManager.LoadScene("FlappyGame");
        }
    }
}
