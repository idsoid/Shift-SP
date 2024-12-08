using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text playerHealthText;
    [SerializeField] TMP_Text playerAmmoText;
    [SerializeField] TMP_Text gameTimerText;
    private float timer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: optimize to use gamemanager?
        playerHealthText.text = "Health: " + GameManager.Instance.GetPlayer().GetComponent<ITarget>().health + "/100";
        playerAmmoText.text = "Bullets: " + InventoryController.Instance.GunEquipped.BulletCount + "/" + InventoryController.Instance.GunEquipped.MagSize;
        timer += Time.deltaTime;
        gameTimerText.text = "Timer: " + timer;
    }
}
