//проверить и изменить
if (Time.timeSinceLevelLoad > 120f && !bossSpawned)
{
    Instantiate(miniBossPrefab, spawnPoint.position, Quaternion.identity);
    bossSpawned = true;
}
