using UnityEngine;

public class EnemyNoPassiveDamage : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Prevent any physics collision damage
        if (collision.gameObject.CompareTag("Player"))
        {
            // Do nothing - we only want damage from the actual attack
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Prevent any trigger-based passive damage
        if (other.CompareTag("Player"))
        {
            // Do nothing
        }
    }
}