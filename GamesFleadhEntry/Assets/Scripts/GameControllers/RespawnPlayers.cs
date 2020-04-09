using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlayers : MonoBehaviour
{
    public List<GameObject> players;
    public int alivePlayerCount;

    void Update()
    {
        alivePlayerCount = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (!players.Contains(go))
            {
                players.Add(go);
            }
            if (go.GetComponent<PlayerHPXP>().IsAlive())
            {
                alivePlayerCount++;
            }
        }
        if (alivePlayerCount == 0)
        {
            StartCoroutine(DelayRespawn());
        }
    }

    IEnumerator DelayRespawn()
    {
        yield return new WaitForSeconds(1);
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerHPXP>().Revive(this.transform);
        }
    }
}
