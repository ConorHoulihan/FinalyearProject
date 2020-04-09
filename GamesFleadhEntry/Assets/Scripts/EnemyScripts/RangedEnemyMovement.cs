using Photon.Pun;
using UnityEngine;

public class RangedEnemyMovement : MonoBehaviour
{
    private Transform closestPlayer, parent;
    private Rigidbody2D rigidbody2d;
    public GameObject middlePoint, firePoint, parentHolder;
    private PhotonView PV;
    private float fireRate = 2f, FireTime = -1f, angle;
    public float damage = 20;
    //public GameObject prefab;

    void Start()
    {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();

        if(GameObject.FindGameObjectsWithTag("Bossholder")[0])
            parentHolder = GameObject.FindGameObjectsWithTag("Bossholder")[0];

        foreach (Transform child in parentHolder.transform)
        {
            float temp = Vector3.Distance(child.transform.position, transform.position);
            if (!parent)
            {
                parent = child.transform;
            }
            else if (temp < Vector3.Distance(parent.transform.position, transform.position))
            {
                parent = child.transform;
            }
        }
        this.transform.SetParent(parent);
    }

    void Update()
    {
        FindclosestPlayer();

        float dist = Vector2.Distance(closestPlayer.transform.position, transform.position);
        if (dist > 10f)// && dist < 30)
        {
            rigidbody2d.velocity = (closestPlayer.transform.position - transform.position).normalized * 5;
        }
        else if (dist < 8f)
        {
            rigidbody2d.velocity = -(closestPlayer.transform.position - transform.position).normalized * 10;
        }
        else
        {
            rigidbody2d.velocity = new Vector2(0, 0);
            if (PV.IsMine)
            {
                if (FireTime - Time.time < 0)
                {
                    FireTime = Time.time + fireRate;
                    PV.RPC("RPC_FireProjectile", RpcTarget.AllViaServer);
                }
            }
        }

        if (closestPlayer.position != transform.position)
        {
            Vector3 lookPos = closestPlayer.position - middlePoint.transform.position;
            angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            middlePoint.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    [PunRPC]
    void RPC_FireProjectile()
    {
        if (PV)
            if (PhotonNetwork.IsMasterClient)
            {
                {
                    var bulletRef = PhotonNetwork.InstantiateSceneObject(System.IO.Path.Combine("PhotonPrefabs", "EnemyProjectile"),
                    firePoint.transform.position, Quaternion.Euler(0, 0, angle - 90), 0);
                    bulletRef.GetComponent<EnemyBullet>().SetDamageAndOwner(damage, this.transform);
                    Physics2D.IgnoreCollision(bulletRef.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
                }
            }
    }

    public void SetParent(Transform parent)
    {
        this.transform.SetParent(parent);
    }
    private void FindclosestPlayer()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (go.GetComponent<PlayerHPXP>().IsAlive())
            {
                float temp = Vector2.Distance(go.transform.position, transform.position);
                if (!closestPlayer)
                {
                    closestPlayer = go.transform;
                }
                else if (temp < Vector2.Distance(closestPlayer.transform.position, transform.position))
                {
                    closestPlayer = go.transform;
                }
            }
            else
            {
                closestPlayer = null;
            }
        }
    }
}
