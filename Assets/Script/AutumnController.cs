using UnityEngine;
using UnityEngine.SceneManagement;

public class AutumnController : MonoBehaviour
{
    public const string sceneName = "Autumn";

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(WinterController.sceneName);
        }
    }
}