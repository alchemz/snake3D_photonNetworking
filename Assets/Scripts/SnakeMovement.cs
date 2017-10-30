using System.Collections;
//use list
using System.Collections.Generic;
using UnityEngine;



public class SnakeMovement : Photon.MonoBehaviour{
    //make a list to store position of the snakebody parts' position
    public List<Transform> bodyParts = new List<Transform>();

   
    // Update is called once per frames
    void Update () {
        if (photonView.isMine)
        {
            // InputRotation();    
            MouseRotationSnake();
         
            SpawnOrbManager();

            Running();
            Scaling();
        }
        ColorMySnake();

        if (velocity > 0.12f)
        {
            MakeOurSnakeGlow(true);
        }
        else
            MakeOurSnakeGlow(false);
    }

    public Material blue, navy;
    void ColorMySnake()
    {
        for(int i=0; i<bodyParts.Count; i++)
        {
            if(i%2 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = blue;
            }
            else
            {
                bodyParts[i].GetComponent<Renderer>().material = navy;
            }
        }
    }

    public float spawnOrbEveryXSeconds = 3;
    public GameObject orbPrefab;
    void SpawnOrbManager()
    {
        //IEnumerator runs every seconds
        StartCoroutine("CallEveryFewSeconds", spawnOrbEveryXSeconds);

    }
    IEnumerator CallEveryFewSeconds(float x)
    {
        yield return new WaitForSeconds(x);
        float radiusSpawn = 5;
        //assign random position to the new orbs
        Vector3 randomNewOrbPosition = new Vector3(
                Random.Range(
                        Random.Range(transform.position.x - 10, transform.position.x -5),
                        Random.Range(transform.position.x +5, transform.position.x+10)
                    ),
                Random.Range(
                        Random.Range(transform.position.y - 10, transform.position.y - 5),
                        Random.Range(transform.position.y + 5, transform.position.y + 10)
                    ),
                0
            );
        Vector3 direction = randomNewOrbPosition - transform.position;
        Vector3 finalPosition = transform.position + (direction.normalized * radiusSpawn);
        //initalize a random name
        int randomNumber = Random.Range(1, 10000);
        //use the spawnforeverybody method
        photonView.RPC("SpawnForEverybody", PhotonTargets.AllBuffered, finalPosition, "orb "+randomNumber);
      
        StopCoroutine("CallEveryFewSeconds");
    }

    [PunRPC]
    void SpawnForEverybody(Vector3 _finalPos, string _name)
    {
        GameObject newOrb = Instantiate(orbPrefab, _finalPos, Quaternion.identity) as GameObject;
        newOrb.transform.name = _name;
        //position in the empty orb
        GameObject orbParent = GameObject.Find("Orbs");
        newOrb.transform.parent = orbParent.transform;
    }
    private Vector3 pointInWorld;
    private Vector3 mousePosition;
    private float radius = 3.0f;
    private Vector3 direction;

    void MouseRotationSnake()
    {
        //ray for collision
        //get the point hit by mouse to unity world
        Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 1000.0f);

        mousePosition = new Vector3(hit.point.x, hit.point.y, 0);
        direction = Vector3.Slerp(direction,mousePosition - transform.position, Time.deltaTime*1);
        //direction = mousePosition - transform.position;
        direction.z = 0;

