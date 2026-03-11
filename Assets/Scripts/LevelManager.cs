using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int enemiesRemaining;
    public GameObject gate;

    void Start()
    {
        // Count enemies automatically at level start
        //enemiesRemaining = FindObjectsByType<Enemy>().Length;
    }

    public void EnemyKilled()
    {
        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            UnlockGate();
        }
    }

    void UnlockGate()
    {
        Debug.Log("All enemies defeated! Gate unlocked.");

        if (gate != null)
        {
            gate.SetActive(false); // removes the gate
        }
    }
}