
using UnityEngine;
using UnityEngine.AI; 

public class pumpkinjackmovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform;

    [SerializeField] public Light playerLight;
    [SerializeField] public Transform pjsp;
    PlayerMovement playe;

    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {playerTransform = player.transform;
        playe = player.GetComponent<PlayerMovement>();

        }
    }

void Update() 
{
    if (playerTransform == null) return; // Exit early if there is no player

    // 1. Move towards the player
    agent.SetDestination(playerTransform.position);

    // 2. Determine speed based on conditions
    if (playe != null && playe.hasmachete) 
    {
        // If the player has a machete, set speed to 5 regardless of light
        agent.speed = 5f; 
    } 
    else if (playerLight != null && playerLight.enabled) 
    {
        // If no machete but light is on, calculate dynamic speed
        agent.speed = (25.0f - playerLight.intensity) * 0.2f; 
    } 
    else 
    {
        // If no machete and light is off, default speed
        agent.speed = 5f; 
    }
}


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playe = collision.gameObject.GetComponent<PlayerMovement>();

            if (playe != null)
            {
                playe.Hit();
                transform.position = pjsp.position;
            }

        }
    }

}
