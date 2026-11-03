using UnityEngine;
using FrostGame.PlayerStuf;
using System;

public class CheckAttackScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<PlayerControl>().OnAttackChanged += HandleAttackChanged;
    }

    private void HandleAttackChanged(bool obj)
    {
        Debug.Log("Attack Pressed: " + obj);
        /*throw new NotImplementedException()*/
        ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
