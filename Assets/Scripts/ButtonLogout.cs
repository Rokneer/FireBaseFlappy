using UnityEngine;
using Firebase.Auth;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonLogout : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("FirebaseHome");
    }
}
