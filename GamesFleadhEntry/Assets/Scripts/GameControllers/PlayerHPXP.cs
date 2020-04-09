using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPXP : MonoBehaviour, IPunObservable
{
    public float playerHealth, playerMaxHealth = 100f, playerXp, playerDamage = 20f, damageCD = .5f, damageCDTime = -1;
    public int potionCount = 0, currentLevel = 1, statPoints = 0;
    public bool isAlive = true;
    public Healthbar healthbar, XPBar;
    public PlayerMovements pm;
    private PhotonView PV;
    public Text XPtext, statPointsText, HpText, potionCounterTxt;
    public GameObject LevelUpUI;
    private Transform closestLivingPlayer;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        playerHealth = playerMaxHealth;
        if (PV.IsMine)
        {
            PV.RPC("RPC_AddChar", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedChar);
            healthbar.SetMaxBar(playerMaxHealth);
            SetXpBarValue();
        }
    }

    private void Update()
    {
        if (isAlive)
        {
            healthbar.SetCurrent(playerHealth);
            HpText.text = playerHealth + "/" + playerMaxHealth;
            closestLivingPlayer = null;
            potionCounterTxt.text = "Health Potions: " + potionCount;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            if (!PV.IsMine)
            {
                transform.GetComponent<PlayerMovements>().ActivePlayer(false);
            }
            FindclosestPlayer();

            if (Input.GetKeyDown(KeyCode.E) && potionCount > 0)
            {
                playerHealth += 20;
                potionCount--;

                if (playerHealth > playerMaxHealth)
                {
                    playerHealth = playerMaxHealth;
                }
            }
            if (playerHealth <= 0)
            {
                isAlive = false;
            }

            if (statPoints > 0)
            {
                LevelUpUI.SetActive(true);
                statPointsText.text = "Stat Points Available:" + statPoints;
            }
            else
            {
                LevelUpUI.SetActive(false);
            }
        }
        else
        {
            PV.RPC("RPC_IsDead", RpcTarget.AllBuffered);
        }
    }
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isAlive);
            stream.SendNext(playerHealth);
            stream.SendNext(playerMaxHealth);
        }
        else if (stream.IsReading)
        {
            try
            {
                isAlive = (bool)stream.ReceiveNext();
                playerHealth = (float)stream.ReceiveNext();
                playerMaxHealth = (float)stream.ReceiveNext();
            }catch (InvalidCastException){}
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.IsChildOf(this.transform))
        {
            if (collision.gameObject.tag == "EnemyProjectile" && Time.time > damageCDTime)
            {
                damageCDTime = Time.time + damageCD;
                Debug.Log("You got Shot");
                playerHealth -= collision.transform.GetComponent<EnemyBullet>().GetDamage();
            }
            else if ((collision.gameObject.tag == "Baddie" || collision.gameObject.tag == "Baddie1") && Time.time > damageCDTime)
            {
                damageCDTime = Time.time + damageCD;
                Debug.Log("You got Stabbed");
                playerHealth -= collision.transform.GetComponent<EnemyHealth>().GetMeleeDamage();
            }
        }
        if (collision.gameObject.tag == "Potion")
        {
            potionCount += 4;
        }
    }

    public void AddExp(float xp)
    {
        playerXp += xp;
        LevelTracker();
        SetXpBarValue();
    }

    public void LevelTracker()
    {
        if (playerXp >= (currentLevel * currentLevel) * 100)
        {
            currentLevel++;
            statPoints += (int)Mathf.Ceil(currentLevel / 3);
        }
    }

    private void SetXpBarValue()
    {
        XPBar.SetCurrent(playerXp);
        XPBar.SetMaxBar((currentLevel * currentLevel) * 100);
        XPtext.text = currentLevel.ToString() + ":  " + playerXp.ToString() + "/" + ((currentLevel * currentLevel) * 100);

    }

    public void IncreaseHealth()
    {
        if (statPoints > 0)
        {
            playerMaxHealth += 25f;
            playerHealth += 25;
            ReduceStatPoints();
            healthbar.SetMaxBar(playerMaxHealth);
        }
    }

    public void ReduceStatPoints()
    {
        statPoints--;
    }

    public int GetStatPoints()
    {
        return statPoints;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public void Revive(Transform revPoint)
    {
        transform.position = revPoint.position;
        playerHealth = playerMaxHealth;
        isAlive = true;
    }
    private void FindclosestPlayer()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (go.transform.GetComponent<PlayerHPXP>().IsAlive() && go.transform != this.transform)
            {
                float temp = Vector2.Distance(go.transform.position, transform.position);
                if (!closestLivingPlayer)
                {
                    closestLivingPlayer = go.transform;
                }
                else if (temp < Vector2.Distance(closestLivingPlayer.transform.position, transform.position))
                {
                    closestLivingPlayer = go.transform;
                }
            }
        }
    }

    [PunRPC]
    void RPC_IsDead()
    {
    foreach (Transform child in transform)
        {
            if (child.transform.name != "Camera")
            {
                child.gameObject.SetActive(false);
            }
        }
    }

}
