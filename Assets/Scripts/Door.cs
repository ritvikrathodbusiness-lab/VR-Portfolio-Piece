using UnityEngine;

public class Door : MonoBehaviour
{
    #region Variables
    [Header("Door Settings")]
    public float openSpeed = 2f;

    [Tooltip("How far each panel slides open, in local units, along local Z.")]
    public float openDistance = 5f;

    [Tooltip("Stop lerping once within this distance of the target — avoids " +
             "running Update forever since Lerp never exactly reaches its target.")]
    public float snapThreshold = 0.001f;

    [SerializeField] private Transform doorLeftTransform;
    [SerializeField] private Transform doorRightTransform;

    bool canOpen = false;
    Vector3 doorLeftClosedPos;
    Vector3 doorRightClosedPos;
    #endregion

    void Start()
    {
        doorLeftClosedPos = doorLeftTransform.localPosition;
        doorRightClosedPos = doorRightTransform.localPosition;
    }

    void Update()
    {
        Vector3 leftTarget = canOpen
            ? doorLeftClosedPos + new Vector3(0f, 0f, openDistance)
            : doorLeftClosedPos;

        Vector3 rightTarget = canOpen
            ? doorRightClosedPos + new Vector3(0f, 0f, -openDistance)
            : doorRightClosedPos;

        // Skip the lerp entirely once close enough — saves doing this math
        // every frame indefinitely while the door sits idle open or closed.
        if (Vector3.Distance(doorLeftTransform.localPosition, leftTarget) > snapThreshold)
        {
            doorLeftTransform.localPosition = Vector3.Lerp(doorLeftTransform.localPosition, leftTarget, openSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(doorRightTransform.localPosition, rightTarget) > snapThreshold)
        {
            doorRightTransform.localPosition = Vector3.Lerp(doorRightTransform.localPosition, rightTarget, openSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered door trigger"); 
            canOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canOpen = false;
        }
    }
}