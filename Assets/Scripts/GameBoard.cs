using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  //18장
using UnityEngine.SceneManagement; //20
 
public class GameBoard : MonoBehaviour
{
    //게임기판
    private static int boardWidth = 28; //가로 pellet갯수
    private static int boardHeight = 36; //세로 갯수

    private bool didStartDeateh = false; //16장
    private bool didStartConsumed = false; //19장  //고스트 consumed상태

    public static int playerOneLevel = 1; //21    
    public static int playerTwoLevel = 1; //21



    public int totalPellets = 0;
    public int score = 0;
    public static int playerOneScore = 0; //19   
    public static int playerTwoScore = 0; //19
   

    public static bool isPlayerOneUp = true; //19
    public bool shouldBlink = false; //22  처음 false

    public float blinkIntervalTime = 0.1f; //22 0.1초마다 깜빡
    private float blinkIntervalTimer = 0; //22

    public int pacManLives = 3; //16장 생명력

    public AudioClip backgroundAudioNormal; //15장 오디오
    public AudioClip backgroundAudioFrightened; //15장
    public AudioClip backgroundAudioPacManDeath; // 16장
    public AudioClip consumedGhostAudioClip; //19

    public Sprite mazeBlue;//22 
    public Sprite mazeWhite; //22


    public Text playerText; //18장 화면에 출력
    public Text readyText; // 18장 화면에 출력

    public Text highScoreText;//19
    public Text playerOneUp;
    public Text playerTwoUp;
    public Text PlayerOneScoreText;
    public Text PlayerTwoScoreText;
    public Image playerLives2;
    public Image playerLives3;

