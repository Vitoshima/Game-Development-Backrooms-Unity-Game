using UnityEngine;

public class EnemyTriggerBridge : MonoBehaviour
{
    [Header("Enemy Reference")]
    public EnemyController enemyController;   // Drag your enemy GameObject here

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyController.OnPlayerEnter(other.transform);
            Debug.Log("Enemy started chasing player");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyController.OnPlayerExit();
            Debug.Log("Enemy stopped chasing - player left trigger");
        }
    }
}