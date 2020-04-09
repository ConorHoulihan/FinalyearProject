using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPathfinders : MonoBehaviour
{
    private Transform closestPlayer;
    private PhotonView PV;
    public float Maxbaddies, baddieCount, spawnTime = -1f, spawnDelay = .5f;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        FindclosestPlayer();
        baddieCount = 0;
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.tag == "Baddie1")
            {
                baddieCount++;
            }
        }
        if (Time.time > spawnTime && Vector2.Distance(closestPlayer.position, transform.position) < 30 && baddieCount < Maxbaddies)
        {
            spawnTime = Time.time + spawnDelay;
            PV.RPC("RPC_SpawnMinions", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    void RPC_SpawnMinions()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var minion = PhotonNetwork.InstantiateSceneObject(System.IO.Path.Combine("PhotonPrefabs", "Enemy"),
                transform.position+new Vector3(0,3,0), Quaternion.Euler(0, 0, 0), 0);
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
}


