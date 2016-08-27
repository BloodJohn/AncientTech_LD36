using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatController : MonoBehaviour
{
    public void RestartClick()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }
}