using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransformScript : Photon.MonoBehaviour {
    void Start()
    {
        //move the mine snake only
     
        PhotonNetwork.sendRate = 20;
        PhotonNetwork.sendRateOnSerialize = 20;
    }
    void FixedUpdate()
    {
        SmoothMovement();
    }

    void SmoothMovement()
    {
        //slowly change the current position to the dest
        if (photonView.isMine)
        {

        }
        else
        {
            //sync the transform
            transform.position = Vector3.Lerp(transform.position, realPosition, Time.deltaTime*5);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, Time.deltaTime * 5);
        }
    }
    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //if writing something to the newtwork
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //if reading 
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
        }
    }

}
