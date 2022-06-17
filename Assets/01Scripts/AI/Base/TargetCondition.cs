using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCondition : AICondition
{
    public Transform BasePos;
    public float Distance;
    public LayerMask layerMask;
    public override bool CheckCondition()
    {
        Transform target = TargetSetter.SetTarget(BasePos, Distance, layerMask);
        if(target != null)
            return true;
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}