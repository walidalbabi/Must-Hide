using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField]
    private float MaxHealth;
    [SerializeField]
    private float _HP = 100;

    public float HP { get { return _HP; }}

    private bool canHeal;


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



    public void DoDamage(float Damage)
    {
        _HP -= Damage;
    }

}
