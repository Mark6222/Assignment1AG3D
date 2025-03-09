using UnityEngine;

public class Ambush : MonoBehaviour
{
    [SerializeField] private Animator anim;
    void Start()
    {
        anim = GameObject.Find("Type3").GetComponent<Animator>();
    }
    void OnTiggerEnter(Collider other)
    {
        Debug.Log("Ambush");
        anim.SetTrigger("goToAmbush");
    }
}
