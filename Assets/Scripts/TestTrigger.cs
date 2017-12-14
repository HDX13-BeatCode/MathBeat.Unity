using UnityEngine;
using MathBeat.Core;
using System.Collections;

public class TestTrigger : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Log.Debug("{0} has entered the trigger!", other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Log.Debug("{0} has exited the trigger!", other.name);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Log.Debug("{0} is inside the trigger!", other.name);
    }
}
