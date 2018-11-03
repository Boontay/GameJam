using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    public float startingHealth = 100f;
    public float currentHealth;
    public float flashSpeed = 1f;
    public float damagePerSecond = 1f;
    PlayerMovement playerMovement;

    void Start() {
        currentHealth = startingHealth;
    }

    public float Update() {
        if (currentHealth == 0) {
            SceneManager.LoadScene("GameOver");
        } else if (Vector3.Distance(transform.position, playerMovement.GetPosition()) < 3) {
            currentHealth -= 2f * Time.deltaTime;
        } else if (playerMovement.CheckPlayerInput() == "space") {
            currentHealth -= 0.5f * Time.deltaTime;
        } else {
            currentHealth -= 1f * Time.deltaTime;
        }

        return currentHealth;
    }
}