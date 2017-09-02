using UnityEngine;
using System.Collections;

public class Ripple : MonoBehaviour
{
    public Transform rippleObject;

    bool ripple = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ripple)
        {

        }
    }

    public void DoRipple(Vector2 coords)
    {
        ripple = true;
        Instantiate(rippleObject, coords, rippleObject.rotation);

    }
}
