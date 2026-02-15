using UnityEngine;
using UnityEngine.UI; // Необходимо для работы с компонентами UI
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement; // Для перезагрузки сцены, если понадобится

[System.Serializable]
public class RacerResult
{
	public string snakeName;
	public float time;
	public int rank;
}

public class RaceManager : MonoBehaviour
{
	public static RaceManager Instance;

	[Header("Participants")]
	public GameObject playerPrefab;
	public GameObject botPrefab;
	public Transform[] spawnPoints;
	public Transform[] circuitWaypoints;

	[Header("Race State")]
	public bool isRaceStarted = false;
	private float startTime;
	public List<RacerResult> results = new List<RacerResult>();

	[Header("UI Settings")]
	[Tooltip("Перетащите сюда панель финиша из Canvas")]
	public GameObject finishCanvas;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		// При старте убеждаемся, что панель финиша скрыта
		if (finishCanvas != null) finishCanvas.SetActive(false);

		SpawnParticipants();
		isRaceStarted = true;
		startTime = Time.time;
	}

	void SpawnParticipants()
	{
		if (spawnPoints.Length > 0)
		{
			GameObject p = Instantiate(playerPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
			p.name = "Player";
		}

		for (int i = 1; i < spawnPoints.Length; i++)
		{
			GameObject bot = Instantiate(botPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
			bot.name = "Bot_" + i;
			var ai = bot.GetComponent<SnakeBotAI>();
			if (ai != null) ai.waypoints = circuitWaypoints;
		}
	}

	public void FinishRacer(string n)
	{
		// Если гонка уже не идет или участник уже в списке — выходим
		if (!isRaceStarted || results.Any(r => r.snakeName == n)) return;

		// Добавляем результат
		results.Add(new RacerResult
		{
			snakeName = n,
			time = Time.time - startTime,
			rank = results.Count + 1
		});

		Debug.Log(n + " финишировал на " + results.Count + " месте!");

		// Логика для ботов
		if (n.Contains("Bot"))
		{
			GameObject botObj = GameObject.Find(n);
			if (botObj != null) Destroy(botObj);
		}

		// Логика для игрока
		if (n == "Player")
		{
			isRaceStarted = false; // Останавливаем гонку
			ShowFinishMenu();      // Показываем меню
		}
	}

	private void ShowFinishMenu()
	{
		if (finishCanvas != null)
		{
			finishCanvas.SetActive(true); // Включаем панель финиша

			// Разблокируем курсор, чтобы игрок мог нажать кнопки в меню
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			// Опционально: можно поставить игру на паузу
			// Time.timeScale = 0f; 
		}
		else
		{
			Debug.LogWarning("Finish Canvas не назначен в инспекторе RaceManager!");
		}
	}

	// Метод для кнопки "Начать заново" (пригодится для UI)
	public void RestartRace()
	{
		Time.timeScale = 1f; // Сбрасываем паузу, если она была
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}