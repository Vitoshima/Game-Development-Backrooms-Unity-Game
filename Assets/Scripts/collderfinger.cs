using UnityEngine;

public class ColliderFinder : MonoBehaviour
{
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Hitting: " + hit.gameObject.name + " at " + hit.gameObject.transform.position);
    }
}