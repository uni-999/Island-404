using UnityEngine;

public class OffTrackDetector : MonoBehaviour
{
    public Terrain terrain;                  // Drag твой Terrain объект в инспекторе (или оставь пустым — найдёт автоматически)
    public int[] onTrackIndices = { 0, 1 };    // Индексы слоёв, которые считаются трассой (0 = первый слой, обычно песок; 1 = второй, обычно dirt). Проверь в Paint Texture!
    public float onTrackMultiplier = 1.5f;   // Ускорение на трассе (x1.5)
    public float offTrackMultiplier = 1.0f;  // Базовая скорость за трассой (ощущается как замедление)

    private SnakeController snakeController; // Ссылка на твой основной скрипт

    void Start()
    {
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain; // Автоматически найдёт активный Terrain
        }

        snakeController = GetComponent<SnakeController>();
        if (snakeController == null)
        {
            Debug.LogError("SnakeController не найден на этом объекте! Добавь его на голову змеи.");
        }
    }

    void Update()
    {
        // Raycast вниз от головы змеи (чуть выше, чтобы точно попадать)
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 15f))
        {
            // Проверяем, что попали в Terrain
            if (hit.collider is TerrainCollider)
            {
                int dominantIndex = GetDominantTexture(hit.point);

                // Является ли текущий слой трассой?
                bool isOnTrack = false;
                foreach (int index in onTrackIndices)
                {
                    if (dominantIndex == index)
                    {
                        isOnTrack = true;
                        break;
                    }
                }

                // Меняем скорость в SnakeController
                snakeController.speedMult = isOnTrack ? onTrackMultiplier : offTrackMultiplier;
            }
        }
    }

    // Функция определения главной текстуры под точкой (по alphamap Terrain)
    private int GetDominantTexture(Vector3 worldPosition)
    {
        if (terrain == null || terrain.terrainData == null) return -1;

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        // Переводим мировые координаты в координаты alphamap
        float normalizedX = (worldPosition.x - terrainPos.x) / terrainData.size.x;
        float normalizedZ = (worldPosition.z - terrainPos.z) / terrainData.size.z;

        int mapX = Mathf.FloorToInt(normalizedX * terrainData.alphamapWidth);
        int mapZ = Mathf.FloorToInt(normalizedZ * terrainData.alphamapHeight);

        // Защита от выхода за границы
        mapX = Mathf.Clamp(mapX, 0, terrainData.alphamapWidth - 1);
        mapZ = Mathf.Clamp(mapZ, 0, terrainData.alphamapHeight - 1);

        // Получаем alphamap (вес текстур)
        float[,,] alphamap = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        // Находим слой с максимальным весом
        int maxIndex = 0;
        float maxWeight = 0f;
        for (int i = 0; i < terrainData.alphamapLayers; i++)
        {
            float weight = alphamap[0, 0, i];
            if (weight > maxWeight)
            {
                maxWeight = weight;
                maxIndex = i;
            }
        }

        return maxIndex;
    }
}