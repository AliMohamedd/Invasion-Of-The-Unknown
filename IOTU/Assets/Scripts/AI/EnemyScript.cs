//V1.8
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using IOTU;
using System.Threading;
using System.Threading.Tasks;

// Enum to define different states for the enemy
public enum EnemyState
{
    Patrolling,
    Chasing,
    InvestigatingSound,
    ReturningToPatrol,
    Attacking
}

public class EnemyScript : MonoBehaviour
{
    // Public variables for setting up various parameters in the inspector
    public float detectionRange = 10f;
    public float chaseRange = 20f;
    public float searchRadius = 30f;
    public float searchInterval = 5f;
    public float rotationSpeed = 5f;
    public float chasingRotationSpeed = 10f;
    public LayerMask obstacleLayer;
    public LayerMask hidingSpotLayer;
    public float hearingRange = 5f;
    public float soundDetectionDelay = 1f;
    public AudioSource ghostAudio;
    public float visionAngle = 60f;
    public float visionDistance = 10f;
    public float timeToLosePlayer = 5f; // Time to wait before returning to patrol after losing sight of the player
    public float attackRange = 2f; // Distance at which the enemy attacks the player

    // Private variables for internal use
    private Transform player;
    private Vector3 lastKnownPlayerPosition;
    private NavMeshAgent agent;
    private EnemyState currentState = EnemyState.Patrolling;
    private float timeSinceLostPlayer = 0f; // Time since the player was lost
    private bool waitingForNextPoint = false;
    public List<Transform> myPoints; // List of patrol points
    private int currentWayToPointIndex = 0; // Index of the current patrol point
    private bool reversing = false; // Flag for reversing patrol points
    
    // Variables for set difficulty of enemy Searching
    VideoSettingsSO m_VideoSettingsSO;
    string[] difficulty = { "Easy", "Medium", "Hard" };
    int pointer;

