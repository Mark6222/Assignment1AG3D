using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    int health;
    void Start()
    {
        health = 100;
    }

    void Update()
    {
        
    }
    public int GetHealth(){return health;}
    public void setHealth(int amount){
        health = amount;
        if(health <= 0) DestroyNPC();
    }
    void DestroyNPC(){Destroy(gameObject);}
    public void DecreaseHealth(int amount){health = health - amount;}
}
