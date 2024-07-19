using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private Button playAgainBtn;

	private void Start()
	{
		Board.Instance.OnGameOver += Board_OnGameOver;
        playAgainBtn.onClick.AddListener(RestartGame);

        Hide();
	}

    private void RestartGame()
    {
        Board.Instance.RestartGame();
        Hide();
    }

	private void Board_OnGameOver(object sender, System.EventArgs e)
	{
        Show();
	}

	private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
