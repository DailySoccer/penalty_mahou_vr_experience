using UnityEngine;
using System.Collections;

public class Chivato : MonoBehaviour {

    Animation anim;
    AnimationState state;
    public Transform LHand;
    public Transform RHand;

    public Vector3 Init;
    public Vector3 LHandPosition;
    public Vector3 RHandPosition;

    public float Time;


    Transform FindChild(string name, Transform transform)
    {
        foreach (Transform child in transform)
        {
            if (child.name == name)
            {
                return child;
            }
            else {
                var ret = FindChild(name, child);
                if (ret != null) return ret;
            }
        }
        return null;
    }

    // Use this for initialization
    void Start () {
        Init = transform.position;
        anim = GetComponent<Animation>();
        state = anim[anim.clip.name];

        LHand = FindChild("Bip001 L Hand", transform);
        RHand = FindChild("Bip001 R Hand", transform);
    }

    // Update is called once per frame
    void Update () {
        Time = state.time;
        LHandPosition = LHand.position - Init;
        RHandPosition = RHand.position - Init;
    }
}