    public Text consumedGhostScoreText; //19

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];  //가로 28, 세로 36

    private bool didIncrementLevel = false; //23 레벨증가
           
   
    // Use this for initialization
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
        //모든 활성화한 로드된 type 타입의 오브젝트 리스트를 반환합니다.
        foreach (GameObject o in objects)
        {
            Vector2 pos = o.transform.position;

            if (o.name != "PacMan" && o.name != "Nodes" && o.name != "NonNodes" && o.name != "Maze" && o.name != "Pellets"
                && o.tag != "Ghost" && o.tag != "ghostHome" && o.name != "Canvas" && o.tag != "UIElements") 
            {//

                if (o.GetComponent<Tile>() != null)
                {
                    if (o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
                    {
                        totalPellets++;  //totalpellet수를 찾음
                    }
                }
                board[(int)pos.x, (int)pos.y] = o;  //팩맨을 제외하고 o를 넣어줌
                //Debug.Log(board[(int)pos.x, (int)pos.y]);
            }
            else
            {
                //  Debug.Log("Found PackMan at:" + pos);//팩맨을 찾음
            }
        }

        if (isPlayerOneUp) //23 게임 시작시 플레이
        {
            if (playerOneLevel == 1)
            {
                GetComponent<AudioSource>().Play();
            }
            else
            {
                if(playerTwoLevel == 1)
                {
                    GetComponent<AudioSource>().Play();
                }
            }

        }
        StartGame();  //18
    }


    void Update()  //19~
    {
        UpdateUI();

        CheckpelletsConsumed(); //21 소비된거 찾아줌

        CheckShouldBlink();//22 깜빡깜빡
    }

    void UpdateUI()
    {
        PlayerOneScoreText.text = playerOneScore.ToString();
        PlayerTwoScoreText.text = playerTwoScore.ToString();

        if (isPlayerOneUp)
        {
            if (GameMenu.livesPlayerOne == 3)
            {
                playerLives3.enabled = true;
                playerLives2.enabled = true;
            }
            else if (GameMenu.livesPlayerOne == 2)
            {

                playerLives3.enabled = false;
                playerLives2.enabled = true;

            }
            else if (GameMenu.livesPlayerOne == 1)
            {
                playerLives3.enabled = false;
                playerLives2.enabled = false;
            }
            //~19
        } else
        {
            if (GameMenu.livesPlayerTwo == 3)
            {
                playerLives3.enabled = true;
                playerLives2.enabled = true;
            }
            else if (GameMenu.livesPlayerTwo == 2)
            {

                playerLives3.enabled = false;
                playerLives2.enabled = true;

            }
            else if (GameMenu.livesPlayerTwo == 1)
            {
                playerLives3.enabled = false;
                playerLives2.enabled = false;
            }

        }

       
    } //~19

    void CheckpelletsConsumed() //21
    {
        if (isPlayerOneUp)
        {
            //-Player one is playing
            if(totalPellets == GameMenu.playerOnePelletsConsumed)  //토탈이랑 같으면 win
            {
                PlayerWin(1);
            }
        }
        else
        {
            //-Player two is playing
            if(totalPellets == GameMenu.playerTwoPelletsConsumed)
            {
                PlayerWin(2);
            }
        }
    }

    void PlayerWin (int playerNum) //21
    {
        if(playerNum == 1)
        {
            if (!didIncrementLevel) //23
            {
                didIncrementLevel = true;
                playerOneLevel++; //레벨올려줌
                StartCoroutine(ProcessWin(2));
            }
           
        }
        else{
            if (!didIncrementLevel) //23
            {
                didIncrementLevel = true;
                playerTwoLevel++; //레벨올려줌
                StartCoroutine(ProcessWin(2));
            }

        }
       


    }
    IEnumerator ProcessWin(float delay) {   //21 이김

        GameObject pacMan = GameObject.Find("PacMan");  
        pacMan.transform.GetComponent<PacMan>().canMove = false;
        pacMan.transform.GetComponent<Animator>().enabled = false;

        transform.GetComponent<AudioSource>().Stop();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = false;
            ghost.transform.GetComponent<Animator>().enabled = false;
        }

        yield return new WaitForSeconds(delay);

        StartCoroutine(BlinkBoard(2f));
        
    }

    IEnumerator BlinkBoard(float delay)  //21 이김
    {
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }
        //-Blink Board
        shouldBlink = true; //22 shouldBlink 를 true로 줌

        yield return new WaitForSeconds(delay);

        //-Restart the game at the next level

        shouldBlink = false; //2초 후 false
        StartNextLevel();
    }

    private void StartNextLevel()  //22
    {
        
        StopAllCoroutines(); //24  코루틴을 다 멈춤
        if (isPlayerOneUp)
        {
            ResetPelletsForPlayer(1);  //리셋
            GameMenu.playerOnePelletsConsumed = 0;  //소비된 걸 0
        }
        else
        {
            ResetPelletsForPlayer(2);
            GameMenu.playerTwoPelletsConsumed = 0;
        }

        GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = mazeBlue;//mazeBlue로

        didIncrementLevel = false; //초기상태

        StartCoroutine(ProcessStartNextLevel(1));  //다음 레벨
    }
    IEnumerator ProcessStartNextLevel(float delay)  //24
    {
        playerText.transform.GetComponent<Text>().enabled = true;  //text true
        readyText.transform.GetComponent<Text>().enabled = true;

        if (isPlayerOneUp)
            StartCoroutine(StartBlinking(playerOneUp));
        else
            StartCoroutine(StartBlinking(playerTwoUp));

        RedrawBoard();  //다시 그림

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestartShowObjects(1));  //다음 레벨 갈 때
    }
    private void CheckShouldBlink()  //22
    {
        if (shouldBlink) //shouldBlink가 true이면
        {
            if(blinkIntervalTimer< blinkIntervalTime) //0.1초보다 크면
            {
                blinkIntervalTimer += Time.deltaTime;
            }else
            {
                blinkIntervalTimer = 0;

                if(GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite == mazeBlue)
                {
                    GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = mazeWhite;
                    
                }
                else
                {
                    GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = mazeBlue;
                    
                }
            }


        }
    } 

    public void StartGame()  //18 처음 시작
    {
        if (GameMenu.isOnePlayerGame) {  // 19

            playerTwoUp.GetComponent<Text>().enabled = false;
            PlayerTwoScoreText.GetComponent<Text>().enabled = false;
        }
        else
        {

            playerTwoUp.GetComponent<Text>().enabled = false; //er
            PlayerTwoScoreText.GetComponent<Text>().enabled = true;
        }
        if (isPlayerOneUp) //19
        {

            StartCoroutine(StartBlinking(playerOneUp));
        }
        else  //19
        {
            StartCoroutine(StartBlinking(playerTwoUp));
        }
        //- Hide All Ghosts 
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;  //처음 false
            ghost.transform.GetComponent<Ghost>().canMove = false;
        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
        pacMan.transform.GetComponent<PacMan>().canMove = false;

        StartCoroutine(ShowObjectsAfter(2.25f));  //2.25초 후 실행
    }

    public void StartConsumed (Ghost consumedGhost)  //19 고스트에서 consume를 호출
    {
        if (!didStartConsumed)  //didStartConsumed가 false이면 처음 false로 줌
        {

            didStartConsumed = true;

            //-Pause all the ghosts
            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }

            //-Pause Pac-Man
            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<PacMan>().canMove = false;  //일시정지

            //-Hide Pac-Man
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false; //모습 감춤

            //-Hide the consumed ghost
            consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = false;//받아온 고스트 renderer없앰

            //-Stop background Musinc
            transform.GetComponent<AudioSource>().Stop();  //음악 멈춤

            Vector2 pos = consumedGhost.transform.position;//소비된 고스트 위치 잡아옴
            //Debug.Log("World Pos :" + pos);
            Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);  //​​월드 좌표에서 뷰포트 좌표로 변환합니다.
            //Debug.Log("viewPort Pos :" + viewPortPoint);
           // consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = pos;
           // consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = pos;
            consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
            consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

            consumedGhostScoreText.GetComponent<Text>().enabled = true;

            // -Play the consumed sound
            transform.GetComponent<AudioSource>().PlayOneShot(consumedGhostAudioClip);

            //-Wait for the audio clip to finish
            StartCoroutine(ProcessConsumeAfter(0.75f, consumedGhost)); 
        }
    }

    IEnumerator StartBlinking(Text blinkText)  //19
    {
        
            yield return new WaitForSeconds(0.25f); //0.25초마다 1up깜빡이게

            blinkText.GetComponent<Text>().enabled = !blinkText.GetComponent<Text>().enabled;  //반대값을 넣어줌
        
             StartCoroutine(StartBlinking(blinkText));
    }
    IEnumerator ProcessConsumeAfter ( float delay, Ghost consumedGhost) //19 받아온 고스트
    {
        yield return new WaitForSeconds(delay);  //0.75초 멈추고 실행

        //-Hide the score
        consumedGhostScoreText.GetComponent<Text>().enabled = false; //200사라짐

        //-Show Pac - Man
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

        //-Show Consumed Ghost
        consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = true; //renederer 보이게

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;
        }

        //-Resume Pac-Man
        pacMan.transform.GetComponent<PacMan>().canMove = true;

        //-Start Background Music
        transform.GetComponent<AudioSource>().Play();

        didStartConsumed = false;


    }

    IEnumerator ShowObjectsAfter (float delay) //18
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;      //ghost찾음   
        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

        playerText.transform.GetComponent<Text>().enabled = false;  //payerText 지움

        StartCoroutine(StartGameAfter(2));         //2초 후 실행
    }
    IEnumerator StartGameAfter (float delay) //18
    {
        yield return new WaitForSeconds(delay);
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;  
        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().canMove= true;

        readyText.transform.GetComponent<Text>().enabled = false;

        transform.GetComponent<AudioSource>().clip = backgroundAudioNormal;
        transform.GetComponent<AudioSource>().Play();
    }

    public void StartDeath() //17장 , Ghost : CollisionCheck
    {
        if (!didStartDeateh)
        {

            StopAllCoroutines(); //19~  현재 behaviour상에서 동작하는 모든 coroutine의 동작을 멈춥니다

            if (GameMenu.isOnePlayerGame)
            {
                playerOneUp.GetComponent<Text>().enabled = true;

            }
            else
            {
                playerOneUp.GetComponent<Text>().enabled = true;
                playerTwoUp.GetComponent<Text>().enabled = true;//~19
            }
            didStartDeateh = true;

            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost"); //ghost 태그를 찾음

            foreach (GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;  //canMove를 멈춘다.
            }

            GameObject pacMan = GameObject.Find("PacMan"); //PackMan 겜오브젝트와 
            pacMan.transform.GetComponent<PacMan>().canMove = false; //PacMan의 canMove도 false

            pacMan.transform.GetComponent<Animator>().enabled = false;  //animatior도 false

            transform.GetComponent<AudioSource>().Stop();  //audio도 멈춘다.

             StartCoroutine(ProcessDeathAfter(2));


         }
     }
     IEnumerator ProcessDeathAfter (float delay)  //17장
     {
            yield return new WaitForSeconds(2); //2초 멈춤

            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost"); //ghost 태그를 찾음

            foreach (GameObject ghost in o)
            {
                ghost.transform.GetComponent<SpriteRenderer>().enabled = false;  //SpriteRenderer를 false로 줌
            }
            StartCoroutine(ProcessDeathAnimation(1.9f));
        
    }

    IEnumerator ProcessDeathAnimation (float delay)  //17
    {
        GameObject pacMan = GameObject.Find("PacMan"); //packMan을찾음

        pacMan.transform.localScale = new Vector3(1, 1, 1);
        pacMan.transform.localRotation = Quaternion.Euler(0, 0, 0); //방향 고정

        pacMan.transform.GetComponent<Animator>().runtimeAnimatorController = pacMan.transform.GetComponent<PacMan>().deathAnimation; //deathAnimation을 실행
        pacMan.transform.GetComponent<Animator>().enabled = true; //animator 다시 실행

        transform.GetComponent<AudioSource>().clip = backgroundAudioPacManDeath; //오디로를 backgroundAudioPacManDeath로 실행
        transform.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(delay); //1.9초 딜레이

        StartCoroutine(ProcessRestart(1));
    }
    IEnumerator ProcessRestart(float delay)  //17
    {
        if (isPlayerOneUp)  //24
            GameMenu.livesPlayerOne -= 1;  
        else
            GameMenu.livesPlayerTwo -= 1;

        if (GameMenu.livesPlayerOne == 0 && GameMenu.livesPlayerTwo == 0) //~24
        {
            playerText.transform.GetComponent<Text>().enabled = true;

            readyText.transform.GetComponent<Text>().text = "GAME OVER";  //둘다 0 GAME OVER
            readyText.transform.GetComponent<Text>().color = Color.red;

            readyText.transform.GetComponent<Text>().enabled = true;

            GameObject pacMan = GameObject.Find("PacMan");  //팩맨을 찾고
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;  //현재 SpriteRenderer를 없애고

            transform.GetComponent<AudioSource>().Stop();  //소리끄고
            StartCoroutine(ProcessGameOver(2));
        }//~20
        else if (GameMenu.livesPlayerOne == 0 || GameMenu.livesPlayerTwo == 0) //24~ 둘중 하나 체력 0
        {
            if(GameMenu.livesPlayerOne == 0)
            {
                playerText.transform.GetComponent<Text>().text = "PLAYER 1";
            }else if(GameMenu.livesPlayerTwo == 0)
            {
                playerText.transform.GetComponent<Text>().text = "PLAYER 2";
            }

            readyText.transform.GetComponent<Text>().text = "GAME OVER";
            readyText.transform.GetComponent<Text>().color = Color.red;

            readyText.transform.GetComponent<Text>().enabled = true;
            playerText.transform.GetComponent<Text>().enabled = true; //

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            yield return new WaitForSeconds(delay);

            if (!GameMenu.isOnePlayerGame)  //isOnePlayerGame이 false이면 : 게임이 1개가 아님
                isPlayerOneUp = !isPlayerOneUp;  //값을 반대로 1p, 2p 번갈아가면서

            if (isPlayerOneUp)
                StartCoroutine(StartBlinking(playerOneUp));  //반짝반짝
            else
                StartCoroutine(StartBlinking(playerTwoUp));

            RedrawBoard();

            if (isPlayerOneUp)
                playerText.transform.GetComponent<Text>().text = "PLAYER 1";
            else
                playerText.transform.GetComponent<Text>().text = "PLAYER 2";

            readyText.transform.GetComponent<Text>().text = "READY";
            readyText.transform.GetComponent<Text>().color = new Color(240f / 255f, 207f / 255f, 101f / 255f);

            yield return new WaitForSeconds(delay);

            StartCoroutine(ProcessRestartShowObjects(2));  //죽고 restart

        } //~24
        else  //체력이 0이 아닐 때 
        {    
                           
            playerText.transform.GetComponent<Text>().enabled = true; //18
            readyText.transform.GetComponent<Text>().enabled = true; //18

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            if (!GameMenu.isOnePlayerGame) 
                isPlayerOneUp = !isPlayerOneUp;  //24~ //번갈아 가면서

            if (isPlayerOneUp) 
                StartCoroutine(StartBlinking(playerOneUp));
            else
                StartCoroutine(StartBlinking(playerTwoUp));

            if (!GameMenu.isOnePlayerGame)
            {

                if (isPlayerOneUp)
                    playerText.transform.GetComponent<Text>().text = "PLAYER 1";
                else
                    playerText.transform.GetComponent<Text>().text = "PLAYER 2";
            } //~24

            RedrawBoard();
            yield return new WaitForSeconds(delay);  //딜레이 2초

            StartCoroutine(ProcessRestartShowObjects(1));// 18

        }
    }
    IEnumerator ProcessGameOver(float delay)  //20게임끝
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("GameMenu");
    } 

    IEnumerator ProcessRestartShowObjects(float delay) //18
    {
        playerText.transform.GetComponent<Text>().enabled = false;  //텍스트 false

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
            ghost.transform.GetComponent<Animator>().enabled = true;//24
            ghost.transform.GetComponent<Ghost>().MoveToStartingPosition();
        }

        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.GetComponent<Animator>().enabled = false;
       
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;
        pacMan.transform.GetComponent<PacMan>().MoveToStartingPosition();

        yield return new WaitForSeconds(delay);

        Restart();   //리스타트
    }
    
    public void Restart() //16장
    {
        Debug.Log("Restart");  //처음 위치에서 시작할 때
        int playerLevel = 0; //24~ 리스타트 하면 playerlevel 0으로 바꾸고

        if (isPlayerOneUp)
            playerLevel = playerOneLevel;  //현재 레벨 넣어줌
        else
            playerLevel = playerTwoLevel;

        GameObject.Find("PacMan").GetComponent<PacMan>().SetDifficultyForLevel(playerLevel);  //플레이어 호출
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in obj)
        {
            ghost.transform.GetComponent<Ghost>().SetDifficultyForLevel(playerLevel);
        }
        readyText.transform.GetComponent<Text>().enabled = false;
                
     
        GameObject pacMan = GameObject.Find("PacMan"); //PackMan 겜오브젝트와 
        pacMan.transform.GetComponent<PacMan>().Restart();  //PackMan 스크립트에서 Restart

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost"); //ghost 태그를 찾음

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().Restart(); //Restart함
        }

        transform.GetComponent<AudioSource>().clip = backgroundAudioNormal;
        transform.GetComponent<AudioSource>().Play();

        didStartDeateh = false;


    }
    void ResetPelletsForPlayer(int playerNum)  //24 pellet을 리셋시킨다.
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
       
       

        foreach (GameObject o in objects)
        {
            if(o.GetComponent<Tile>()!= null)
            {
                if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
                {
                    if(playerNum == 1)
                    {
                        o.GetComponent<Tile>().didConsumedPlayerOne = false;  //playerone을 false로 만듦
                    }else
                    {
                        o.GetComponent<Tile>().didConsumedPlayerTwo = false;
                    }
                }
            }
        }
    }

    void RedrawBoard()  //24
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));  //gameobject를 찾음

        foreach (GameObject o in objects)
        {
            if(o.GetComponent<Tile>() != null) //tile이있으면
            {
                if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
                {
                    if (isPlayerOneUp)
                    {
                        if (o.GetComponent<Tile>().didConsumedPlayerOne) //didConsumedPlayerOne가 TRUE
                            o.GetComponent<SpriteRenderer>().enabled = false;
                        else
                            o.GetComponent<SpriteRenderer>().enabled = true;
                    }else
                    {
                        if (o.GetComponent<Tile>().didConsumedPlayerTwo)
                            o.GetComponent<SpriteRenderer>().enabled = false;
                        else
                            o.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
        }
    }
}