    // Start is called before the first frame update
    void Start()
    {
        m_VideoSettingsSO = Resources.Load<VideoSettingsSO>("VideoGraphics/VideoSettings_Data"); //Load data from jason file
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            pointer = PlayerPrefs.GetInt("Difficulty");
        }
        else
        {
            pointer = m_VideoSettingsSO.pointerDiff;
        }
        

        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        GameEvents.GameMonesterFoundMe?.Invoke(false);
        SetDifficultyLevel(difficulty[pointer]);
    }

    // Update is called once per frame
    void Update()
    {
        HandleState(); // Handle the current state of the enemy

        if (currentState == EnemyState.ReturningToPatrol && !waitingForNextPoint)
        {
            StartCoroutine(WaitAndCheckNextPoint(5f)); // Wait and check the next patrol point
        }
    }

    // Method to handle the enemy's current state
    void HandleState()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol(); // Patrol behavior
                break;
            case EnemyState.Chasing:
                ChasePlayer(); // Chasing behavior
                break;
            case EnemyState.InvestigatingSound:
                InvestigateSound(); // Investigating sound behavior
                break;
            case EnemyState.ReturningToPatrol:
                ReturnToPatrol(); // Returning to patrol behavior
                break;
            case EnemyState.Attacking:
                Attack(); // Attacking behavior
                break;
        }

        // Check if the player is within the vision cone
        if (IsPlayerInVisionCone())
        {
            currentState = EnemyState.Chasing; // Switch to chasing state
            lastKnownPlayerPosition = player.position; // Update the last known player position
            timeSinceLostPlayer = 0f; // Reset the time since the player was lost
        }
        else if (currentState == EnemyState.Chasing)
        {
            timeSinceLostPlayer += Time.deltaTime; // Increment the time since the player was lost
            if (timeSinceLostPlayer >= timeToLosePlayer)
            {
                currentState = EnemyState.ReturningToPatrol; // Switch to returning to patrol state
                timeSinceLostPlayer = 0f; // Reset the time since the player was lost
            }
        }

        // If not chasing or investigating sound, continue patrolling
        if (currentState != EnemyState.Chasing && currentState != EnemyState.InvestigatingSound)
        {
            Patrol();
        }

        // Check if the enemy is close enough to attack the player
        if (Vector3.Distance(transform.position, player.position) <= attackRange && currentState == EnemyState.Chasing)
        {
            currentState = EnemyState.Attacking; // Switch to attacking state
        }
    }

    // Method to check if the player is within the vision cone
    bool IsPlayerInVisionCone()
    {
        Vector3 directionToPlayer = player.position - transform.position; // Direction to the player
        float angle = Vector3.Angle(transform.forward, directionToPlayer); // Angle between forward direction and direction to player
        if (angle <= visionAngle / 2) // Corrected vision angle check
        {
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, visionDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true; // Player is within the vision cone
                }
            }
        }
        return false;
    }

    // Method to handle patrolling behavior
    void Patrol()
    {
        if (myPoints.Count == 0 || agent == null) return;

        // Check distance to current waypoint
        float distanceToWayPoint = Vector3.Distance(myPoints[currentWayToPointIndex].position, transform.position);

        // Check if close enough to switch to next waypoint
        if (distanceToWayPoint <= 2f)
        {
            // Move to the next waypoint
            if (reversing)
            {
                currentWayToPointIndex--;
                if (currentWayToPointIndex <= 0)
                {
                    reversing = false;
                    currentWayToPointIndex = 0;
                }
            }
            else
            {
                currentWayToPointIndex++;
                if (currentWayToPointIndex >= myPoints.Count - 1)
                {
                    reversing = true;
                    currentWayToPointIndex = myPoints.Count - 1;
                }
            }
        }

        // Set destination to current waypoint
        agent.SetDestination(myPoints[currentWayToPointIndex].position);
        RotateTowards(myPoints[currentWayToPointIndex].position - transform.position, rotationSpeed); // Rotate towards the patrol point
    }

    // Method to handle chasing behavior
    void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > chaseRange && !IsPlayerInVisionCone())
        {
            if (lastKnownPlayerPosition == Vector3.zero)
            {
                currentState = EnemyState.Patrolling;
            }
            else
            {
                CheckTheNearestPoints();
            }
            timeSinceLostPlayer = 0f; // Reset the time since the player was lost
        }
        else
        {
            agent.SetDestination(player.position); // Set the destination to the player position
            lastKnownPlayerPosition = player.position;
            RotateTowards(lastKnownPlayerPosition - transform.position, chasingRotationSpeed); // Rotate towards the player
        }
    }

    // Method to handle investigating sound behavior
    void InvestigateSound()
    {
        agent.SetDestination(lastKnownPlayerPosition); // Set the destination to the last known player position
        RotateTowards(lastKnownPlayerPosition - transform.position, rotationSpeed); // Rotate towards the player

        if (Vector3.Distance(transform.position, lastKnownPlayerPosition) <= 1f)
        {
            currentState = EnemyState.Patrolling; // Switch to patrolling state
        }
    }

    // Method to handle returning to patrol behavior
    void ReturnToPatrol()
    {
        CheckTheNearestPoints(); // Check and set the nearest patrol point
        agent.SetDestination(myPoints[currentWayToPointIndex].position); // Set the destination to the nearest patrol point
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentState = EnemyState.Patrolling; // Switch to patrolling state
        }
    }

    // Method to rotate towards a given direction
    void RotateTowards(Vector3 direction, float speed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction); // Calculate the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime); // Smoothly rotate towards the target
    }

    // Method to check and set the nearest patrol point
    void CheckTheNearestPoints()
    {
        if (myPoints.Count == 0)
        {
            //Debug.LogWarning("No patrol points defined.");
            return;
        }

        currentWayToPointIndex = myPoints
            .Select((point, index) => new { point, index }) // Create a collection of points with their indices
            .OrderBy(pair => Vector3.Distance(pair.point.position, lastKnownPlayerPosition)) // Order by distance to the player
            .First().index; // Get the index of the nearest point

        lastKnownPlayerPosition = myPoints[currentWayToPointIndex].position; // Set the last known player position to the nearest point
        currentState = EnemyState.Patrolling; // Switch to patrolling state
    }

    // Coroutine to wait and check the next patrol point
    IEnumerator WaitAndCheckNextPoint(float waitTime)
    {
        waitingForNextPoint = true; // Set the waiting flag
        yield return new WaitForSeconds(waitTime); // Wait for the specified time
        currentState = EnemyState.Patrolling; // Switch to patrolling state
        waitingForNextPoint = false; // Reset the waiting flag
    }

    // Method to chose the difficulty of the enemy searching 
    public void SetDifficultyLevel(string level)
        {
            switch (level)
            {
                case "Easy":
                    detectionRange = 5f;
                    chaseRange = 10f;
                    searchRadius = 15f;
                    visionAngle = 45f;
                    visionDistance = 5f;
                    break;
                case "Medium":
                    detectionRange = 10f;
                    chaseRange = 20f;
                    searchRadius = 30f;
                    visionAngle = 60f;
                    visionDistance = 10f;
                    break;
                case "Hard":
                    detectionRange = 15f;
                    chaseRange = 30f;
                    searchRadius = 45f;
                    visionAngle = 75f;
                    visionDistance = 15f;
                    break;
                default:
                    Debug.LogWarning("Unknown difficulty level");
                    break;
            }
        }

    // Method to handle attacking behavior
    void Attack()
    {
        // Implement your attack logic here
        GameEvents.GameMonesterFoundMe?.Invoke(true);
        UIEvents.LoseScreenShown?.Invoke();
        GameEvents.GameLost?.Invoke();
        enabled = false;
        //Task.Delay(2000);
        StartCoroutine(load_again());
       

        //Debug.Log("Player caught! Game Over.");
        //SceneManager.LoadScene("GameOverScene"); // Load the game over scene, ali will add it 

    }
    
    // Mehtod to handle loaded scecnes
    private void SceneHandler()
    {
        SceneEvents.LastSceneUnloaded?.Invoke();
        SceneEvents.SceneIndexLoaded?.Invoke(2);
    }

    // Coroutine to wait and load the current level after catching
    private IEnumerator load_again()
    {

        yield return new WaitForSeconds(5);
        UIEvents.GamePlayScreenShown?.Invoke();
        SceneHandler();


    }
}

