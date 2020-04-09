using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMoves : MonoBehaviour
{
    private PhotonView PV;
    public Transform spawnPoint, firePoint, closestPlayer;
    private float spawnTime = -1f, spawnDelay = 1f, damage = 20, angle = 0, fireTime = -1f, baddieCount;
    public float fireDelay = .5f, maxBaddies=6f;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        FindclosestPlayer();//Finds the closest player to this enemy
        baddieCount = 0;
        foreach (Transform child in GetComponentsInChildren<Transform>())//Counts current amount of minions spawned
        {
            if (child.tag == "Baddie")
            {
                baddieCount++;
            }
        }
        if (closestPlayer)
        {
            if ((baddieCount < maxBaddies) && (Time.time > spawnTime) && (Vector2.Distance(closestPlayer.position, transform.position) < 20))
            {
                spawnTime = Time.time + spawnDelay;
                PV.RPC("RPC_SpawnMinion", RpcTarget.AllViaServer);
            }
            if ((Time.time > fireTime) && (Vector2.Distance(closestPlayer.position, transform.position) < 20))
            {
                fireTime = Time.time + fireDelay;
                angle += 10;
                PV.RPC("RPC_FireProjectile", RpcTarget.AllViaServer);
            }
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

    [PunRPC]
    void RPC_SpawnMinion()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var minion = PhotonNetwork.InstantiateSceneObject(System.IO.Path.Combine("PhotonPrefabs", "Enemy2"),
            spawnPoint.transform.position, Quaternion.Euler(0, 0, 0), 0);
            minion.GetComponent<RangedEnemyMovement>().SetParent(this.transform);
        }
    }

    [PunRPC]
    void RPC_FireProjectile()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var bulletRef = PhotonNetwork.InstantiateSceneObject(System.IO.Path.Combine("PhotonPrefabs", "BossProjectile"),
                firePoint.transform.position, Quaternion.Euler(0, 0, angle - 90), 0);
            bulletRef.GetComponent<EnemyBullet>().SetDamageAndOwner(damage, this.transform);
            Physics2D.IgnoreCollision(bulletRef.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
        }
    }
}