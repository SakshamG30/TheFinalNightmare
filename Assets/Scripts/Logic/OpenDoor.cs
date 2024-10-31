using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenDoor : MonoBehaviour
{  // Name of the next scene to load
    private bool isDoorOpen = false;  // Track if the door is open and can be interacted with

    // This function is called by the CheckpointManager to enable door interaction
    public void Start()
    {
        // Disable the door interaction by default
        //SceneManager.LoadScene("PreBossScene");
    }
}
