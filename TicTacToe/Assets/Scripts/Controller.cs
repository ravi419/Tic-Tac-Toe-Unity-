using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum Turn
{
    Empty,
    Cross,
    Naught
}
public class Controller : MonoBehaviour
{

    private static Controller _instance;
    public static Controller Instance
    {
        get
        {
            return _instance;
        }
    }

    //To maintain cell state
    public List<Turn> player = new List<Turn>(9);

    //To Keep track of the empty , cross and Naught cells
    public List<GameObject> spawnedList = new List<GameObject>(9);

    public Turn _turn;
    public Text Instruction , GameName;
    private string Name = "";
    public GameObject Cross, Naught , Bar;
    //Keep track of the first and last cell of the winning row
    Vector3 position1, position2;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject); return;
        }

        _instance = this;
        Debug.Log("Controller Instance Created");
        DontDestroyOnLoad(this.gameObject);
        _turn = Turn.Cross;
    }
    // Start is called before the first frame update
    void Start()
    {
        Instruction.text = "Player 1 starts";
        Name = PlayerPrefs.GetString("NameOfGame");
        GameName.text = Name;
    }

    public void Spawn(GameObject empty , int Id)
    {
        if(_turn == Turn.Cross)
        {
            spawnedList[Id] = Instantiate(Cross, empty.transform.position, Quaternion.identity);
            player[Id] = _turn;

            if (PlayerWon(_turn))
            {
                _turn = Turn.Empty;
                Instruction.text = "Player 1 has Won";
                Instantiate(Bar, CalculateCenter(), Quaternion.Euler(0, 0, CalculateSlope()));
            }
            else
            {
                Instruction.text = "Player 2 Turn";
                
                _turn = Turn.Naught;
            }

            
        }
        if (_turn == Turn.Naught)
        {
            if (Name.Equals("PlayerVsCPU"))
            {
                int bestScore = -1, bestPosition = -1 , score;
                for(int i = 0; i < 9; i++)
                {
                    if (player[i] == Turn.Empty)
                    {
                        player[i] = Turn.Naught;
                        score = MinMax(Turn.Cross, player, -1000, +1000);
                        player[i] = Turn.Empty;

                        if (bestScore < score)
                        {
                            bestScore = score;
                            bestPosition = i;
                            Debug.Log("BestScore is :" + bestScore + " and position is :" + bestPosition);
                        }
                    }
                }
                if (bestScore > -1)
                {
                    spawnedList[bestPosition] = Instantiate(Naught, spawnedList[bestPosition].transform.position, Quaternion.identity);
                    player[bestPosition] = _turn;
                }
                

            }
            else
            {
                spawnedList[Id] = Instantiate(Naught, empty.transform.position, Quaternion.identity);
                player[Id] = _turn;
            }
            
            if (PlayerWon(_turn))
            {
                _turn = Turn.Empty;
                Instruction.text = "Player 2 has Won";
                Instantiate(Bar, CalculateCenter(), Quaternion.Euler(0, 0, CalculateSlope()));
            }
            else
            {
                
                Instruction.text = "Player 1 Turn";
                _turn = Turn.Cross;
               
            }
               
        }
        if (MatchDraw())
        {
            _turn = Turn.Empty;
            Instruction.text = "Match is drawn";
        }
        Destroy(empty);
    }

    private bool IsEmptyField()
    {
        bool isEmpty = false;
        for(int i=0; i < 9; i++)
        {
            if (player[i] == Turn.Empty)
            {
                isEmpty = true;
                break;
            }
        }
        return isEmpty;
    }

    private bool PlayerWon(Turn currentPlayer)
    {
        bool playerWon = false;
        int[,] allConditions = new int[8, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 } ,{ 2, 4, 6 } };
        for(int i = 0; i < 8; i++)
        {
            if(player[allConditions[i,0]] == currentPlayer && player[allConditions[i,1]] == currentPlayer && player[allConditions[i,2]] == currentPlayer)
            {
                playerWon = true;
                position1 = spawnedList[allConditions[i, 0]].transform.position;
                position2 = spawnedList[allConditions[i, 2]].transform.position;
                break;
            }
        }
        return playerWon;
    }
    private bool MatchDraw()
    {
        bool matchDraw = false;
        if(PlayerWon(Turn.Cross)==false && PlayerWon(Turn.Naught)==false && IsEmptyField() == false)
        {
            matchDraw = true;
        }
        return matchDraw;
    }

    private Vector3 CalculateCenter()
    {
        float x = (position1.x + position2.x) / 2;
        float y = (position1.y + position2.y) / 2;
        float z = (position1.z + position2.z) / 2;

        return new Vector3(x, y , z);
    }

    private float CalculateSlope()
    {
        float slope = 0;
        if(position1.x == position2.x)
        {
            slope = 0;
        }
        else if(position1.y == position2.y)
        {
            slope = 90;
        }
        else if(position1.x > 0)
        {
            slope = -45;
        }
        else
        {
            slope = 45;
        }
        return slope;
    }

    public int MinMax(Turn currentPlayer , List<Turn> board , int alpha , int beta)
    {
        if (MatchDraw())
        {
            return 0;
        }
        if (PlayerWon(Turn.Naught))
        {
            return +1;
        }
        if (PlayerWon(Turn.Cross))
        {
            return -1;
        }
        int score;
        if(currentPlayer == Turn.Naught)
        {
            for(int i = 0; i < 9; i++)
            {
                if(board[i] == Turn.Empty)
                {
                    board[i] = Turn.Naught;
                    score = MinMax(Turn.Cross, board, alpha, beta);
                    board[i] = Turn.Empty;

                    if (score > alpha)
                    {
                        alpha = score;
                    }
                    if (alpha > beta)
                    {
                        break;
                    }

                }
            }
            return alpha;

        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == Turn.Empty)
                {
                    board[i] = Turn.Cross;
                    score = MinMax(Turn.Naught, board, alpha, beta);
                    board[i] = Turn.Empty;

                    if (score < beta)
                    {
                        beta = score;
                    }
                    if (alpha > beta)
                    {
                        break;
                    }

                }
            }
            return beta;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
