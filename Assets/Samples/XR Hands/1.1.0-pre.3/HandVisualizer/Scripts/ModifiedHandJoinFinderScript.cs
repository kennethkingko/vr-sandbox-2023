using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiedHandJoinFinderScript : MonoBehaviour
{       

    [SerializeField]
    public GameObject testlmao;

    public GameObject L_thumbtip;
    public GameObject L_middletip;

    // Start is called before the first frame update
    void Start()
    {
        L_thumbtip = GameObject.Find("Hand Visualizer/LeftHandDebugDrawJoints/ThumbTip");
        L_middletip = GameObject.Find("Hand Visualizer/LeftHandDebugDrawJoints/MiddleTip");

    }

    // Update is called once per frame
    void Update()   
    {

        float distanceX = Vector3.Distance(L_thumbtip.transform.position, L_middletip.transform.position);

        if (distanceX < 1){
            testlmao.SetActive(true);
        } else{
            testlmao.SetActive(false);
        }

        
    }
}
