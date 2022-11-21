using UnityEngine;

/// <summary>
/// Periodically spawns the specified prefab in a random location.
/// </summary>
public class Spawner : MonoBehaviour
{
    /// <summary>
    /// Object to spawn
    /// </summary>
    public GameObject Prefab;

    /// <summary>
    /// Seconds between spawn operations
    /// </summary>
    public float SpawnInterval = 10;

    /// <summary>
    /// How many units of free space to try to find around the spawned object
    /// </summary>
    public float FreeRadius = 10;

    /// Keeps track of when the next spawn should happen and updates dynamically
    public float NewSpawn = 0; 

    /// <summary>
    /// Check if we need to spawn and if so, do so.
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    void Update()
    {
        if (Time.time > NewSpawn) 
        {
            NewSpawn += SpawnInterval;
            Vector3 Position = new Vector3(SpawnUtilities.RandomFreePoint(FreeRadius).x, SpawnUtilities.RandomFreePoint(FreeRadius).y);
            Instantiate(Prefab, Position, Quaternion.identity);
        }
    }
}
