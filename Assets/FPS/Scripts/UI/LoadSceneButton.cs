using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public string sceneName = "";

    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == gameObject 
            && Input.GetButtonDown(GameConstants.k_ButtonNameSubmit))
        {
            LoadTargetScene();
        }
    }

    public void LoadTargetScene()
    {
        //Comentado para pegar sempre a ultimafase jogada, o proxima só muda quando carregar o WinScene
        //SceneManager.LoadScene(sceneName);

        SceneManager.LoadScene(PlayerPrefs.GetString("sceneName"));
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("sceneName"));
    }
}
