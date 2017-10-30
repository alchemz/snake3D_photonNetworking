using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollsionNetworkScript : Photon.MonoBehaviour {

    //give the snake a unique name
    void Awake()
    {
        if (photonView.isMine)
        {
            //  PhotonView photonView = PhotonView.Get(this);
            // call a method "ChangeMyName", target all the current photon server
            photonView.RPC("ChangeMyName", PhotonTargets.AllBuffered, PhotonNetwork.playerList.Length.ToString());
        }
    }

    [PunRPC]
    void ChangeMyName(string myNewName)
    {
        gameObject.transform.name = "Snake " + myNewName;
    }

    public Transform bodyObject;
    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Orb")
        {
          //  Destroy(other.gameObject);
            if (photonView.isMine)
            {
                photonView.RPC("AddThisSnakeNewBodyPart", PhotonTargets.AllBuffered, gameObject.transform.name);
                photonView.RPC("DestroyOrb", PhotonTargets.AllBuffered, other.gameObject.name);
            }
        }
    }
    [PunRPC]
    void DestroyOrb(string _name)
    {
        Destroy(GameObject.Find(_name).gameObject);
    }

    [PunRPC]
    void AddThisSnakeNewBodyPart(string gameO)
    {
        Transform wantedPlayer = GameObject.Find(gameO.ToString()).transform;

        if (wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Count == 0)
        {
            Vector3 currentPos = wantedPlayer.position;
            Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;

            newBodyPart.GetComponent<SnakeBody>().head = wantedPlayer;
            wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Add(newBodyPart);
        }
        else
        {
            Vector3 currentPos = wantedPlayer.GetComponent<SnakeMovement>().bodyParts[wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Count - 1].position;
            Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;
            //follow the head player
            newBodyPart.GetComponent<SnakeBody>().head = wantedPlayer;
            wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Add(newBodyPart);
        }
    }
}
