using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject player;
    public GameObject destination;
    public Vector3 landingDisplacement;

    void OnTriggerEnter(Collider other)
    {
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = destination.transform.position + landingDisplacement;
        player.GetComponent<CharacterController>().enabled = true;
    }
}
