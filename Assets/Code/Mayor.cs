using UnityEngine;
using System.Collections;

public class Mayor : MonoBehaviour 
{
	public Vector3 TargetPosition { get; set; }

    float targetAngle = 0.0f;
    Animator animator;

    FixedJoint joint;
    GameObject currentItem;

    RadialMenu currentMenu;

    const float PickupRadius = 5.0f, MaxThrowRadius = 8.0f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    Vector3 BallisticVelocity(Vector3 delta)
    {
        float height = delta.y;
        delta.y = 0.0f;
        float dist = delta.magnitude;
        delta /= dist;

        float throwAngle = 0.7853982f;
        return new Vector3(delta.x * Mathf.Sin(throwAngle), Mathf.Cos(throwAngle), delta.z * Mathf.Sin(throwAngle)) * Mathf.Clamp(dist, 0.1f, MaxThrowRadius);
    }

    void Update()
    {
        if (currentMenu != null)
        {
            if (currentMenu.ShowingMenu)
                return;
            else
            {
                currentMenu = null;
                return;
            }
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        Vector3 delta = move.magnitude < 0.5f ? Vector3.zero : move.normalized * 5.0f;

        delta.y = rigidbody.velocity.y;
        rigidbody.velocity = delta;

        if (delta.sqrMagnitude > 0.1f)
            targetAngle = Mathf.Rad2Deg * -Mathf.Atan2(delta.z, delta.x) + 90.0f;
        rigidbody.rotation = Quaternion.Euler(0.0f, Mathf.LerpAngle(rigidbody.rotation.eulerAngles.y, targetAngle, Time.deltaTime * 5.0f), 0.0f);

        animator.SetFloat("Speed", move.magnitude);

        //Try and pick up or throw a item
        if (Input.GetMouseButtonUp(0))
        {
            if (currentItem == null && (TargetPosition - transform.position).magnitude < PickupRadius)
            {
                foreach (RaycastHit hit in Physics.SphereCastAll(new Ray(TargetPosition + Vector3.down * 10.0f, Vector3.up), 1.0f))
                {
                    if (hit.transform.tag == "PickupItem")
                    {
                        currentItem = hit.transform.gameObject;
                        currentItem.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                        break;
                    }
                    else if (hit.transform.tag == "RadialMenuItem")
                    {
                        currentMenu = hit.collider.GetComponent<RadialMenu>() ?? hit.collider.transform.parent.GetComponent<RadialMenu>();
                        currentMenu.OpenMenu();
                        break;
                    }
                }
            }
            else if (currentItem != null)
            {
                Destroy(joint);
                joint = null;

                Vector3 crateDelta = TargetPosition - currentItem.transform.position;
                targetAngle = Mathf.Rad2Deg * -Mathf.Atan2(crateDelta.z, crateDelta.x) + 90.0f;

                currentItem.rigidbody.constraints = RigidbodyConstraints.None;
                currentItem.rigidbody.velocity = BallisticVelocity(crateDelta);

                Crate crate = currentItem.GetComponent<Crate>();
                if (crate != null)
                    crate.OpenOnImpact(TargetPosition);

                currentItem = null;
            }
        }

        //Put down the item
        if (Input.GetMouseButtonUp(1) && currentItem != null)
        {
            Destroy(joint);
            joint = null;

            currentItem.rigidbody.constraints = RigidbodyConstraints.None;
            currentItem.rigidbody.velocity = (transform.forward + Vector3.down) * 3.0f;
            currentItem = null;
        }

        //Gradually position the item over the mayor's head, then attach a fixed joint.
        if (currentItem != null && joint == null)
        {
            Vector3 targetPos = transform.position + new Vector3(0.0f, 2.6f, 0.0f);
            if ((currentItem.transform.position - targetPos).magnitude > 0.1f)
                currentItem.transform.position = Vector3.Lerp(currentItem.transform.position, targetPos, Time.deltaTime * 15.0f);
            else
            {
                currentItem.transform.position = targetPos;
                currentItem.rigidbody.constraints = RigidbodyConstraints.None;
                joint = gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = currentItem.rigidbody;
            }
        }
    }
}
