using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    private Transform closestPlayer, parent;
    private GameObject parentHolder;
    public float speed = 300f;
    public float NextWaypointDist=1f;

    Path path;
    int currentWaypoint = 0;
    Seeker seeker;
    Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0, 0.5f);

        parentHolder = GameObject.FindGameObjectsWithTag("Spawnerholder")[0];

        foreach (Transform child in parentHolder.transform)//Identifies which spawner spawned this minion and sets it in correct place in the hierarchy
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

    void UpdatePath()
    {
        FindclosestPlayer();
        if (seeker.IsDone() && closestPlayer!=null)
        seeker.StartPath(rb.position, closestPlayer.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < NextWaypointDist)
        {
            currentWaypoint++;
        }
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            transform.position = Vector2.MoveTowards(transform.position, collision.transform.position, -1 * 50 * Time.deltaTime);
        }
    }

    public void SetParent(Transform p)
    {
        this.transform.SetParent(p);
    }
}