        pointInWorld = direction.normalized *radius + transform.position;
        transform.LookAt(pointInWorld);
    }
    void InputRotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            currentRotation += rotationSensitity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            currentRotation -= rotationSensitity * Time.deltaTime;
        }
    }
    public float speed = 3.5f;
    public float currentRotation;
    public float rotationSensitity = 50.0f;
    private Vector3 lastFramePosition;
    private float velocity;
    void FixedUpdate()
    {
        if (photonView.isMine)
        {
            MoveForward();
            // Rotation();
            ApplyStatsForBody();
            CameraFollow();
        }
        velocity = Vector3.Magnitude(transform.position - lastFramePosition);
        //Debug.Log("velocity: " + velocity);
        lastFramePosition = transform.position;//store the last frame position
    }

    //Y: up
    void MoveForward()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void Rotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation));
    }

    [Range(0.0f, 1.0f)]
    public float smoothTime = 0.5f;
    void CameraFollow()
    {
        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
        //make it follow the player
        Vector3 cameraVelocity = Vector3.zero;
        camera.position = Vector3.SmoothDamp(camera.position, 
                                             new Vector3(
                                                gameObject.transform.position.x,
                                                gameObject.transform.position.y,
                                                -10), 
                                             ref cameraVelocity, 
                                             smoothTime);
    }

    private int orbCounter;
    private int currentOrb;
    public int[] growOnThisOrb; //a list of orb
    private Vector3 currentSize = Vector3.one;
    public float growthRate = 0.1f;
    public float bodyPartOverTimeFollow = 0.19f;
    bool SizeUp(int x)
    {
        try
        {
            //if not grow, it will shrink
            if (x == growOnThisOrb[currentOrb])
            {
                currentOrb++;
                return false;
            }
            else
            {
                return false;
            }
        }
        catch (System.Exception e)
        {
            print("No more grow from this points, add more rows" + e.StackTrace.ToString());
        }
        return false;
    }
    /*
    public Transform bodyObject;
    void OnCollisionEnter(Collision other)
    {
        //if hit, destory the object
        if(other.transform.tag == "Orb")
        {
            Destroy(other.gameObject);
           // orbCounter++;
            if (SizeUp(orbCounter) == false)
            {
                orbCounter++;
                //body parts if the transform for bodypart position
                if (bodyParts.Count == 0)
                {
                    Vector3 currentPos = transform.position;
                    Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;

                   // newBodyPart.localScale = currentSize;//update new body part
                    //newBodyPart.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;

                    bodyParts.Add(newBodyPart);
                }
                else
                {
                    Vector3 currentPos = bodyParts[bodyParts.Count - 1].position;
                    Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;

                  //  newBodyPart.localScale = currentSize;//update new body part
                   // newBodyPart.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;

                    bodyParts.Add(newBodyPart);
                }
            }
            
        }
    }
    */
    private bool running;
    public float speedWhileRunning = 6.5f;
    public float speedWhileWalking = 3.5f;
    public float bodyPartFollowTimeWalking = 0.19f;
    public float bodyPartFollowTimeRunning = 0.1f;
    void Running()
    {
       // MakeOurSnakeGlow(running);
        if(bodyParts.Count > 2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                speed = speedWhileRunning;
                running = true;
                bodyPartOverTimeFollow = bodyPartFollowTimeRunning;
            }
            if (Input.GetMouseButtonUp(0))
            {
                speed = speedWhileWalking;
                running = false;
                bodyPartOverTimeFollow = bodyPartFollowTimeWalking;
            }
        }
        else
        {
            speed = speedWhileWalking;
            running = false;
            bodyPartOverTimeFollow = bodyPartFollowTimeWalking;
        }

        if (running == true)
        {
            //if we are on local server
            if (photonView.isMine)
            {
                StartCoroutine("LoseBodyParts");
            }
        }
        else
        {
            bodyPartOverTimeFollow = bodyPartFollowTimeWalking;
        }
    }

    IEnumerator LoseBodyParts()
    {
        yield return new WaitForSeconds(0.5f);
        if (photonView.isMine)
            photonView.RPC("LoseBodyPartsSync", PhotonTargets.AllBuffered);   

        StopCoroutine("LoseBodyParts");
    }

    [PunRPC]
    void LoseBodyPartsSync()
    {
        int lastIndex = bodyParts.Count - 1;
        Transform lastBodyPart = bodyParts[lastIndex].transform;

        Instantiate(orbPrefab, lastBodyPart.position, Quaternion.identity);

        bodyParts.RemoveAt(lastIndex);
        Destroy(lastBodyPart.gameObject);

        orbCounter--;
    }



    void MakeOurSnakeGlow(bool areWeRunning)
    {
        foreach(Transform bodyParts_x in bodyParts)
        {
            if (bodyParts_x.name == "glow")
            {
                this.gameObject.SetActive(areWeRunning);
            }
        }
    }
    

    private Vector3 headV;
    void ApplyStatsForBody()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, currentSize, ref headV, 0.5f);
        foreach (Transform bodyPart_x in bodyParts)
        {
            bodyPart_x.localScale = transform.localScale;
            bodyPart_x.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;
        }
    }

    public List<bool> scalingTrack;
    private int howBigAreWe_questionmark;
    public float followTimeSensitivity;
    public float scaleSensitivity;
    void Scaling()
    {
        scalingTrack = new List<bool>(new bool[growOnThisOrb.Length]);
        howBigAreWe_questionmark = 0;
        for(int i=0; i<growOnThisOrb.Length; i++)
        {
            if(orbCounter >= growOnThisOrb[i])
            {
                scalingTrack[i] = !scalingTrack[i];//return the bool
                howBigAreWe_questionmark++;
            }
        }

       
        currentSize = new Vector3(
             1+ (howBigAreWe_questionmark * scaleSensitivity),
             1 + (howBigAreWe_questionmark * scaleSensitivity),
             1 + (howBigAreWe_questionmark * scaleSensitivity)
             );
        bodyPartFollowTimeWalking = (howBigAreWe_questionmark / 100.0f) + followTimeSensitivity;
        bodyPartFollowTimeRunning = bodyPartFollowTimeWalking / 2;
    }
}
