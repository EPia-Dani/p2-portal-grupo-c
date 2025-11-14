using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Image backgroundImage;

    private void OnEnable()
    {
        Laser.playerDie += ShowGameOverScreen;
    }
    private void OnDisable()
    {
        Laser.playerDie -= ShowGameOverScreen;
    }
    private void ShowGameOverScreen()
    {
        backgroundImage.gameObject.SetActive(true);
    }
    private void Update()
    {
        if (backgroundImage.gameObject.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    public void RestartGame()
    {
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
