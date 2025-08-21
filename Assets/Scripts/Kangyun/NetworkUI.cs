using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkUI : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
}
