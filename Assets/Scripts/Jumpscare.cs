using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    public Animation JumpscareAnimation;
    public AudioSource JumpscareAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            JumpscareAnimation.Play();

            if (JumpscareAudio != null)
            {
                JumpscareAudio.Play();
            }

            this.gameObject.SetActive(false);
        }
    }
}