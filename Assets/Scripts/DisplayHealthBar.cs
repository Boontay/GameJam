using UnityEngine;
using UnityEngine.UI;

public class DisplayHealthBar : MonoBehaviour {
    public Transform player;
    public Text healthText;
    private PlayerHealth ph;

    private void Update() {
        float health = Mathf.Floor(ph.Update());
        healthText.text = health.ToString();
    }
}
