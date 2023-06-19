using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //[SerializeField] GameObject _canvas;
    public void StartServer()
    {
        if (!NetworkManager.Singleton.StartServer())
        {
            Debug.LogWarning("Failed to start server");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void StartClient()
    {
        if (!NetworkManager.Singleton.StartClient())
        {
            Debug.LogWarning("Failed to start client");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void StartHost()
    {
        if (!NetworkManager.Singleton.StartHost())
        {
            Debug.LogWarning("Failed to start Host");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
