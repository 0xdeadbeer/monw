using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public TMP_Text questionTMP;
    public Vector3 direction = Vector3.down;
    public float speed = 10f;
    
    void Start()
    {
        
    }

    void Update()
    {
        this.transform.Translate(direction * speed * Time.deltaTime);
    }
}
