using Firebase.Auth;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class ButtonRegister : MonoBehaviour
{
    [SerializeField] private Button _registrationButton;
    private Coroutine _registrationCoroutine;

    public event Action<FirebaseUser> OnUserRegistered;
    public event Action<string> OnUserResgistrationFailed;

    void Reset()
    {
        _registrationButton = GetComponent<Button>();
    }
    void Start()
    {
        _registrationButton.onClick.AddListener(HandleRegistrationButtonClicked);
    }
    private void HandleRegistrationButtonClicked()
    {
        string email = GameObject.Find("InputEmail").GetComponent<InputField>().text;
        string password = GameObject.Find("InputPassword").GetComponent<InputField>().text;

        _registrationCoroutine = StartCoroutine(RegisterUser(email, password));
    }
    private IEnumerator RegisterUser(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task {registerTask.Exception}");
            OnUserResgistrationFailed?.Invoke($"Failed to register task {registerTask.Exception}");
        }
        else
        {
            Debug.Log($"Succesfully registered user {registerTask.Result.Email}");

            UserData data = new UserData();

            data.username = GameObject.Find("InputUsername").GetComponent<InputField>().text;
            string json = JsonUtility.ToJson(data);

            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(registerTask.Result.UserId).SetRawJsonValueAsync(json);

            OnUserRegistered?.Invoke(registerTask.Result);
        }
        _registrationCoroutine = null;
    }
}