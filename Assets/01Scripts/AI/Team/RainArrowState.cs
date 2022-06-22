using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ToolBox.Pools;

public class RainArrowState : AIState, ISkillState
{
    [field: SerializeField]
    public float CoolDown { get; set; }
    private float _coolDown = 0;

    [field: SerializeField]
    public bool IsUsingSkill { get; set; } = false;

    [field: SerializeField]
    public Slider CoolDownSilder { get; set; }
    public override Action OnStateAction { get; set; }

    [field: SerializeField]
    public override List<AITransition> Transition { get; set; }

    public GameObject Circle;
    public float Range;

    [SerializeField]
    private Transform _basePos;
    private AgentAnimation _anim;


    private void Awake()
    {
        _anim = _basePos.GetComponent<AgentAnimation>();
        OnStateAction += () =>
        {
            if (IsUsingSkill)
                return;
            if (_coolDown > 0)
                return;
            IsUsingSkill = true;
            StartCoroutine(Check());
        };
    }

    private IEnumerator Check()
    {
        RaycastHit hit;
        GameObject circle = Circle.Reuse();
        circle.GetComponent<Radius>().SetRadius(Range);
        _anim.PlaySpecialAnimation();
        Vector3 targetPos;
        while (true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Floor"));
            if (hit.collider != null)
            {
                targetPos = new Vector3(hit.point.x, _basePos.position.y, hit.point.z);
                _basePos.LookAt(targetPos);
                circle.transform.position = hit.point;
                if (Input.GetMouseButton(0))
                {
                    IsUsingSkill = false;
                    _coolDown = CoolDown;
                    circle.Release();
                    _anim.RePlay();
                    Collider[] victims = Physics.OverlapSphere(hit.point, Range, LayerMask.GetMask("Enemy"));
                    foreach(Collider c in victims)
                    {
                        IHittable iHit = c.GetComponent<IHittable>();
                        iHit.DamageAgent(5, _basePos.gameObject);
                    }
                    yield break;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void Update()
    {
        if (_coolDown > 0)
        {
            _coolDown -= Time.deltaTime;
            if (_coolDown < 0)
                _coolDown = 0;
            CoolDownSilder.value = _coolDown / CoolDown;
        }
    }
}
