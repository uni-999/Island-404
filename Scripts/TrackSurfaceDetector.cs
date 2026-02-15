using UnityEngine;

public class TrackSurfaceDetector : MonoBehaviour
{
    public int roadTextureIndex = 1; // Индекс "NewLayer 1" из твоего фото
    public float roadSpeedMult = 1.25f; // +25% ускорение на трассе
    public float offRoadSpeedMult = 0.8f; // Замедление вне трассы (Requirement 31)

    private SnakeController snake;
    private Terrain terrain;

    void Start()
    {
        snake = GetComponent<SnakeController>();
        terrain = Terrain.activeTerrain;
    }

    void Update()
    {
        if (terrain == null) return;

        bool onRoad = IsOnRoad();
        snake.speedMult = onRoad ? roadSpeedMult : offRoadSpeedMult;
    }

    private bool IsOnRoad()
    {
        Vector3 tPos = transform.position - terrain.transform.position;
        float mapX = tPos.x / terrain.terrainData.size.x;
        float mapZ = tPos.z / terrain.terrainData.size.z;

        int x = Mathf.FloorToInt(mapX * terrain.terrainData.alphamapWidth);
        int z = Mathf.FloorToInt(mapZ * terrain.terrainData.alphamapHeight);

        if (x < 0 || z < 0 || x >= terrain.terrainData.alphamapWidth || z >= terrain.terrainData.alphamapHeight) return false;

        float[,,] alpha = terrain.terrainData.GetAlphamaps(x, z, 1, 1);
        return alpha[0, 0, roadTextureIndex] > 0.5f;
    }
}