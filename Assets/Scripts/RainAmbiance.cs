using UnityEngine;

  public class RainAmbience : MonoBehaviour
  {
      [SerializeField] AudioSource ambientSource;
      [SerializeField] AudioClip ambientRainClip;

      [SerializeField] AudioSource thunderSource;
      [SerializeField] AudioClip[] thunderClips;
      [SerializeField] float minThunderInterval = 10f;
      [SerializeField] float maxThunderInterval = 30f;

      private float thunderTimer;

      void Start()
      {
          ambientSource.clip = ambientRainClip;
          ambientSource.loop = true;
          ambientSource.Play();

          ResetThunderTimer();
      }

      void Update()
      {
          thunderTimer -= Time.deltaTime;
          if (thunderTimer <= 0f)
          {
              PlayThunder();
              ResetThunderTimer();
          }
      }

      private void ResetThunderTimer()
      {
          thunderTimer = Random.Range(minThunderInterval, maxThunderInterval);
      }

      private void PlayThunder()
      {
          if (thunderClips.Length == 0) return;
          AudioClip clip = thunderClips[Random.Range(0, thunderClips.Length)];
          thunderSource.PlayOneShot(clip);
      }
  }