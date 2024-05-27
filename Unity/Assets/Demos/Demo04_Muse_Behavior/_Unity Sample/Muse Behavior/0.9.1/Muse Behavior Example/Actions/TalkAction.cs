using System;
using UnityEngine;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;

[Serializable]
[NodeDescription(
        name: "Talk", 
        description: "Show text in world-space above the Agent with the Sentence for a specified Duration.", 
        story: "[Agent] says [Sentence]", 
        id: "c70ad05265db4a5a8623561f705aefab")]
public class TalkAction : Action
{
    public BlackboardVariable<GameObject> Agent;
    public BlackboardVariable<string> Sentence;
    public BlackboardVariable<float> Duration = new BlackboardVariable<float>(2.0f);

    private float m_WaitTimer;
    private Animator m_Animator;
    private GameObject m_TextObject;
    private GameObject m_TextMeshAsset;
    private static readonly int s_Talking = Animator.StringToHash("Talking");

    protected override Status OnStart()
    {
        if (!Agent?.Value)
        {
            return Status.Failure;
        }

        // Create the floating text object.
        if (m_TextMeshAsset == null)
        {
            m_TextMeshAsset = Resources.Load<GameObject>("TextMesh Speech Preset");
        }
        CreateTextObject(Sentence, Agent);

        // Animate the character.
        m_Animator = Agent.Value.GetComponent<Animator>();
        if (m_Animator != null)
        {
            m_Animator.SetBool(s_Talking, true);
        }
        
        // Start the timer.
        m_WaitTimer = Duration.Value;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Update the text's orientation.
        m_TextObject.transform.rotation = GetTextLookRotation();

        m_WaitTimer -= Time.deltaTime;
        if (m_WaitTimer < 0)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (m_TextObject != null)
        {
            UnityEngine.Object.Destroy(m_TextObject);
            m_TextObject = null;
        }
        if (m_Animator != null)
        {
            m_Animator.SetBool(s_Talking, false);
        }
    }

    public void CreateTextObject(string text, GameObject parent)
    {
        Vector3 pos = GetBoundsOffset(parent);
        m_TextObject = UnityEngine.Object.Instantiate(m_TextMeshAsset, parent.transform, true);
        m_TextObject.GetComponent<TMPro.TextMeshPro>().text = text;

        m_TextObject.transform.localPosition = pos;
        m_TextObject.transform.rotation = GetTextLookRotation();
    }

    Vector3 GetBoundsOffset(GameObject gameObject)
    {
        MeshFilter parentMesh = gameObject.GetComponent<MeshFilter>();
        if (parentMesh != null)
        {
            return GetBoundsOffset(parentMesh.mesh.bounds);
        }
        Collider parentCollider = gameObject.GetComponent<Collider>();
        if (parentCollider != null)
        {
            return GetBoundsOffset(parentCollider.bounds);
        }
        return Vector3.zero;
    }

    Vector3 GetBoundsOffset(Bounds bounds)
    {
        return new Vector3(0.0f, bounds.max.y + 0.05f);
    }

    Quaternion GetTextLookRotation()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0.0f;
        cameraForward.Normalize();
        return Quaternion.LookRotation(cameraForward);
    }
}
