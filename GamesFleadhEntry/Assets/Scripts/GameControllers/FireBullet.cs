using Photon.Pun;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 8f;
    public Transform closestPlayer;
    private float damageDealt;
    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
        {
            FindclosestPlayer();
            this.transform.SetParent(closestPlayer);
        }
        GetComponent<Rigidbody2D>().velocity = (transform.parent.GetComponent<Rigidbody2D>().velocity) / 4;
        Destroy(this.gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Baddie" || collision.gameObject.tag == "Baddie1")
        {
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damageDealt);
            if (collision.gameObject.GetComponent<EnemyHealth>().getMaxHP() <= 500) { 
                collision.transform.position = Vector2.MoveTowards(collision.transform.position, transform.position, -1 * 50 * Time.deltaTime);
            }
            if (transform.parent && collision.gameObject.GetComponent<EnemyHealth>().getHP() <= 0) {
                transform.parent.GetComponent<PlayerHPXP>().AddExp(collision.gameObject.GetComponent<EnemyHealth>().GetExp());
            }
        }
        Destroy(this.gameObject);
    }

    public void SetDamageDAndPlayer(float damage, Transform player)
    {
        damageDealt = damage;
        this.transform.SetParent(player);
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
