using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [Header("Settings")]
    public float targetY = 5f;        // Global Y height when at the top
    public float speed = 2f;          // Movement speed
    public bool startAtTop = false;   // If true, elevator begins at targetY

    private Vector3 bottomPos;
    private Vector3 topPos;

    private void Start()
    {
        // Bottom is always global Y = 0
        bottomPos = new Vector3(transform.position.x, 0f, transform.position.z);
        topPos = new Vector3(transform.position.x, targetY, transform.position.z);

        // Apply the correct starting position
        transform.position = startAtTop ? topPos : bottomPos;

        StartCoroutine(ElevatorLoop());
    }

    private IEnumerator ElevatorLoop()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, bottomPos) < 0.1f)
                yield return MoveTo(topPos);
            else
                yield return MoveTo(bottomPos);

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }
    }
}
