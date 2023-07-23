using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UINavigator : MonoBehaviour
{
    public string sceneName;
    public void GotoScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
