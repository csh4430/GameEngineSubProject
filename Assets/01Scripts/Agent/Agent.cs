    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AgentAnimation))]
[RequireComponent(typeof(CharacterController))]
public class Agent : MonoBehaviour
{
    public float Hp;
    public float OriginalHp;
    public float Attack;
    public float Defence;
    public float Speed;

    protected Action OnDied;
    protected Action<float, GameObject> OnDamaged;
    protected Action<float, GameObject> OnHealed;

    private AgentAnimation _anime;
    protected AgentMove _move;
    private AgentHpBar _hpBar;
    protected AgentAudio _audio;
    protected CharacterController _controller;
    private DamageParticle _damageParticle;
    private AIBase _ai;

    protected ISkillState _skillState;
    protected virtual void Awake()
    {
        OriginalHp = Hp;
        _controller = GetComponent<CharacterController>();
        _anime = GetComponent<AgentAnimation>();
        _move = GetComponent<AgentMove>();
        _audio = GetComponent<AgentAudio>();
        _ai = transform.Find("AI")?.GetComponent<AIBase>();
        _hpBar = GetComponent<AgentHpBar>();
        _damageParticle = GetComponent<DamageParticle>();
        _skillState = GetComponentInChildren<ISkillState>();
        OnDied += () =>
        {
            _anime.PlayDieAnimation(Random.Range(1, 3));
            _audio.StopAudio();
            if(_ai != null)
                _ai.enabled = false;
        };

        OnDamaged += (damage, attacker) =>
        {
            _anime.PlayDamageAnimation();
            _damageParticle.RenderParticle(transform);
            Debug.Log($"{attacker.name} attacked with {damage} Damages.");
            _hpBar.SetHpBar(Hp, OriginalHp);
        };

        OnHealed += (value, healer) =>
        {
            StartCoroutine(HealCoroutine(value));
        };
    }
    private IEnumerator HealCoroutine(float value)
    {
        for(int i = 0; i < 10; i++)
        {
            Hp += value / 10;
            yield return new WaitForSeconds(0.1f);
            if(Hp > OriginalHp)
                Hp = OriginalHp;
            _hpBar?.SetHpBar(Hp, OriginalHp);
        }
    }

    public void StopSkill()
    {
        _skillState.IsUsingSkill = false;
    }
}
