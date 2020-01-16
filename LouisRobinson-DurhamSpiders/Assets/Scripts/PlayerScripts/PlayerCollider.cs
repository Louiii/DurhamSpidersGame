using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollider : MonoBehaviour
{
    [HideInInspector]
    public int Boosts;
    public GameObject damage;
    public GameManager gm;


    void Start()
    {
        damage.active = true;
        damage.GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Spider")
        {
            damage.GetComponent<Image>().CrossFadeAlpha(0.6f, 0.01f, false);
            damage.GetComponent<Image>().CrossFadeAlpha(0.0f, 10.0f, false);

            hit.gameObject.GetComponent<SpiderMovement>().enabled = false;
            hit.gameObject.tag = "DeadSpider";
            gm.RefreshList();
            hit.gameObject.GetComponent<Animation>().Play("death2");

            gm.LostLife();
        } 
        else if (hit.gameObject.tag == "PowerUp")
        {
            Destroy(hit.gameObject);
            Boosts += 1;
        }
        else if (hit.gameObject.tag == "Treasure")
        {
            Destroy(hit.gameObject);
            gm.FoundTreasure();
        }

    }


}
