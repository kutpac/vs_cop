using UnityEngine;

  public class FootstepAudio : MonoBehaviour
  {
      [SerializeField] AudioSource audioSource;
      [SerializeField] AudioClip footstepSounds;
      [SerializeField] float minPitch = 0.9f;
      [SerializeField] float maxPitch = 1.1f;
      [SerializeField] float volume = 0.5f;

      public void PlayFootstep()
      {
          audioSource.pitch = Random.Range(minPitch, maxPitch);
          audioSource.PlayOneShot(footstepSounds,volume);

      }
  }