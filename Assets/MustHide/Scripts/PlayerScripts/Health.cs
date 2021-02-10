using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Health : MonoBehaviour
{
    [SerializeField]
    private bool isMonster;
    [SerializeField]
    private float MaxHealth;
    [SerializeField]
    private float _HP = 100;

    [SerializeField]
    private int PlayerTeam;

    private bool isDead;

    public float HP { get { return _HP; }}

    private bool canHeal;

    //Components


    //Checking Triggers


    private void Update()
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;


        if (_HP <= 0 && !isDead)
        {
            GetComponent<PhotonView>().RPC("Dead", RpcTarget.AllBuffered);
        }
    }


    public void SetPlayerTeam(int team)
    {
        PlayerTeam = team;
    }


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
            GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered , collision.gameObject.GetComponent<BulletScript>().BulletDamage);
        }
    }
    //Healing By Time
    IEnumerator Heal()
    {
        for (float currentHealth = _HP; currentHealth <= 100; currentHealth += 5f)
        {
            if (!canHeal)
                break;

            _HP = currentHealth;
            yield return new WaitForSeconds(1f);
        }
    }


    [PunRPC]
    public void DoDamage(float Damage)
    {
        _HP -= Damage;
    }

    [PunRPC]
    public void Dead()
    {
        isDead = true;
        GetComponent<PlayerMovement>().SR.enabled = false;
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

}
