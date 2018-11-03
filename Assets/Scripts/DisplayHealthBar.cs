using UnityEngine;
using UnityEngine.UI;

public class DisplayHealthBar : MonoBehaviour {
    public Transform player;
    public Text healthText;

    private void Update() {
        healthText.text = player.position.z.ToString("0");
    }
}
