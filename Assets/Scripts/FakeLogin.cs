using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FakeLogin : MonoBehaviour
{
    public void Login()
    {
        SceneManager.LoadScene(Constants.GAME_SCENE);
    }
}