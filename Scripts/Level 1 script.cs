using UnityEngine;
using UnityEngine.SceneManagement;
public class Level1script : MonoBehaviour
{
	public void StartFirstLevel()
	{
		SceneManager.LoadScene(1);
	}
	public void StartSecondLevel()
	{
		SceneManager.LoadScene(2);
	}
	public void StartThirstLevel()
	{
		SceneManager.LoadScene(3);
	}

	public void GameExit()
	{
		Application.Quit();
	}
}	
