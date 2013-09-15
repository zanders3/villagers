using UnityEngine;

public class Crate : MonoBehaviour
{
    private Vector3 targetPosition = Vector3.zero;
    public GameObject Prefab;

    public void OpenOnImpact(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    void Update()
    {
        if (targetPosition != Vector3.zero && targetPosition.y < 2.0f)
        {
            Vector3 delta = (targetPosition - transform.position);
            if (delta.sqrMagnitude > 1.0f)
            {
                rigidbody.constraints = RigidbodyConstraints.FreezePosition;
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5.0f);
            }
            else
            {
                Destroy(gameObject);
                targetPosition = Vector3.zero;
                GameObject.Instantiate(Prefab, new Vector3(transform.position.x, 0.0f, transform.position.z), Quaternion.identity);
            }
        }
    }
}
