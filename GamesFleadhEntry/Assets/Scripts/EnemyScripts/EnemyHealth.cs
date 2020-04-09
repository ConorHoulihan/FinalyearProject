using Photon.Pun;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPunObservable
{
    public float currentHP = 100, maxHP = 100, Meleedamage = 25;

    public void TakeDamage(float damage)
    {
        GetComponent<PhotonView>().RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    public float GetExp()
    {
        return maxHP;
    }

    public float getHP()
    {
        return currentHP;
    }

    public float getMaxHP()
    {
        return maxHP;
    }

    public float GetMeleeDamage()
    {
        return Meleedamage;
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        currentHP -= damage;
        //Debug.Log("Hell yeah brother");

        if (currentHP <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       // throw new System.NotImplementedException();
    }
}
