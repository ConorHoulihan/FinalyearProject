using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float speed = 10, damage = 20;

    void Update()
    {
        transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision);
        if (collision.gameObject.tag == "Baddie" || collision.gameObject.tag == "Baddie1")
        {
            Physics2D.IgnoreCollision(collision.transform.GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void SetDamageAndOwner(float d, Transform player)
    {
        damage = d;
        this.transform.SetParent(player);
    }
    public float GetDamage()
    {
        return damage;
    }
}
