
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void AnyKey(string sceneName)
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
