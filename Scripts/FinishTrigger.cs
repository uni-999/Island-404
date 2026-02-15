using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем имя объекта или тег
        if (other.name.Contains("Player") || other.name.Contains("Bot"))
        {
            RaceManager.Instance.FinishRacer(other.name);
        }
    }
}