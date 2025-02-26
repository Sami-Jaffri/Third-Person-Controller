using UnityEngine;

public class CoinController : MonoBehaviour
{
    public int coinValue = 1; 

    void Update()
    {
        transform.Rotate(1, 0, 0); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddScore(coinValue); 
                Destroy(gameObject); 
            }
        }
    }
}
