using UnityEngine;
using UnityEngine.SceneManagement;

public class SpringController : MonoBehaviour
{
    public const string sceneName = "Spring";

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(SummerController.sceneName);
        }
    }
}
