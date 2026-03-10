using Unity.Mathematics;
using UnityEngine;

public static class MinMaxAI
{
    public static (int x, int y) GetBestMove(int[,] matrix, int size)
    {
        int bestValue = int.MaxValue; // La IA vol Minimitzar
        int bestX = -1;
        int bestY = -1;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (matrix[i, j] == 0)
                {
                    // Simular moviment de la IA
                    matrix[i, j] = -1;

                    // Avaluar moviment del torn del jugador
                    int moveValue = MinMax(matrix, size, 0, true, int.MinValue, int.MaxValue);

                    // Desfer moviment simulat
                    matrix[i, j] = 0;

                    // Guardar ek millor (mínim) per a la IA
                    if (moveValue < bestValue)
                    {
                        bestValue = moveValue;
                        bestX = i;
                        bestY = j;
                    }
                }
            }
        }

        return (bestX, bestY);
    }

    private static int MinMax(int[,] matrix, int size, int depth, bool isMaximizing, int alpha, int beta)
    {
        // Casos base
        int result = EvaluateBoard(matrix, size);

        if (result == 1)
            return 10 - depth; // Guanya Max (jugador)

        if (result == -1)
            return -10 + depth; // Guanya Min (IA)

        if (result == 0)
            return 0; // Empat

        // Maximitzar torn del jugador
        if (isMaximizing)
        {
            int bestVal = int.MinValue;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        matrix[i, j] = 1; // Simula moviment del jugador
                        int val = MinMax(matrix, size, depth + 1, false, alpha, beta);
                        matrix[i, j] = 0;

                        bestVal = Mathf.Max(bestVal, val);
                        alpha = Mathf.Max(alpha, bestVal);

                        // Poda beta: el min mai triarà aquesta branca
                        if (beta <= alpha)
                            return bestVal;
                    }
                }
            }

            return bestVal;
        }
        // Minimitzar torn de la IA
        else
        {
            int bestVal = int.MaxValue;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        matrix[i, j] = -1; // Simular moviment de la IA
                        int val = MinMax(matrix, size, depth + 1, true, alpha, beta);
                        matrix[i, j] = 0;

                        bestVal = Mathf.Min(bestVal, val);
                        beta = Mathf.Min(beta, bestVal);

                        // Poda alfa: el max mai triarà aquesta branca
                        if (beta <= alpha)
                            return bestVal;
                    }
                }
            }

            return bestVal;
        }
    }

    private static int EvaluateBoard(int[,] matrix, int size)
    {
        // Comprovar files i columnes
        for (int i = 0; i < size; i++)
        {
            int rowSum = 0, colSum = 0;

            for (int j = 0; j < size; j++)
            {
                rowSum += matrix[i, j];
                colSum += matrix[j, i];
            }

            if (rowSum == size || colSum == size)
                return 1;

            if (rowSum == -size || colSum == -size)
                return -1;
        }

        // Comprovar les dues diagonals
        int diag1 = 0, diag2 = 0;
        
        for (int i = 0; i < size; i++)
        {
            diag1 += matrix[i, i];
            diag2 += matrix[size - 1 - i, i];
        }

        if (diag1 == size || diag2 == size)
            return 1;

        if (diag1 == -size || diag2 == -size)
            return -1;

        // Comprovar si queden caselles buides
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (matrix[i, j] == 0)
                    return 2;
            }
        }

        return 0; // Tauler ple sense guanyador, empat
    }
}
