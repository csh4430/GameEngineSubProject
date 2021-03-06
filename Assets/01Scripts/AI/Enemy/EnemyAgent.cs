using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgent : Agent, IHittable
{
    [field:SerializeField]
    public bool IsDead { get; private set; } = false;


    public void DieAgent()
    {
        IsDead = true;
        _controller.enabled = false;
        GameManager.Instance.KillEnemy();
        OnDied?.Invoke();
    }

    public void DamageAgent(float damage, GameObject attacker = null)
    {
        if (IsDead) return;
        Hp -= damage - damage * Defence;
        OnDamaged?.Invoke(damage, attacker);
        if (Hp <= 0)
        {
            DieAgent();
        }
    }
}
