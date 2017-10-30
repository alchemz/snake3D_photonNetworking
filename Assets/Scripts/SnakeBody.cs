using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour {
    private int myOrder; //snake body prefabs attached in order, so that it can follow the one infront
    public Transform head; //track the head position

	// when the game starts, this is the first to initialize
	void Start () {
       // head = GameObject.FindGameObjectWithTag("Player").gameObject.transform;

        //access the bodyparts transform created in SnakeMovements
        for(int i=0; i< head.GetComponent<SnakeMovement>().bodyParts.Count; i++)
        {
            if(gameObject == head.GetComponent<SnakeMovement>().bodyParts[i].gameObject)
            {
                myOrder = i;
            }
        }
	}

    private Vector3 movementVelocity;
    [Range(0.0f, 1.0f)]
    public float overTime = 0.5f;
	// Update is called once per frame
	void FixedUpdate () {
		if(myOrder == 0)
        {
            //the first snake body part follows the head
            transform.position = Vector3.SmoothDamp(transform.position, head.position, ref movementVelocity, overTime);
            transform.LookAt(head.transform.position);
        }
        else
        {
            //if not first, then follow the second one
            transform.position = Vector3.SmoothDamp(transform.position, 
                                                    head.GetComponent<SnakeMovement>().bodyParts[myOrder - 1].position,
                                                    ref movementVelocity, overTime);
            transform.LookAt(head.transform.position);
        }
	}
}
