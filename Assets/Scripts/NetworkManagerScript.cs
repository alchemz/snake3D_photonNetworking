using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//first, create an array of room
//second, instantiate the snakeHead
//third, use PhotonNetwork with the same setting version
//5. if not connected, use the GUI to print out the detailed status
//6. else if no room existed, start the server
//7. create a room
public class NetworkManagerScript : MonoBehaviour {
    private RoomInfo[] roomsList;
    public GameObject ourSnakeHead;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        else if(PhotonNetwork.room == null)
        {
            if (GUI.Button(new Rect(100, 100, 250, 50), "Start Server"))
            {
                PhotonNetwork.CreateRoom("Slither room", new RoomOptions(){ MaxPlayers=15 }, null);
            }
            if(roomsList != null)
            {
                for(int i=0; i< roomsList.Length; i++)
                {
                    if(GUI.Button(new Rect(100, 250 + (110*i), 250, 50), "Join this room"))
                    {
                        PhotonNetwork.JoinRoom(roomsList[i].Name);
                    }
                }
            }
        }
    }

    void OnReceiveRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected to the room");
        PhotonNetwork.Instantiate(ourSnakeHead.transform.name, Vector3.zero, Quaternion.identity, 0);
    }


}
