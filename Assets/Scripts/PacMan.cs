using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour {

    public AudioClip chomp1;  //chomp1 =쩝쩝 먹다
    public AudioClip chomp2;

    public RuntimeAnimatorController chompAnimation; //17장  런타임중 애니메이션 교체
    public RuntimeAnimatorController deathAnimation; //17장 죽엇을 때 애니메이션

    public bool canMove = true; //17장 gameboard 죽었을때 false
    public Vector2 orientation; //

    public float speed = 6.0f;

    public Sprite idleSprite;  //벽에 막혀 멈추고 있을 때

    private bool playedChomp1 = false; //13장 sound

    private AudioSource audio; //13장
                               //오디오 소스는 일시 정지 및 다시 시작, 이동, 롤오프, 피치 설정 및 
    //도플러 효과 지원과 함께 기본적인 위치 오디오     재생을 지원합니다.


    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection;  //다음 방향

   
    
    private Node currentNode,previousNode, targetNode;  //현재, 이전, 타겟 노드

    private Node startingPosition; //16장 리스타트
	// Use this for initialization
	void Start () {
        audio = transform.GetComponent<AudioSource>(); //13장

        Node node = GetNodeAtPosition(transform.localPosition);  //현재 node의 위치를 받아옴

        startingPosition = node; //16장  startingPosition을 넣어줌
        //Debug.Log("Start Node:" + node);
        if (node != null)
        {
            currentNode = node;  //받아온 node를 currentnode에 넣는다.
           // Debug.Log(currentNode);
        }

        direction = Vector2.left;  //처음 시작 왼쪽
        orientation = Vector2.left;  //

        ChangePosition(direction);  //방향을 변환

        if (GameBoard.isPlayerOneUp)
        {
            SetDifficultyForLevel(GameBoard.playerOneLevel);
        }
        else
        {
            SetDifficultyForLevel(GameBoard.playerTwoLevel);
        }
	}
    public void SetDifficultyForLevel(int level) //23
    {
        if((level == 1)) { //24

            speed = 6;

        }//~24
        if(level == 2)
        {
            speed = 7;
        }else if (level == 3)
        {
            speed = 8;
        }
        else if(level == 4)
        {
            speed = 9;
        }
        else if (level == 5)
        {
            speed = 10;
        }
    }
    public void MoveToStartingPosition()  //18
    {
           
        transform.position = startingPosition.transform.position;  //Restart호출 하면 startingPosition을 넣어줌

        transform.GetComponent<SpriteRenderer>().sprite = idleSprite; //다시시작하면 idle

        direction = Vector2.left;
        orientation = Vector2.left;

        UpdateOrientation();
     
    }

    public void Restart()  //16장
    {
        canMove = true;  //canMove를 true   

        currentNode = startingPosition;  
               
        nextDirection = Vector2.left;

       
        transform.GetComponent<Animator>().runtimeAnimatorController = chompAnimation; //17 재시작 chompAnimation으로 
        transform.GetComponent<Animator>().enabled = true;

        ChangePosition(direction);
                
    }

    // Update is called once per frame
    void Update () {
        // Debug.Log("SCORE:" + GameObject.Find("Game").GetComponent<GameBoard>().score);//점수추가
        if (canMove)  //17
        {
            CheckInput();

            Move();

            UpdateOrientation();

            UpdateAnimationState();

            ConsumePellet();// pellet소비
        }
    }

    void PlayChompSound()  //13장 - 번갈아 가면서 실행
    {
        if (playedChomp1)
        {

            //-Play chomp2 , set playerchomp1 to false;
            audio.PlayOneShot(chomp2);
            playedChomp1 = false;
        }
        else
        {
            //-play Chomp1, set player to true
            audio.PlayOneShot(chomp1);
            playedChomp1 = true;
        }
    }

    void CheckInput()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftArrow)){ //왼쪽 방향키
            ChangePosition(Vector2.left);//방향을 -1로줌
        }else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangePosition(Vector2.right);
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow)){
            ChangePosition(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePosition(Vector2.down);
        }
    }

    void ChangePosition (Vector2 d)
    {
        if (d != direction)  //받은 d가 direction방향과 같지 않으면, 처음 0
            nextDirection = d;   //nextDirection에 d를 넣는다.

        if(currentNode != null)  //currentNode가 있으면 
        {
            Node MoveToNode = CanMove(d);  //d 방향과 같은 이웃노드를 받아옴
           // Debug.Log("d:" + d);
         //   Debug.Log("MoveToNode" + MoveToNode);
            if(MoveToNode != null) //MoveToNode가 null이 아니면
            {
                direction = d;  //방향을 넣음
               // Debug.Log("direction:"+ direction);
                targetNode = MoveToNode; //targetNode에 MoveTonode를 넣음
               // Debug.Log("MoveToNode:" + MoveToNode);
                previousNode = currentNode; //currentNode를 previousNode에 넣음
                //Debug.Log("CurrentNode" + currentNode);
                currentNode = null; //currentNode를 null로만듦
            }
        }
    }

    void Move()
    {
       
        if(targetNode !=currentNode &&targetNode != null)//target노드가 현재노드가 아니고, targetnode가 null이 아닐 때
        {

            if(nextDirection == direction * -1) //반대방향 누를 때
            {
              //  Debug.Log("Opposite");
                direction *= -1;

                Node tempNode = targetNode;  

                targetNode = previousNode;  //이전 node를 target을 넣음

                previousNode = tempNode;  //targetNode를 이전 노드에 넣음
            }
            if (OverShotTarget())  //계속 같은 방향(?)
            {
                //Debug.Log("OverShotTarget");
                currentNode = targetNode; //target을 현재 node에 넣고


                transform.localPosition = currentNode.transform.position; //현재 위치를 넣어줌

                GameObject otherPortal = GetPortal(currentNode.transform.position); //portal의 위치를를 otherPortal에 넣음

                if(otherPortal != null) //otherPortal이 null이 아니면
                {
                    Debug.Log("Portal");
                    transform.localPosition = otherPortal.transform.position; //otherportal의 위치로 바꿈

                    currentNode = otherPortal.GetComponent<Node>();//현재 노드에 otherPortal의 node를 넣음
                }

                Node moveToNode = CanMove(nextDirection);  //다음 방향 가능 한 곳을 찾음

                if (moveToNode != null)
                    direction = nextDirection;

                if (moveToNode == null) //다음 방향이 없다면
                    moveToNode = CanMove(direction);  //현재 방향을 moveToNode에 넣음

                if(moveToNode != null)  //MoveToNode가 null이 아니라면 targetNode에 moveToNode를 넣고
                {

                    targetNode = moveToNode;
                    previousNode = currentNode;
                    currentNode = null;
                }

                else
                {
                    direction = Vector2.zero;  //아니면 zero값을 넣어줌
                }
            }
            else
            {
                transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime; //이동
                
            }
        }
        transform.localPosition +=(Vector3) (direction * speed) * Time.deltaTime;
    }

    /*void MoveToNode(Vector2 d)
    {
        Node moveToNode = CanMove(d);
        if(moveToNode != null)
        {
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }*/

    void UpdateOrientation()  //PacMan 회전
    {
        if(direction == Vector2.left)
        {
            orientation = Vector2.left;
            transform.localScale = new Vector3(-1,1,1); //부모와 상대적인 크기, 없으면 자기 자신 크기
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            //z축 주위로 z, x축 주위로 x, y축 주위로 y 각도만큼 회전한(순서대로) Rotation을 반환합니다
        }
        else if(direction == Vector2.right)
        {
            orientation = Vector2.right;
            transform.localScale = new Vector3(1,1,1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2.up)
        {
            orientation = Vector2.up;
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == Vector2.down)
        {
            orientation = Vector2.down;
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
    }

    void UpdateAnimationState()  //Idel상태
    {
       
            if (direction == Vector2.zero)
            {
                GetComponent<Animator>().enabled = false;
                GetComponent<SpriteRenderer>().sprite = idleSprite;

            }
            else
            {
                GetComponent<Animator>().enabled = true;
            }
        
    }

    void ConsumePellet()  //pellet소비
    {
        GameObject o = GetTileAtPosition(transform.position);//현재 위치의 tile을 받아옴


        if(o != null)
        {
            Tile tile = o.GetComponent<Tile>();  //Tile스크립트를 받아오고 

            if (tile != null)
            {
                bool didConsume = false;
                if (GameBoard.isPlayerOneUp) //24 isPlayerOneUp이면 실행 1p
                {
                    if(!tile.didConsumedPlayerOne && (tile.isPellet || tile.isSuperPellet))  //didConsumedPlayerOne이 true 일 때 
                    {
                        didConsume = true;
                        tile.didConsumedPlayerOne = true;
                        GameBoard.playerOneScore += 10;
                        GameMenu.playerOnePelletsConsumed++;
                    }
                }else
                {
                    if (!tile.didConsumedPlayerTwo && (tile.isPellet || tile.isSuperPellet))
                    {
                        didConsume = true;
                        tile.didConsumedPlayerTwo = true;
                        GameBoard.playerTwoScore += 10;
                        GameMenu.playerTwoPelletsConsumed++;
                    }

                }
                if (didConsume)
                {
                    o.GetComponent<SpriteRenderer>().enabled = false;
                    PlayChompSound();

                    if (tile.isSuperPellet)
                    {
                        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

                        foreach(GameObject go in ghosts)
                        {
                            go.GetComponent<Ghost>().StartFrightenedMode();
                        }
                    }
                }
                
                //~24
                /*if (!tile.didConsume && (tile.isPellet || tile.isSuperPellet)){  //didConsume상태이고, isPellet이나 isSuperPellet이면
                    o.GetComponent<SpriteRenderer>().enabled = false;  //false로 바꿈
                    tile.didConsume = true;  //tile.didConsume을 true로 바꿈

                    if (GameMenu.isOnePlayerGame) //19~
                    {
                        GameBoard.playerOneScore += 10; //22
                        GameObject.Find("Game").transform.GetComponent<GameBoard>().playerOnePelletsConsumed++;  //21 1씩증가

                    }
                    else
                    {
                        if (GameBoard.isPlayerOneUp)
                        {
                            GameBoard.playerOneScore += 10;  //22 static
                            GameObject.Find("Game").transform.GetComponent<GameBoard>().playerOnePelletsConsumed++; //21
                        }
                        else
                        {
                           GameBoard.playerTwoScore += 10;
                            GameObject.Find("Game").transform.GetComponent<GameBoard>().playerOnePelletsConsumed++; //21
                        }
                    } //~19
                    
                    
                    PlayChompSound(); //13장

                    if (tile.isSuperPellet)  //14장 --Player가 isSuperPellet을 만나면 호출 
                    {

                        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost"); 

                        foreach (GameObject go in ghosts)
                        {
                            go.GetComponent<Ghost>().StartFrightenedMode(); //blue
                        }
                    }
                  
                }*/
            }
        }
    }

    Node CanMove(Vector2 d)
    {
        Node moveToNode = null;  //MoveToNode를  null로 만듦

        for(int i =0; i< currentNode.neighbors.Length; i++)
        {
            if(currentNode.validDirections[i] == d) //가용방향과 d가 같으면
            {

                moveToNode = currentNode.neighbors[i];//현재 moveToNode에 현재neighbors를 넣음
            }
        }
        return moveToNode;
    }

    GameObject GetTileAtPosition(Vector2 pos) //tile얻는 위치
    {
        int tileX = Mathf.RoundToInt(pos.x);  //근접한 정수로 반올림한 /f/를 반환합니다.
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[tileX, tileY];//tile의 x값, y값을 받아옴

        if (tile != null)
            return tile;

        return null;
    }

    Node GetNodeAtPosition (Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];//Node의 x값 y값
        //pcckman제외 tile을 받아옴
       //Debug.Log("tile:" + tile);
        if(tile != null)
        {
            return tile.GetComponent<Node>(); //그 tile에 node를 받아옴(방향, 이웃노드)
            
        }

        return null;
    }
    
    bool OverShotTarget() //같은 방향
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf> nodeToTarget;  //0 or 1의 값 반환
    }

    float LengthFromNode(Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude; //길이를 제곱한 값을 반환
    }
   

    GameObject GetPortal (Vector2 pos) //다른포탈로 이동
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];
        //GetComponent : type에 관한 모든 정보 반환

        //PackMan을 빼고 다른값 넣어줌
        if (tile != null) //tile이 null이아니면 
        {
            if (tile.GetComponent<Tile>() != null)  //tile(다른 방향에서 나오는)이면
            {
                if (tile.GetComponent<Tile>().isPortal)//isPortal이 true이면 
                {
                    GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                    return otherPortal;  //이동한 포탈 리턴
                }
            }
            
        }
        return null;
    }   
}
