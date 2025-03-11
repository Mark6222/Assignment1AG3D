using UnityEngine;

public class Ambush : MonoBehaviour
{
    [SerializeField] private Animator anim;
    void Start()
    {
        anim = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Animator>();
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("gheuifehn");
        if(other.gameObject.tag == "Player") anim.SetTrigger("goToAmbush");
    }
}
