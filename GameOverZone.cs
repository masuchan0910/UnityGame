using UnityEngine;
[RequireComponent(typeof(Collider))]
public class GameOZone : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager = null;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            gameManager.GameOver();
        }
    }
}