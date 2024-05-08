using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    FpsGunControler gunController = null;
    [SerializeField]
    Canvas gameClearCanvas = null;
    [SerializeField]
    Canvas gameOverCanvas = null;
    [SerializeField]
    Text countText = null;
    [SerializeField, Min(1)]
    int maxCount = 30;

    bool isGameClear = false;
    bool isGameOver = false;
    int count = 0;

    public int Count
    {
        set
        {
            count = Mathf.Max(value, 0);

            UpdateCountText();

            if (count >= maxCount)
            {
                GameClear();
            }
        }
        get
        {
            return count;
        }
    }

    void Start()
    {
        count = 0;

        UpdateCountText();
    }

    public void GameClear()
    {
        if (isGameClear || isGameOver)
        {
            return;
        }

        gunController.enabled = false;
        gameClearCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;

        isGameClear = true;
    }

    public void GameOver()
    {
        if (isGameClear || isGameOver)
        {
            return;
        }

        gunController.enabled = false;
        gameOverCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;

        isGameOver = true;
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateCountText()
    {
        countText.text = count.ToString() + " / " + maxCount.ToString();
    }

}