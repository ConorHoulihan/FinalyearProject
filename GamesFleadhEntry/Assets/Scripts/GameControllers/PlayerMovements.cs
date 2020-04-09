using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovements : MonoBehaviour, IPunObservable
{
    private float moveSpeed = 10.0f, currentSpeed, dashSpeed = 30f, playerDamage = 20, dashTimer, dashDuration = .1f, dashDelay = 1, dashTime = -1f, angle, fireTimer, fireRate = 1f, FireTime = -1f;
    public float AttackRange = 3, swingspeed =.1f;
    private bool canDash = true, canFire = true, exposition = true, swordSwing = false, gameOver=false;
    public bool meleeChar = false;

    private Rigidbody2D body;
    private Vector2 movement, mousePos;
    private PhotonView PV;
    public UnityEngine.Camera cam;
    public AudioListener audioL;
    public Transform middleP, endSwingPos, startSwingPos;
    public GameObject projectilePrefab, expositionBox, sword, SpeedUpBar, dashDelayBar, fireDelayBar, finalBoss, endDialogue;
    public Transform arm;
    public Canvas canvas;
    public LayerMask whatIsEnemy;
    public Healthbar dashBar, attackBar;
    public PlayerHPXP pHpXp;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
        {
            ActivePlayer(false);
        }
        StartCoroutine(DelaySetMelee());
    }

    void Update()
    {
        if (PV.IsMine)
        {
            dashTimer = dashTime - Time.time;
            dashBar.SetMaxBar(dashDelay);
            dashBar.SetCurrent(dashTimer);

            fireTimer = FireTime - Time.time;
            attackBar.SetCurrent(fireTimer);
            attackBar.SetMaxBar(fireRate);

            if (dashTimer < 0)
            {
                dashTimer = 0;
                canDash = true;
            }
            if (fireTimer < 0)
            {
                fireTimer = 0;
                canFire = true;
            }

            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            middleP.position = body.position - new Vector2(0, 0.45f);

            var v3 = Input.mousePosition;
            v3.z = 10.0F;

            if (swordSwing)
            {
                sword.transform.position = Vector2.MoveTowards(sword.transform.position, endSwingPos.transform.position, swingspeed);
                if (sword.transform.position == endSwingPos.transform.position)
                {
                    swordSwing = false;
                    sword.SetActive(false);
                }
            }

            mousePos = cam.ScreenToWorldPoint(v3);
            if (pHpXp.IsAlive())
            {
                if (Input.GetKeyDown(KeyCode.Space) && canFire)
                {
                    if (!meleeChar)
                    {
                        PV.RPC("RPC_FireProjectile", RpcTarget.AllViaServer);
                    }
                    else
                    {
                        sword.SetActive(true);
                        swordSwing = true;
                        sword.transform.position = startSwingPos.position;
                        FireTime = Time.time + fireRate;
                        canFire = false;
                        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(arm.position, AttackRange, whatIsEnemy);
                        for (int i = 0; i < enemiesToDamage.Length; i++)
                        {
                            enemiesToDamage[i].GetComponent<EnemyHealth>().TakeDamage(playerDamage);
                            if(enemiesToDamage[i].GetComponent<EnemyHealth>().getMaxHP()<500)
                                enemiesToDamage[i].transform.position = Vector2.MoveTowards(enemiesToDamage[i].transform.position, transform.position, -1 * 50 * Time.deltaTime);
                            if (enemiesToDamage[i].gameObject.GetComponent<EnemyHealth>().getHP() <= 0)
                            {
                                transform.GetComponent<PlayerHPXP>().AddExp(enemiesToDamage[i].gameObject.GetComponent<EnemyHealth>().GetExp());
                            }
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
                {
                    canDash = false;
                    dashTime = Time.time + dashDuration + dashDelay;
                    currentSpeed = dashSpeed;
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    exposition = false;
                }
                else if (Time.time > dashTime - dashDelay)
                {
                    currentSpeed = moveSpeed;
                }
            }
            if (!exposition)
                expositionBox.SetActive(false);
        }
        IsGameOver();
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(arm.position, AttackRange);
    }

    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            body.velocity = new Vector2(movement.x * currentSpeed, movement.y * currentSpeed);
            Vector2 aimDir = mousePos - body.position;
            angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + 90f;
            middleP.rotation.Set(0, 0, angle, 0);
            middleP.transform.eulerAngles = new Vector3(middleP.transform.eulerAngles.x, middleP.transform.eulerAngles.y, angle);
        }
    }

    public void ActivePlayer(bool isActiveP)
    {
        cam.enabled = isActiveP;
        audioL.enabled = isActiveP;
        canvas.enabled = isActiveP;
    }

    public void IncreaseDamage()
    {
        if (pHpXp.GetStatPoints() > 0)
        {
            playerDamage += 5f;
            pHpXp.ReduceStatPoints();
        }
    }

    public void IncreaseAspeed()
    {
        if (pHpXp.GetStatPoints() > 0)
        {
            fireRate -= .2f;
            swingspeed += .04f;
            pHpXp.ReduceStatPoints();
            if (fireRate < .3)
            {
                fireDelayBar.SetActive(false);
            }
        }
    }

    public void ReduceDashDelay()
    {
        if (pHpXp.GetStatPoints() > 0)
        {
            dashDelay -= .2f;
            pHpXp.ReduceStatPoints();
            if(dashDelay< .3)
            {
                dashDelayBar.SetActive(false);
            }
        }
    }

    public void MoveSpeedUp()
    {
        if (pHpXp.GetStatPoints() > 0)
        {
            moveSpeed += 3f;
            pHpXp.ReduceStatPoints();
            if (moveSpeed > 30)
            {
                SpeedUpBar.SetActive(false);
            }
        }
    }
    public void IsGameOver()
    {
        finalBoss = GameObject.FindGameObjectsWithTag("Bossholder")[0];
        if (finalBoss.transform.childCount <= 0 && !gameOver)
        {
            gameOver = true;
            transform.position = new Vector3(0, 0, 0);
            endDialogue.SetActive(true);
        }
    }


    public float GetDamage()
    {
        return playerDamage;
    }

    IEnumerator DelaySetMelee()
    {
        yield return new WaitForSeconds(1);
        foreach (Transform child in transform)
        {
            if (child.name == "PlayerAvatar2(Clone)")
            {
                meleeChar = true;
                playerDamage *= 1.5f;
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(middleP.localRotation);
            if (body)
                stream.SendNext(body.velocity);
        }
        else if (stream.IsReading)
        {
            middleP.localRotation = (Quaternion)stream.ReceiveNext();
            if (body)
                body.velocity = (Vector2)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void RPC_FireProjectile()
    {
        FireTime = Time.time + fireRate;
        canFire = false;
        if (PV.IsMine)
        {
            var bulletRef = PhotonNetwork.Instantiate(System.IO.Path.Combine("PhotonPrefabs", "Projectile"),
                arm.transform.position, Quaternion.Euler(0, 0, angle + 180), 0);
            bulletRef.GetComponent<FireBullet>().SetDamageDAndPlayer(playerDamage,this.transform);
            Physics2D.IgnoreCollision(bulletRef.GetComponent<Collider2D>(), GetComponentInChildren<Collider2D>(), true);

        }
    }
}
