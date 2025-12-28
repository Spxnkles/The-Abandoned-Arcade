using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public Camera playerCamera;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Spawnpoint[] points = FindObjectsByType<Spawnpoint>(FindObjectsSortMode.None);

        foreach (Spawnpoint point in points)
        {
            if (point.spawnID == StoryManager.Instance.spawnPointID)
            {
                Debug.Log("Found a spawn!");
                PlayerController.Instance.transform.position = point.transform.position;
                PlayerController.Instance.transform.rotation = point.transform.rotation;
                playerCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
                
            }
        }

    }
}
