using UnityEngine;

public class TestPauseMenu : MonoBehaviour
{

    private void OnRestart()
    {
        Debug.Log("Master told me to restart...");
    }
    private void OnEnable()
    {
        PauseMenu.ButtonRestart += OnRestart;
    }

    private void OnDisable()
    {
        PauseMenu.ButtonRestart -= OnRestart;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!PauseMenu.Instance.IsVisible())
            {
                Debug.Log("Showing...");
                PauseMenu.Instance.Show();
            }
        }
    }
}
