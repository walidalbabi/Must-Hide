using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class Health : MonoBehaviour
{
    [SerializeField]
    private bool isMonster;
    [SerializeField]
    private float _maxHealth;


    private float _HP = 100;

    [SerializeField]
    private int PlayerTeam;

   
    public Slider slider;
    [SerializeField]
    private Image healthBarFill;
    [SerializeField]
    private Text playerNameTxt;
    [SerializeField]
    private GameObject Ghost;

    public bool isDead;

    public bool canUseAbility = true;

    public float HP { get { return _HP; }}
    public float maxHealth { get { return _maxHealth; }}


    private bool canHeal;

    private int splashIndex = 1;

    //Counter for Not Spaming Blood
    private float _bloodTimer;

    //Components
    public PhotonPlayer photonPlayer;
    public SpriteRenderer spriterenderer;
    private BoxCollider2D boxCollider;
    private AudioManager audioManager;
    private PhotonView PV;
    [SerializeField]
    private GameObject PlayerLight;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioManager = GetComponent<AudioManager>();
    }

    private void Start()
    {
        playerNameTxt.text = PV.Owner.NickName;

        Invoke("SetHealthBar", 2f);

        if (!PV.IsMine)
            PlayerLight.SetActive(false);

        if (PV.IsMine)
        {
            if (isMonster)
            {
                Camera.main.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("Portal");
                PV.RPC("RPC_SetPlayerAvatar", RpcTarget.AllBuffered, 1);
            }
            else
            {
                PV.RPC("RPC_SetPlayerAvatar", RpcTarget.AllBuffered, 2);
            }
    
        }
    }



    private void Update()
    {
        if (!PV.IsMine)
            return;

        if(_bloodTimer < 0.5f)
        {
            _bloodTimer += Time.deltaTime;
        }
    }

    //Setting Health Properties After Delay
    public void SetHealthBar()
    {
        if (PlayerTeam == InGameManager.instance.CurrentTeam)
        {

            slider.gameObject.SetActive(true);

            if (PV.IsMine)
            {
                healthBarFill.color = Color.red;
            }
            else
                healthBarFill.color = Color.green;

            _HP = _maxHealth;
            slider.maxValue = _maxHealth;
            slider.value = _HP;
        }
        else
        {
            slider.gameObject.SetActive(false);
        }
    }

    //Reenabling Health Bar After Transfering
    public void ReEnableHealthBar()
    {
        if (PlayerTeam == InGameManager.instance.CurrentTeam)
        {

            slider.gameObject.SetActive(true);

            if (PV.IsMine)
            {
                healthBarFill.color = Color.red;
            }
            else
                healthBarFill.color = Color.green;

            slider.value = _HP;
        }
        else
        {
            slider.gameObject.SetActive(false);
        }
    }

    //Setting Player Team for Network
    public void SetPlayerTeam(byte team)
    {
        PV.RPC("RPC_SetPlayerTeam", RpcTarget.AllBuffered, team);
    }

    //Checking Triggers

    private void OnTriggerEnter2D(Collider2D collision)
    {

         if(isMonster)
        if (collision.gameObject.CompareTag("HealZone") && _HP < _maxHealth)
        {
            canHeal = true;
                StartCoroutine(Heal());      
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(isMonster)
        if (collision.gameObject.CompareTag("HealZone"))
        {
            canHeal = false;
            Debug.Log("StopHealing");
            StopCoroutine(Heal());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!PV.IsMine) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {

            if (GetComponent<Bomog>())
                if (GetComponent<Bomog>().isAbility)
                    return;

            if (gameObject.tag == "Monster")
            {
                DoDamage(collision.gameObject.GetComponent<BulletScript>().BulletDamage);
                
                if (GetComponent<PropsController>())
                {
                   var script = GetComponent<PropsController>();

                    script.isBuff = true;

                    if (script.isBuff)
                        script.buffCounter = 0;
                }

               // Set Blood
            if (_bloodTimer >= 0.5f)
                {
                    PV.RPC("RPC_SpawnBlood", RpcTarget.AllBuffered);
                    _bloodTimer = 0;
                }

                audioManager.PlaySound(AudioManager.Sound.MonsterGetShot, 10f, 0, 1f, 1f, true);

                CheckIfDead();
            }

        }
    }

    private void CheckIfDead()
    {
        if (!PV.IsMine) return;

        if (_HP <= 0 && !isDead)
        {
            PV.RPC("Dead", RpcTarget.AllBuffered);
            photonPlayer.SetIsDead(true);

            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bones"), transform.position, Quaternion.identity);

            Camera.main.GetComponent<Camera>().cullingMask = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 11);

            if (isMonster)
            {
                //  InGameManager.instance.SetGameLogs(PV.Owner.NickName, "Died!", "<color=#00FF08>");
                GetComponent<AudioManager>().PlaySound(AudioManager.Sound.MonsterDead, 20f, 0, 1f, 0.75f, true);
                InGameManager.instance.UpdateMonsterDead();
                SetGameLogs(true);
            }
            else
            {
                //  InGameManager.instance.SetGameLogs(PV.Owner.NickName, "Died!", "<color=#FF0000>");
                GetComponent<AudioManager>().PlaySound(AudioManager.Sound.MonsterDead, 20f, 0, 1f, 0.75f, true);
                InGameManager.instance.UpdateHuntersDead();
                SetGameLogs(false);
            }

        }
    }

    //Healing By Time
    IEnumerator Heal()
    {
        for (float currentHealth = _HP; currentHealth <= _maxHealth; currentHealth += 5f)
        {
            if (!canHeal)
                break;

            _HP = currentHealth;
            slider.value = _HP;
            yield return new WaitForSeconds(1f);
        }
    }

    public void DoDamage(float Damage)
    {
        if (PV.IsMine)
            PV.RPC("RPC_DoDamage", RpcTarget.AllBuffered, Damage);
    }

    public void HealForAmount(float amount)
    {
        PV.RPC("RPC_HealForAmount", RpcTarget.AllBuffered, amount);
    }

    public void SetCanUseAbility(bool state)
    {
        PV.RPC("RPC_SetCanUseAbility", RpcTarget.AllBuffered, state);
        Invoke("SetBackAbility", 10f);
    }

    private void SetBackAbility()
    {
        PV.RPC("RPC_SetCanUseAbility", RpcTarget.AllBuffered, true);
    } 

    [PunRPC]
    private void RPC_SetCanUseAbility(bool state)
    {
        canUseAbility = state;
    }

    [PunRPC]
    private void RPC_DoDamage(float Damage)
    {
        _HP -= Damage;
        slider.value = _HP;
    }

    [PunRPC]
    private void RPC_SpawnBlood()
    {
        if (splashIndex == 1)
            splashIndex = 2;
        else
            splashIndex = 1;

        if (isMonster)
        {
            Instantiate((GameObject)Resources.Load(Path.Combine("PhotonPrefabs", "Blood", "BloodParticleMonster")), transform.position, Quaternion.identity);
            Instantiate((GameObject)Resources.Load(Path.Combine("PhotonPrefabs", "Blood", "Splash_" + splashIndex + "Monster")), transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate((GameObject)Resources.Load(Path.Combine("PhotonPrefabs", "Blood", "BloodParticleHunter")), transform.position, Quaternion.identity);
            Instantiate((GameObject)Resources.Load(Path.Combine("PhotonPrefabs", "Blood", "Splash_" + splashIndex + "Hunter")), transform.position, Quaternion.identity);
        }
    }

    [PunRPC]
    private void RPC_HealForAmount(float amount)
    {
        if ((_maxHealth - _HP) >= 30)
            _HP += amount;
        else
            _HP = _maxHealth;

        slider.value = _HP;
    }

    [PunRPC]
    private void RPC_SetPlayerTeam(byte pTeam) 
    {
        PlayerTeam = pTeam;      
    }

    [PunRPC]
    private void Dead()
    {
        isDead = true;
        gameObject.tag = "Untagged";

        GetComponent<PlayerMovement>().SR.enabled = false;
        boxCollider.enabled = false;

        GetComponent<PlayerMovement>().moveSpeed = 6f;

        if (GetComponent<PropsController>())
            GetComponent<PropsController>().enabled = false;  
        
        if (GetComponent<ShootingScript>())
        {
            GetComponent<ShootingScript>().Gun.gameObject.SetActive(false);
            GetComponent<ShootingScript>().enabled = false;

        }

        PlayerLight.SetActive(false);


        Ghost.SetActive(true);
        slider.gameObject.layer = 11;

    }

    [PunRPC]
    void RPC_SetPlayerAvatar(int team)
    {
        GameObject playerAvatar;

        if (team == 1)
            playerAvatar = Instantiate(InGameManager.instance.inGamePlayerAvatar, InGameManager.instance.monstersHeaderPanel);
        else playerAvatar = Instantiate(InGameManager.instance.inGamePlayerAvatar, InGameManager.instance.huntersHeaderPanel);

        var inGA = playerAvatar.GetComponent<InGamePlayerAvatar>();
        inGA.SetHealthComponent(this);
    }

    public void SetGameLogs(bool monster)
    {
        if (monster)
            PhotonNetwork.Instantiate("LogsMonster", InGameManager.instance.logsContent.position, Quaternion.identity);
        else
            PhotonNetwork.Instantiate("LogsHunter", InGameManager.instance.logsContent.position, Quaternion.identity);
    }
}
