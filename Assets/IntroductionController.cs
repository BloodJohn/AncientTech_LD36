using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroductionController : MonoBehaviour
{
    public const string sceneName = "Introduction";

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(SummerController.sceneName);
        }
    }
}
