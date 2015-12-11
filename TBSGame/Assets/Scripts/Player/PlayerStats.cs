using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {
    [SerializeField] private int movement = 3;
    [SerializeField] private int attackRange = 1;
    [SerializeField] private int health = 10;
    [SerializeField] private int attack = 3;
    [SerializeField] private int defense = 1;

    public int Movement
    { get { return movement; } }

    public int AttackRange
    { get { return attackRange; } }

    public int Attack
    { get { return attack; } }

    public int Defense
    { get { return defense; } }


    public void TakeDamage(int attackDamage, GameObject damageIndicator, Canvas canvas)
    {
        GameObject dIndicator = GameObject.Instantiate(damageIndicator);
        dIndicator.GetComponent<DamageUI>().setPosition(this.gameObject, canvas);
        int damage = attackDamage - defense;

        if(damage < 0)
        {
            damage = 0;
        }

        dIndicator.GetComponent<DamageUI>().setDamageText(damage);
        dIndicator.transform.SetParent(canvas.transform, false);
        if(damage == 0)
        {
            return;
        }
        health -= damage;

        if(health < 0)
        {
            health = 0;
        }

        Debug.Log(health);
    }

}
