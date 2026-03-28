using UnityEngine;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

public class BoardGenerator
{
    //Geração do board
    //Determina onde ficam as minas
    int _width;
    int _height;
    int _poolSize;
    int _mineCount;
    int[,] _boardRep; //Representação matemática do board
    List<int> _positionsToExclude;
    List<int> _positions;

    public int[,] GenerateBoard(int x, int y, BoardConfigSO config)
    {
        _width = config.Width;
        _height = config.Height;
        _poolSize = config.PoolSize;
        _mineCount = config.MineCount;
        //Gera  matriz que representa o board matematicamente:
        _boardRep = new int[_width, _height];
        //Calcula quais as posições do primeiro click e seus vizinhos em uma lista linear:
        CreatePositionsToExclude(x, y);
        //Gera a lista "positions" com as posições possíveis no board menos o primeiro click e seus vizinhos:
        CreatePositions();
        //Aplica Fisher_Yates para embaralhar a "_positions":
        Shuffle();
        //Pega as primeiras N (determinado pela dificuldade) positions e coloca -1 nelas na matriz "_boardRep":
        PutMinesInMatrix();
        //Percorre a matriz e coloca o numero de bombas vizinhas de cada posição:
        PutNumbersInMatrix();
        //Retorna a matriz:
        return _boardRep;
    }

    int ConvertPositionFromMatrixToList(int num1, int num2)
    {
        //Converte a posição de uma matriz para uma lista (matriz pra linear)
        int number = num2 * _width + num1; //considerando que num1 é coluna, e num2 é linha. Estou seguindo a representação física de um grid e não a convenção matemática de matrizes
        return number;
    }

    Vector2Int ConvertPositionFromListToMatrix(int number)
    {
        //Converte a posição de uma lista para uma matriz (linear para matriz)
        int num1 = number % _width; //considerando que num1 é coluna, e num2 é linha. Estou seguindo a representação física de um grid e não a convenção matemática de matrizes
        int num2 = number / _width;
        return new Vector2Int(num1, num2);
    }
    
    void CreatePositionsToExclude(int x, int y)
    {
        _positionsToExclude = new List<int>();
        for(int i = - _poolSize; i <= _poolSize; i++)
        {
            for(int j = - _poolSize; j <= _poolSize; j++)
            {
                //Para garantir que não está fora do board:
                int nx = x - i;
                int ny = y - j;
                if (nx >= 0 && nx < _width && ny >= 0 && ny < _height)
                {
                    int position = ConvertPositionFromMatrixToList(nx, ny);
                    _positionsToExclude.Add(position);
                }
            }
        }
        _positionsToExclude.Sort();
    }

    void CreatePositions()
    {
        _positions = new List<int>();
        int boardSize = _width * _height;
        int j = 0;
        for(int i = 0; i < boardSize; i++)
        {
            if(j < _positionsToExclude.Count && i == _positionsToExclude[j]) j++;
            else _positions.Add(i);
        }
    }

    void Shuffle()
    {
        for (int i = _positions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            int temp = _positions[i];
            _positions[i] = _positions[j];
            _positions[j] = temp;
        }
    }

    void PutMinesInMatrix()
    {
        for (int i = 0; i < _mineCount; i++)
        {
            Vector2Int pos = ConvertPositionFromListToMatrix(_positions[i]);
            _boardRep[pos.x, pos.y] = -1;
        }
    }

    void PutNumbersInMatrix()
    {
        for(int i = 0; i < _width; i++)
        {
            for(int j = 0; j < _height; j++)
            {
                if (_boardRep[i, j] == -1) continue;
                int minesProx = 0;
                //Para  cada posição, tenho que procurar por bombas nas 6 posições adjacentes:
                for(int n = -1; n <= 1; n++)
                {
                    for(int m = -1; m <= 1; m++)
                    {
                        int ni = i + n;
                        int mj = j + m;
                        if (ni >= 0 && ni < _width && mj >= 0  && mj < _height)
                        {
                            //Está dentro  do Board, então testa por bomba:
                            if(_boardRep[ni, mj] == -1) minesProx++;
                        }
                    }
                    
                }
                //Define a quantidade de minas próximas na posição [i, j]:
                _boardRep[i, j] = minesProx;
            }
        }
    }
}
