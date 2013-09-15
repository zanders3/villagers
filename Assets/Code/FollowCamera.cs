using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	public Mayor Target;
    public Vector3 Offset;
    public float Speed = 2.0f;

    bool moveToTarget = false;
    public float MaxDistance = 2.0f;

    void LateUpdate()
    {
        Vector3 delta = Target.transform.position + Offset - transform.position;
        if (!moveToTarget && delta.magnitude >= MaxDistance)
            moveToTarget = true;
        if (moveToTarget && delta.magnitude < MaxDistance - 1.0f)
            moveToTarget = false;
        
        if (moveToTarget)
            transform.position = Vector3.Lerp(transform.position, Target.transform.position + Offset, Time.deltaTime * Speed);

        //Track targeted position
        Target.TargetPosition = Vector3.zero;
        foreach (RaycastHit hit in Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)))
        {
            if (hit.collider.tag == "Environment")
            {
                Target.TargetPosition = hit.point;
                break;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Target.TargetPosition != Vector3.zero)
            Gizmos.DrawWireSphere(Target.TargetPosition, 1.0f);
    }
}
