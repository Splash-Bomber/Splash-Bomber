
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
     public string anyKeySceneName; // 키 입력 시 전환할 씬 이름을 저장할 변수
    
        void Update()
        {
            // 키 입력을 감지하여 씬을 전환합니다.
            if (Input.anyKey)
            {
                SceneManager.LoadScene(anyKeySceneName);
            }
        }
    
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
