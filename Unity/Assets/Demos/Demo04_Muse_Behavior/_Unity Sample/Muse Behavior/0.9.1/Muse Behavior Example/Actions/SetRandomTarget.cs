using System;
using UnityEngine;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;

[Serializable]
[NodeDescription(
    name: "Set Random Target", 
    description: "Assigns a target to a random object matching a given tag.", 
    story: "Set Random Target [Target] From Tag [TagValue]", 
    id: "4daff47ae1c14ec780056d158e5e0953")]
public class SetRandomTarget : Action
{
    public BlackboardVariable<GameObject> Target;
    public BlackboardVariable<string> TagValue;

    protected override Status OnUpdate()
    {
        GameObject[] tagged = GameObject.FindGameObjectsWithTag(TagValue);
        if (tagged == null || tagged.Length == 0)
        {
            return Status.Failure;
        }
        
        int randomNumber = UnityEngine.Random.Range(0, tagged.Length);
        Target.Value = tagged[randomNumber];
        return Status.Success;
    }
}
