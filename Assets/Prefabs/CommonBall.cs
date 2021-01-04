using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonBall : MonoBehaviour
{
    public bool isAdd;

    public int kindOfId;

    public void ResetColor()
    {
        GetComponent<Renderer>().material.SetFloat("_Metallic", 0.0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
