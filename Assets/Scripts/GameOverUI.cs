using System.Collections;
  using UnityEngine;
  using UnityEngine.InputSystem;
  using UnityEngine.SceneManagement;

  public class GameOverUI : MonoBehaviour
  {
      [SerializeField] CanvasGroup gameOverGroup;
      [SerializeField] float fadeDuration = 1.5f;

      private bool waitingForInput;

      void Update()
      {
          if (!waitingForInput) return;

          if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
          {
              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
          }
      }

      public void ShowGameOver()
      {
          gameOverGroup.gameObject.SetActive(true);
          StartCoroutine(FadeIn());
      }

      private IEnumerator FadeIn()
      {
          float timer = 0f;
          gameOverGroup.alpha = 0f;

          while (timer < fadeDuration)
          {
              timer += Time.unscaledDeltaTime;
              gameOverGroup.alpha = timer / fadeDuration;
              yield return null;
          }

          gameOverGroup.alpha = 1f;
          waitingForInput = true;
      }
  }