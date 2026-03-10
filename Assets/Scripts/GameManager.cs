using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum States
{
    CanMove,
    CantMove
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BoxCollider2D collider;
    public GameObject token1, token2;
    public int Size = 3;
    public int[,] Matrix;
    [SerializeField] private States state = States.CanMove;
    public Camera camera;

    void Start()
    {
        Instance = this;
        Matrix = new int[Size, Size];
        Calculs.CalculateDistances(collider, Size);

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Matrix[i, j] = 0; // 0: desocupat, 1: fitxa jugador 1, -1: fitxa IA;
            }
        }
    }

    private void Update()
    {
        if (state == States.CanMove)
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 m = new Vector3(mouseScreen.x, mouseScreen.y, 10f);
            Vector3 mousepos = camera.ScreenToWorldPoint(m);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Calculs.CheckIfValidClick((Vector2)mousepos, Matrix))
                {
                    state = States.CantMove;

                    // Només juga la IA si el joc continua (resultat == 2) 
                    if(Calculs.EvaluateWin(Matrix) == 2)
                    {
                        StartCoroutine(WaitingABit());
                    }
                    else
                    {
                        state = States.CanMove;
                    }   
                }
            }
        }
    }
    private IEnumerator WaitingABit()
    {
        yield return new WaitForSeconds(1f);
        MinMaxMove();
    }
    public void MinMaxMove()
    {
        // Crear una còpia de la matriu per no modificar l'original
        int[,] matrixCopy = (int[,])Matrix.Clone();

        // Obtenir el millor moviment per a la IA (equip -1)
        var (x, y) = MinMaxAI.GetBestMove(matrixCopy, Size);

        if (x != -1 && y != -1)
        {
            DoMove(x, y, -1);
        }
            
        state = States.CanMove;
    }
    public void DoMove(int x, int y, int team)
    {
        Matrix[x, y] = team;

        if (team == 1)
        {
            Instantiate(token1, Calculs.CalculatePoint(x, y), Quaternion.identity);
        }
        else
        {
            Instantiate(token2, Calculs.CalculatePoint(x, y), Quaternion.identity);
        }
            
        int result = Calculs.EvaluateWin(Matrix);

        switch (result)
        {
            case 0:
                Debug.Log("Draw");
                break;
            case 1:
                Debug.Log("You Win");
                break;
            case -1:
                Debug.Log("You Lose");
                break;
            case 2: // El joc continua
                if(state == States.CantMove)
                {
                    state = States.CanMove;
                }                
                break;
        }
    }
}
