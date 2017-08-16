using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    public float moveSpeed = 5.9f; //몬스터 이동속도 3.9
    public float normalMoveSpeed = 5.9f;//15장
    public float frightenedModeMoveSpeed = 2.9f;  //14장
    public float consumedMovedSpeed = 15f;//15장 먹히 눈알 스피드

    public bool canMove = true; //gameboard 17장, pacMan이 죽었을 때 false로 바꿈

    public int pinkyReleaseTimer = 5;  
    public int inkyReleaseTimer = 14;
    public int clydeReleaseTimer = 21;
    public float ghostReleaseTimer = 0;  

    public int frightenModeDuration = 10; //14장 지속시간
    public int startBlinkingAt = 7;  //

    public bool isInGhostHouse = false;//

    public Node homeNode;
    public Node ghostHouse; //15장, 먹힌 고스트 집

    public Node startingPosition; //시작위치

    public int scatterModeTimer1 = 7;  //scatter모드
    public int chaseModeTimer1 = 20;    //chase모드
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;

    public Sprite eyesUp; //15장  Sprite
    public Sprite eyesDown;
    public Sprite eyesLeft;
    public Sprite eyesRight;

    public RuntimeAnimatorController ghostUp;  //AnimatorController의 런타임 표현. 
    //런타임 중에 Animator의 컨트롤러를 변경하는 데 사용할 수 있습니다.
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController ghostRight;
    public RuntimeAnimatorController ghostWhite;  //14장 scare모드
    public RuntimeAnimatorController ghostBlue;

         
    private int modeChangeIteration = 1;
    private float modeChangeTimer = 0;

    private float frightenedModeTimer = 0; //14장
    private float blinkTimer = 0;

    private bool frightenedModeIsWhite = false; //14장
   

    private float previousMoveSpeed;  //14장

    private AudioSource backgroundAudio;// 15장 

    public enum Mode   //상태
    {
        Chase,
        Scatter,
        Frightened,
        Consumed //15장 소비된 상태
    }

    public Mode currentMode = Mode.Scatter; //처음 scatter;
    Mode previousMode; //이전상태

    public enum GhostType //
    {
        Red,
        Pink,
        Blue,
        Orange
    }

    public GhostType ghostType = GhostType.Red;//처음 red

    private GameObject pacMan;

    private Node currentNode, targetNode, previousNode;  //현 노드, 다음노드, 이전노드 저장
    private Vector2 direction, nextDirecton; //현 방향, 다음방향





	// Use this for initialization
	void Start () {

        

        if (GameBoard.isPlayerOneUp)
        {
            SetDifficultyForLevel(GameBoard.playerOneLevel);
        }
        else
        {
            SetDifficultyForLevel(GameBoard.playerTwoLevel);
        }
        backgroundAudio = GameObject.Find("Game").transform.GetComponent<AudioSource>();//15장 오디오 소스를 찾음

        pacMan = GameObject.FindGameObjectWithTag("PacMan");  //PacMan을 찾는다.
    
        Node node = GetNodeAtPosition(transform.localPosition);//현재 node를 node에 넣음
      
        if(node != null)
        {
            currentNode = node; //node가 null이아니면 currentNode에 받아온 node를 넣음
            //Debug.Log("Current1");
        }

        if (isInGhostHouse)//ghosthouse가 true이면 (밖에서 체크) 
        {
            direction = Vector2.up;  //방향 up
            targetNode = currentNode.neighbors[0];
        }else
        {
            direction = Vector2.left;  //red 고스트
            targetNode = ChooseNextNode();
        }
       
        previousNode = currentNode;     //현재 노드를 이전 노드에 줌
                                        // Debug.Log("currentNode: " + currentNode);

        UpdateAnimatorController();
        
        
    }

    public void SetDifficultyForLevel(int level) //23
    {
        if(level ==1) //24
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 5;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 20;

            frightenModeDuration = 10;
            startBlinkingAt = 7;

            pinkyReleaseTimer = 5;
            inkyReleaseTimer = 14;
            clydeReleaseTimer = 21;

            moveSpeed = 5.9f;
            normalMoveSpeed = 5.9f;
            frightenedModeMoveSpeed = 2.9f;
            consumedMovedSpeed = 15f;
        }
        else if (level == 2)
        {
            scatterModeTimer1 =7;
            scatterModeTimer2 =7;
            scatterModeTimer3 =5;
            scatterModeTimer4 =1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1033;

            frightenModeDuration =9;
            startBlinkingAt = 6;

            pinkyReleaseTimer = 4;
            inkyReleaseTimer = 12;
            clydeReleaseTimer = 18;

            moveSpeed = 6.9f;
            normalMoveSpeed = 6.9f;
            frightenedModeMoveSpeed = 3.9f;
            consumedMovedSpeed = 18;

        }
        else if (level == 3)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1033;

            frightenModeDuration = 8;
            startBlinkingAt = 5;

            pinkyReleaseTimer = 3;
            inkyReleaseTimer = 10;
            clydeReleaseTimer = 15;

            moveSpeed = 7.9f;
            normalMoveSpeed = 7.9f;
            frightenedModeMoveSpeed = 4.9f;
            consumedMovedSpeed = 20;
        }
        else if (level == 4)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1033;

            frightenModeDuration = 7;
            startBlinkingAt = 5;

            pinkyReleaseTimer = 2;
            inkyReleaseTimer = 8;
            clydeReleaseTimer = 13;

            moveSpeed = 7.9f;
            normalMoveSpeed = 7.9f;
            frightenedModeMoveSpeed = 5.9f;
            consumedMovedSpeed = 22;
        }
        else if (level == 5)
        {
            scatterModeTimer1 = 5;
            scatterModeTimer2 = 5;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1037;

            frightenModeDuration = 6;
            startBlinkingAt = 3;

            pinkyReleaseTimer = 2;
            inkyReleaseTimer = 6;
            clydeReleaseTimer = 10;

            moveSpeed = 9.9f;
            normalMoveSpeed = 9.9f;
            frightenedModeMoveSpeed = 6.9f;
            consumedMovedSpeed = 24;
        }
    }
    public void MoveToStartingPosition()  //18
    {

        if (transform.name != "Ghost_Blinky")
            isInGhostHouse = true;

        transform.position = startingPosition.transform.position;  //스타팅 포지션

        if (isInGhostHouse)
        {
            direction = Vector2.up;

        }else
        {
            direction = Vector2.left;
        }
        UpdateAnimatorController();
    }

    public void Restart()  //16장
    {
        canMove = true; // 17장 다시 움직이게

      
        currentMode = Mode.Scatter; //17 현재 모드 Scatter

        moveSpeed = normalMoveSpeed; //17

        previousMoveSpeed = 0;  //17
        

       

        ghostReleaseTimer = 0;//초기화
        modeChangeTimer = 0;
        modeChangeIteration = 1;

      

        currentNode = startingPosition;

        if (isInGhostHouse)
        {

            direction = Vector2.up;
            targetNode = currentNode.neighbors[0];

        }else
        {
            direction = Vector2.left;
            targetNode = ChooseNextNode();

        }

        previousNode = currentNode;
      
    }

  
    // Update is called once per frame
    void Update()
    {
        if (canMove)  //17장
        {
            ModeUpdate();

            Move();

            ReleaseGhosts();

            CheckCollision(); //15장 충돌 체크

            CheckisInChostHouse();//15장 고스트가 집에 있는지

        }
    }

    void CheckisInChostHouse() //15장
    {
        if (currentMode == Mode.Consumed)  //소비된 상태
        {
            GameObject tile = GetTileAtPosition(transform.position);  //targetTile을 찾음 tile형태
            if (tile != null) {
                if (tile.transform.GetComponent<Tile>() != null) //tile이 null이아니고
                {
                    if (tile.transform.GetComponent<Tile>().isGhostHouse)//tile이 isGhostHouse으로 돌아오면
                    {
                        moveSpeed = normalMoveSpeed; // 다시 원래 속도를 줌

                        Node node = GetNodeAtPosition(transform.position);  //현재 node를 넣음

                        if(node != null)
                        {
                            currentNode = node;

                            direction = Vector2.up; //방향을 위로줌
                            targetNode = currentNode.neighbors[0];  //targetNode가 위에 노드

                            previousNode = currentNode; //현재 노드를 이전노드
                               
                            currentMode = Mode.Chase;       //Mode를 Chase로

                            UpdateAnimatorController();
                        }
                    }
                }
            }

        }
    }

    void CheckCollision() //15장 충돌
    {
        Rect ghostRect = new Rect(transform.position, transform.GetComponent<SpriteRenderer>().sprite.bounds.size /4); //x,y 사각형
        Rect pacManRect = new Rect(pacMan.transform.position, pacMan.transform.GetComponent<SpriteRenderer>().sprite.bounds.size /4);

        if (ghostRect.Overlaps(pacManRect))  //다른 Rect가 이것을 오버랩하면 true를 반환하고
        {
            if(currentMode == Mode.Frightened)  //Firghtened상태면 Consumed()호출
            {
                Consumed();
            }
            else
            {
                if (currentMode != Mode.Consumed)  //  //17장 소비된 상태가 아니면
                {

                    //-Pac-Man Shoulde die  //16장
                    GameObject.Find("Game").transform.GetComponent<GameBoard>().StartDeath();  //충돌하면 startDeathAnimation
                }
            }
           
           

        }
    }
    void Consumed()  //15장 
    {
        if (GameMenu.isOnePlayerGame) //19~
        {
            GameBoard.playerOneScore += 200;

        }
        else
        {
            if (GameBoard.isPlayerOneUp)
            {
                GameBoard.playerTwoScore += 200;
            }
            else
            {
                GameBoard.playerTwoScore += 200;
            }
        }//~19
        currentMode = Mode.Consumed;  
        previousMoveSpeed = moveSpeed;  //현재 스피드를 이전스피드로
        moveSpeed = consumedMovedSpeed;    //moveSpeed를 consumedMovedSpeed로 
        UpdateAnimatorController();

        GameObject.Find("Game").transform.GetComponent<GameBoard>().StartConsumed(this.GetComponent<Ghost>());  
    }

    void UpdateAnimatorController()//12장 start, move  //모습 변하게
    {
        if (currentMode != Mode.Frightened && currentMode != Mode.Consumed) //15장 consumed추가
        {
            if (direction == Vector2.up)  //up이면 ghostUp애니메이션 실행
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostUp;

            }
            else if (direction == Vector2.down)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostDown;
            }
            else if (direction == Vector2.left)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
            }
            else if (direction == Vector2.right)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
            }
            else
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
            }
        }
        else if (currentMode == Mode.Frightened) //14장 frigten ,15장
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostBlue;
        }
        else if (currentMode == Mode.Consumed)  //기존 없애고 눈알만
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = null;
    
            if (direction == Vector2.up)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesUp;
            }else if (direction == Vector2.down)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesDown;
            }
            else if (direction == Vector2.right)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesRight;
            }
            else if (direction == Vector2.left)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesLeft;
            }
        }
    }
    void Move()
    {
        
        if (targetNode != currentNode && targetNode != null && !isInGhostHouse) //targetnode가 현재 노드가 아니고, null이 아니면
           
        {
            //Debug.Log("OverShot" + OverShotTarget());
            //Debug.Log("OverShotTarget2");
            if (OverShotTarget())  //player가 위나아래 앞
            {
                //Debug.Log("OverShot"+OverShotTarget());
                currentNode = targetNode;  //targetnode에 현재노드를 넣는다.

                transform.localPosition = currentNode.transform.position; //위치를 넣는다.
                

                GameObject otherPortal = GetPortal(currentNode.transform.position);

                if (otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                  

                    currentNode = otherPortal.GetComponent<Node>();

                }
                targetNode = ChooseNextNode();

                previousNode = currentNode;

                currentNode = null;

                UpdateAnimatorController();
            }
            else
            {
                //  Debug.Log(direction);
                transform.localPosition += (Vector3)direction * moveSpeed * Time.deltaTime;
            }
        }
    }
    
    void ModeUpdate()
    {
        if(currentMode != Mode.Frightened) //frightened모드가 아니면
        {
            modeChangeTimer += Time.deltaTime;

            if(modeChangeIteration == 1)  //modeChangeIteration가 1이면
            {
                if(currentMode == Mode.Scatter && modeChangeTimer> scatterModeTimer1) // 현재 모드가 scatter이고, modeChaneTimer가 지나면
                {
                    ChangeMode(Mode.Chase);  //chase 모드로 바꿈
                    modeChangeTimer = 0;    //modeChangeTimer을 0으로 바꿈
                }

                if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)  //chase모드고 
                {

                    modeChangeIteration = 2;    //modeChangeIteration을 2로바꾸고
                    ChangeMode(Mode.Scatter); //Scatter모드로
                    modeChangeTimer = 0;
                }
            }else if(modeChangeIteration == 2)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if (currentMode ==Mode.Chase && modeChangeTimer> chaseModeTimer2)
                {
                    modeChangeIteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }

            }
            else if(modeChangeIteration == 3)
            {
              if(currentMode ==Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
              if(currentMode ==Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {
                    modeChangeIteration = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }

            }else if(modeChangeIteration ==4)
            {
                if(currentMode ==Mode.Scatter && modeChangeTimer > scatterModeTimer4) {

                    ChangeMode(Mode.Chase); //chase모드 계속
                    modeChangeTimer = 0;
                }
            }
        }else if(currentMode == Mode.Frightened)  //14장
        {
            frightenedModeTimer += Time.deltaTime;

            if(frightenedModeTimer >=frightenModeDuration) //10초 지나면
            {
                backgroundAudio.clip = GameObject.Find("Game").transform.GetComponent<GameBoard>().backgroundAudioNormal;// 15장 일반 상태의 배경음으로 되돌림
                backgroundAudio.Play();//Play
                
                frightenedModeTimer = 0;
               // Debug.Log("previousMode:"+previousMode);  //changemode를 호출
                ChangeMode(previousMode);  //이전 모드로  frightened 해제
                
            }

            if(frightenedModeTimer >= startBlinkingAt)  //7초부터 반짝반짝
            {
                blinkTimer += Time.deltaTime;

                if(blinkTimer >= 0.1f)  //반짝반짝
                {
                    blinkTimer = 0f;

                    if (frightenedModeIsWhite)
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostBlue;
                        frightenedModeIsWhite = false;
                    }  else
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostWhite;
                        frightenedModeIsWhite = true;
                    }
                }
            }

                          }
    }
    void ChangeMode (Mode m) //모드 변환 ,frighten이면 10초가 지나야만 호출 가능
    {


        if (currentMode == Mode.Frightened) //14장 현재모드가 frigtened이면 
        {

            moveSpeed = previousMoveSpeed;  //이전 스피드를 넣는다

        }
        if(m == Mode.Frightened)  //받은 모드가  Mode.Frightened이면 
        {
        
            previousMoveSpeed = moveSpeed;  // 현재 speed를 이전 speed로 넣음
            moveSpeed = frightenedModeMoveSpeed;  //frightenedModeMoveSpeed 는 2.9를 movespeed에 넣음
        }
        if(currentMode != m) {  //두번 SuperPellet먹으면 error나서 만들어줌, 두 번 같은 상태일 때 변하지 않게
            //15장
            previousMode = currentMode;  // 이전 모드에 현재 모드를 넣음
                                         //previousMode는 scatter이 됨
            currentMode = m;  //받은 m을 현재 모드에 넣음
        }

      
         
        UpdateAnimatorController(); //14장
    }

    public void StartFrightenedMode()  //14장  -PacMan에서 호출
    {
        if (currentMode != Mode.Consumed)
        {
            frightenedModeTimer = 0;// 다시먹을 때 초기화 //15장
            backgroundAudio.clip = GameObject.Find("Game").transform.GetComponent<GameBoard>().backgroundAudioFrightened; //15장 먹을 때 들리는 소리
            backgroundAudio.Play();
            ChangeMode(Mode.Frightened);  //Firghtened 모드
        }

    }

    Vector2 GetRedGhostTargetTile()//레드 일 때
    {
        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
       
        return targetTile;
    }

    Vector2 GetPinkGhostTargetTile()//핑크 일 때 
    {
        //-Four tiles ahead Pac-Man
        //-Taking account Position and Orientation
        // - 4 개의 타일을 앞두고 Pac-Man
        // - 계정 포지션 및 오리엔테이션

        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTile = new Vector2(pacManPositionX, pacManPositionY);
        Vector2 targetTile = pacManTile + (4 * pacManOrientation);  //같이 안따라다니게???
       // Debug.Log("PinkTargetTile:" + targetTile);
        return targetTile;
    }
    Vector2 GetBlueGhostTargetTile()
    {
        //-Select the position two tiles in front of Pac-Man
        //-Draw Vector from blinky to that position
        //-Double the length of the vector
        // - Pac-Man 앞의 위치 타일 두 개 선택
        // - blinky에서 그 위치로 Vector를 그립니다.
        // - 벡터의 길이를 두배로한다.

        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTiler = new Vector2(pacManPositionX, pacManPositionY);

        Vector2 targetTile = pacManTiler + (2 * pacManOrientation);

        //-Temporary Blink Position
        Vector2 tempBlinkyPosition = GameObject.Find("Ghost_Blinky").transform.localPosition;

        int blinkyPositionX = Mathf.RoundToInt(tempBlinkyPosition.x);
        int blinkyPositionY = Mathf.RoundToInt(tempBlinkyPosition.y);

        tempBlinkyPosition = new Vector2(blinkyPositionX, blinkyPositionY);

        float distance = GetDistance(tempBlinkyPosition, targetTile);   //거리 2배
        distance *= 2;

        targetTile = new Vector2(tempBlinkyPosition.x + distance, tempBlinkyPosition.y + distance);
       // Debug.Log("BlueTargetTile:"+targetTile);
        return targetTile;
    }

    Vector2 GetOrangeGhostTargetTile()
    {
        //-Calculate the distance from Pac-Man
        //-If the distance is greater than eight tiles. targeting is the same as Blinky
        //-If the distance is less than eight tiles, then target is his home node, so same as scatter mode;
        // - Pac-Man으로부터의 거리 계산
        // - 거리가 여덟 개의 타일보다 큰 경우. 타겟팅은 Blinky와 동일합니다.
        // - 거리가 8 개 타일보다 작 으면 대상은 홈 노드이므로 스 캐터 모드와 동일합니다.


        Vector2 pacManPosition = pacMan.transform.localPosition;

        float distance = GetDistance(transform.localPosition, pacManPosition);
        Vector2 targetTile = Vector2.zero;

        if (distance > 8)
        {
            targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
        }else if (distance < 8)
        {
            targetTile = homeNode.transform.position;
        }
        return targetTile;
    }


    Vector2 GetTargetTile()//
    {
        Vector2 targetTile = Vector2.zero;

        if (ghostType == GhostType.Red)  //레드일때
        {

            targetTile = GetRedGhostTargetTile();

        } else if (ghostType == GhostType.Pink)
        {
            targetTile = GetPinkGhostTargetTile();
        }
        else if(ghostType ==GhostType.Blue)
        {
            targetTile = GetBlueGhostTargetTile();
        }
        else if (ghostType == GhostType.Orange)
        {
            targetTile = GetOrangeGhostTargetTile();
        }
        return targetTile;
    }

    Vector2 GetRandomTile() { // 15장 frightened일 때
        int x = Random.Range(0, 28);  //width
        int y = Random.Range(0, 36); //height

        return new Vector2(x, y);
    }

    void ReleasePinkGhost()
    {
        if(ghostType == GhostType.Pink && isInGhostHouse)   //핑크고스트, true일 때 
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseBlueGhost()
    {
        if (ghostType == GhostType.Blue && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }
    void ReleaseOrangeGhost()
    {
        if (ghostType == GhostType.Orange && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseGhosts()
    {
        ghostReleaseTimer += Time.deltaTime;
        if (ghostReleaseTimer > pinkyReleaseTimer) 
            ReleasePinkGhost();

        if (ghostReleaseTimer > inkyReleaseTimer) 
            ReleaseBlueGhost();

        if (ghostReleaseTimer > clydeReleaseTimer)
            ReleaseOrangeGhost();

    }
    Node ChooseNextNode()
    {
        Vector2 targetTile = Vector2.zero;  //처음을 zero로 줌

        if(currentMode == Mode.Chase)//
        {
            targetTile = GetTargetTile();
        }else if (currentMode == Mode.Scatter)
        {
            targetTile = homeNode.transform.position;
        }else if (currentMode == Mode.Frightened) //15장 Mode가 Frigthenen이면 Random Tile로 targetTile을 함
        {
            targetTile = GetRandomTile();
        }else if ( currentMode == Mode.Consumed)
        {
            targetTile = ghostHouse.transform.position;  //targetTile을 gostHouse로 함
        }

        Node moveToNode = null; 

        Node[] foundNodes = new Node[4];  //최댓값 4
        Vector2[] foundNodesDirection = new Vector2[4];

        int nodeCounter = 0;

        for(int i = 0; i< currentNode.neighbors.Length; i++)
        {
            if(currentNode.validDirections[i] != direction * -1) {  //반대방향이 아닐 때 

                if(currentMode != Mode.Consumed)  //소비된 상태가 아닐 때
                {
                    GameObject tile = GetTileAtPosition(currentNode.transform.position); //15장

                    if(tile.transform.GetComponent<Tile>().isGhostHouseEntrance == true)
                    {//isGhostHouseEntrance인 tile이면 

                        //-Found a ghost house, don't want to allow movemnet
                        if (currentNode.validDirections[i] != Vector2.down){//vector2가 down을 실행x 
                            //고스트 상태가 아니면 들어오지 못한다.

                            foundNodes[nodeCounter] = currentNode.neighbors[i];  //현재 이웃 노드를 넣음
                            foundNodesDirection[nodeCounter] = currentNode.validDirections[i]; //현재 가용방향을 넣음
                            nodeCounter++;
                        }
                    }
                    else
                    { //isGhostHouseEntrance가 false (원래 상태)
                        foundNodes[nodeCounter] = currentNode.neighbors[i];  //현재 이웃 노드를 넣음
                        foundNodesDirection[nodeCounter] = currentNode.validDirections[i]; //현재 가용방향을 넣음
                        nodeCounter++;
                    }
                }else{            //소비된 상태 일 때        
                        foundNodes[nodeCounter] = currentNode.neighbors[i];  //현재 이웃 노드를 넣음
                        foundNodesDirection[nodeCounter] = currentNode.validDirections[i]; //현재 가용방향을 넣음
                        nodeCounter++;                    
                }                                                        
            }
        }
        if(foundNodes.Length == 1)  //길이가 1이면 
        {
            moveToNode = foundNodes[0];  //가던 방향
            direction = foundNodesDirection[0];
             
        }
        if(foundNodes.Length > 1)  //길이가 1이상이면
       
        {
            float leastDistance = 100000f;

            for(int i =0; i< foundNodes.Length; i++)
            {
                if(foundNodesDirection[i] != Vector2.zero)  //방향이 0이아니면
                {
                    float distance = GetDistance(foundNodes[i].transform.position, targetTile);//target과 현재 노드의 거리를 반환
                    //거리만 계산해서 넣어줌, targetTile이 음수여도 상관 x
                    if(distance < leastDistance)
                    {
                        leastDistance = distance; //거리를 넣어주고
                        moveToNode = foundNodes[i]; //찾을 노드
                        direction = foundNodesDirection[i];

                       // Debug.Log("MoveToNode: "+moveToNode);
                    }
                }
            }
        }
        return moveToNode;
    }

    Node GetNodeAtPosition (Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y]; //x,y값으로 tile을 찾음

        //Debug.Log("Tile:" + tile +", FOR GHOST :" +ghostType);
        if(tile != null)
        {
            if (tile.GetComponent<Node>() != null)
            {
                return tile.GetComponent<Node>(); //tile이 null이 아니면 그 node를 반환함
            }
        }
        return null;
    }

    GameObject GetTileAtPosition(Vector2 pos)  //15장  x,y값 반환
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameObject.Find("Game").transform.GetComponent<GameBoard>().board[tileX, tileY];

        if (tile != null)
            return tile;

        return null;
    }

    GameObject GetPortal (Vector2 pos) //포탈
    {

        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if(tile != null)
        {
            if (tile.GetComponent<Tile>().isPortal)//isPortal이면
            {

                GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                return otherPortal; //otherPortal 반환
            }
        }
        return null;
    }
 

    bool OverShotTarget()
    {

        //Debug.Log("OverShotTarget1");
        float nodeToTarget = LengthFromNode(targetNode.transform.position);  //반대방향
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float LengthFromNode (Vector2 targetPosition)
    {
       // Debug.Log("TargetPosition :" + targetPosition);
       // Debug.Log("PreviousNude.transform.position:" + previousNode.transform.position);
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        
        return vec.sqrMagnitude;
    }


    float GetDistance(Vector2 posA, Vector2 posB) //포인트 사이 거리
    {

        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        float distance = Mathf.Sqrt(dx * dx + dy * dy);//제곱근 반환

        return distance;
    }

}
