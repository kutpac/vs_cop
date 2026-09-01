using UnityEngine;

  public class ZombieFootstepRelay : MonoBehaviour
  {
      [SerializeField] ZombieController zombieController;

      public void PlayFootstep()
      {
          zombieController.PlayFootstep();
      }
  }