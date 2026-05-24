using UnityEngine;

public class AmbulanceFollower : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform target;
    public float moveSpeed = 8f;
    public float rotationSpeed = 5f;
    public float stopDistance = 2f;
    public Vector3 rotationOffset = Vector3.zero;
    public float waitTimeAtVictim = 3f;
    
    private bool reachedTarget = false;
    private bool returningHome = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float waitTimer = 0f;
    private NPCDeath victimScript;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        reachedTarget = false;
        returningHome = false;

        if (target != null)
        {
            victimScript = target.GetComponent<NPCDeath>();
            if (victimScript == null) victimScript = target.GetComponentInParent<NPCDeath>();
        }
    }

    void Update()
    {
        if (returningHome)
        {
            MoveTowards(startPosition);
            if (Vector3.Distance(transform.position, startPosition) < 0.5f)
            {
                Destroy(gameObject, 1f); 
                returningHome = false; 
            }
            return;
        }

        if (reachedTarget)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtVictim)
            {
                returningHome = true;
            }
            return;
        }

        if (target != null)
        {
            MoveTowards(target.position);
            if (Vector3.Distance(transform.position, target.position) < stopDistance)
            {
                reachedTarget = true;
                waitTimer = 0f;
                Debug.Log("Ambulance reached the victim!");

                if (victimScript != null)
                {
                    victimScript.TriggerDisappear();
                }
            }
        }
    }

    void MoveTowards(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position);
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(destination.x, transform.position.y, destination.z), moveSpeed * Time.deltaTime);
    }
}
