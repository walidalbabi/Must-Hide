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
    private float MaxHealth;


    private float _HP = 100;

    [SerializeField]
    private int PlayerTeam;

    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Image healthBarFill;
    [SerializeField]
    private Text playerNameTxt;

    public bool isDead;

    public float HP { get { return _HP; }}

    private bool canHeal;

    private int splashIndex = 1;
    //Components
    public PhotonPlayer photonPlayer;

    private void Start()
    {
        playerNameTxt.text = GetComponent<PhotonView>().Owner.NickName;

        if (PlayerTeam == InGameManager.instance.CurrentTeam)
        {
            if (GetComponent<PhotonView>().IsMine)
            {
                healthBarFill.color = Color.red;
            }
            else
                healthBarFill.color = Color.green;

            _HP = MaxHealth;
            slider.maxValue = MaxHealth;
            slider.value = _HP;
        }
        else
        {
             slider.gameObject.SetActive(false);
        }

    

    }



    private void Update()
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;


        if (_HP <= 0 && !isDead)
        {
            GetComponent<PhotonView>().RPC("Dead", RpcTarget.AllBuffered);
            photonPlayer.SetIsDead(true);
        }
    }


    private void SetPhotonPlayerDelady()
    {
        GetComponent<PhotonView>().RPC("RPC_SetPlayerPhoton", RpcTarget.AllBuffered);
    }

    //Setting Player Team for Network
    public void SetPlayerTeam(byte team)
    {
       GetComponent<PhotonView>().RPC("RPC_SetPlayerTeam", RpcTarget.AllBuffered, team);
    }

    //Checking Triggers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HealZone") && _HP < MaxHealth)
        {
            canHeal = true;
                StartCoroutine(Heal());      
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HealZone"))
        {
            canHeal = false;
            Debug.Log("StopHealing");
            StopCoroutine(Heal());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {

            if (gameObject.tag == "Monster")
            {
                GetComponent<PhotonView>().RPC("RPC_DoDamage", RpcTarget.AllBuffered, collision.gameObject.GetComponent<BulletScript>().BulletDamage);

                GetComponent<PropsController>().isBuff = true;

                if (GetComponent<PropsController>().isBuff)
                    GetComponent<PropsController>().buffCounter = 0;

                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Blood", "BloodParticle"), transform.position, Quaternion.identity);

                if (splashIndex == 1)
                    splashIndex = 2;
                else
                    splashIndex = 1;
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Blood", "Splash_"+splashIndex), transform.position, Quaternion.identity);

                GetComponent<AudioManager>().PlaySound(AudioManager.Sound.MonsterGetShot, 10f, 0,1f, true);

            }
           
        }
    }

    //Healing By Time
    IEnumerator Heal()
    {
        for (float currentHealth = _HP; currentHealth <= MaxHealth; currentHealth += 5f)
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
        GetComponent<PhotonView>().RPC("RPC_DoDamage", RpcTarget.AllBuffered, Damage);
    }

    [PunRPC]
    private void RPC_DoDamage(float Damage)
    {
        _HP -= Damage;
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
        GetComponent<PlayerMovement>().gameObject.SetActive(false);
        GetComponent<PlayerMovement>().enabled = false;
        if (GetComponent<PropsController>())
            GetComponent<PropsController>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;


        if (isMonster)
        {
            InGameManager.instance.UpdateMonsterDead();
        }
        else
        {
            InGameManager.instance.UpdateHuntersDead();
        }

    }

    [PunRPC]
    void RPC_SetPlayerPhoton()
    {
#pragma warning disable CS1717 // Assignment made to same variable
        photonPlayer = photonPlayer;
#pragma warning restore CS1717 // Assignment made to same variable
    }

}
