using Photon.Pun;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    private PhotonView PV;
    private Rigidbody2D rb;
    public Collider2D arm1, arm2;
    public Transform spinPoint1, spinPoint2, middle;
    private float armAngle = 270, armMoveCount = 1;
    private bool goingDown = true, firstboost=true;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(arm1, GetComponent<Collider2D>(), true);
        Physics2D.IgnoreCollision(arm2, GetComponent<Collider2D>(), true);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, middle.position, 3*Time.deltaTime);

        if(transform.position == middle.position)
        {
            arm1.gameObject.SetActive(true);
            arm2.gameObject.SetActive(true);
            MoveArms();
            firstboost = true;
        }
        else
        {
            arm1.gameObject.SetActive(false);
            arm2.gameObject.SetActive(false);
        }
        if (armMoveCount % 3 == 0 && firstboost)
        {
            rb.velocity = Random.onUnitSphere * 30;
            firstboost = false;
            armMoveCount++;
        }
    }

    private void MoveArms()
    {
        armAngle = spinPoint2.rotation.eulerAngles.z;
        if (armAngle >= 270)
        {
            armMoveCount++;
            goingDown = true;
        }
        else if (armAngle <= 90)
        {
            goingDown = false;
        }
        if (goingDown)
        {
            spinPoint1.Rotate(new Vector3(0, 0, .5f));
            spinPoint2.Rotate(new Vector3(0, 0, -.5f));
        }
        else
        {
            spinPoint1.Rotate(new Vector3(0, 0, -.5f));
            spinPoint2.Rotate(new Vector3(0, 0, .5f));
        }
    }
}
