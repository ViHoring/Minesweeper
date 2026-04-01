using System.Collections.Generic;
using UnityEngine;

public class BoardSolver
{
    enum SolverCellState
    {
        Unknown,
        Revealed,
        Flagged
    }

    class Constraint
    {
        public Vector2Int Position;
        public List<Vector2Int> UnknownNeighbors;
        public int RemainingMines;
    }

    int[,] _boardRep;
    SolverCellState[,] _knowledge;
    int _width;
    int _height;

    public bool IsSolvableWithoutGuess(int[,] boardRep, int startX, int startY)
    {
        _boardRep = boardRep;
        _width = boardRep.GetLength(0);
        _height = boardRep.GetLength(1);

        _knowledge = new SolverCellState[_width, _height];

        // Primeiro clique
        Reveal(startX, startY);

        bool madeProgress;

        do
        {
            madeProgress = false;

            // Regras básicas
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_knowledge[x, y] != SolverCellState.Revealed)
                        continue;

                    if (_boardRep[x, y] == -1)
                        return false;

                    if (_boardRep[x, y] == 0)
                        continue;

                    if (ApplyRulesToTile(x, y))
                        madeProgress = true;
                }
            }

            // Nova camada: regras de subconjunto
            if (ApplySubsetRules())
                madeProgress = true;

        } while (madeProgress);

        return AllSafeTilesRevealed();
    }

    bool ApplyRulesToTile(int x, int y)
    {
        int number = _boardRep[x, y];
        int flaggedCount = CountAdjacentFlagged(x, y);
        List<Vector2Int> unknownNeighbors = GetAdjacentUnknown(x, y);

        bool madeProgress = false;

        // Regra 1:
        // Se já marquei todas as minas desse número, o resto é seguro
        if (flaggedCount == number && unknownNeighbors.Count > 0)
        {
            foreach (Vector2Int pos in unknownNeighbors)
            {
                Reveal(pos.x, pos.y);
                madeProgress = true;
            }
        }

        // Regra 2:
        // Se faltam exatamente N minas e existem exatamente N desconhecidos,
        // então todos os desconhecidos são minas
        int minesRemaining = number - flaggedCount;

        if (minesRemaining > 0 && unknownNeighbors.Count == minesRemaining)
        {
            foreach (Vector2Int pos in unknownNeighbors)
            {
                if (_knowledge[pos.x, pos.y] == SolverCellState.Unknown)
                {
                    _knowledge[pos.x, pos.y] = SolverCellState.Flagged;
                    madeProgress = true;
                }
            }
        }

        return madeProgress;
    }

    bool ApplySubsetRules()
    {
        bool madeProgress = false;
        List<Constraint> constraints = BuildConstraints();

        for (int i = 0; i < constraints.Count; i++)
        {
            for (int j = 0; j < constraints.Count; j++)
            {
                if (i == j) continue;

                Constraint a = constraints[i];
                Constraint b = constraints[j];

                if (a.UnknownNeighbors.Count == 0 || b.UnknownNeighbors.Count == 0)
                    continue;

                // Queremos testar se A é subconjunto de B
                if (!IsSubset(a.UnknownNeighbors, b.UnknownNeighbors))
                    continue;

                List<Vector2Int> extraCells = Subtract(b.UnknownNeighbors, a.UnknownNeighbors);

                if (extraCells.Count == 0)
                    continue;

                // Regra A:
                // se ambos precisam da mesma quantidade de minas,
                // então as células extras de B são seguras
                if (a.RemainingMines == b.RemainingMines)
                {
                    foreach (Vector2Int pos in extraCells)
                    {
                        if (_knowledge[pos.x, pos.y] == SolverCellState.Unknown)
                        {
                            Reveal(pos.x, pos.y);
                            madeProgress = true;
                        }
                    }
                }

                // Regra B:
                // se a diferença de minas restantes é exatamente igual
                // ao número de células extras, então todas as extras são minas
                int mineDifference = b.RemainingMines - a.RemainingMines;

                if (mineDifference > 0 && mineDifference == extraCells.Count)
                {
                    foreach (Vector2Int pos in extraCells)
                    {
                        if (_knowledge[pos.x, pos.y] == SolverCellState.Unknown)
                        {
                            _knowledge[pos.x, pos.y] = SolverCellState.Flagged;
                            madeProgress = true;
                        }
                    }
                }
            }
        }

        return madeProgress;
    }

    List<Constraint> BuildConstraints()
    {
        List<Constraint> constraints = new List<Constraint>();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_knowledge[x, y] != SolverCellState.Revealed)
                    continue;

                int number = _boardRep[x, y];

                if (number <= 0)
                    continue;

                int flaggedCount = CountAdjacentFlagged(x, y);
                List<Vector2Int> unknownNeighbors = GetAdjacentUnknown(x, y);
                int remainingMines = number - flaggedCount;

                if (unknownNeighbors.Count == 0)
                    continue;

                Constraint constraint = new Constraint
                {
                    Position = new Vector2Int(x, y),
                    UnknownNeighbors = unknownNeighbors,
                    RemainingMines = remainingMines
                };

                constraints.Add(constraint);
            }
        }

        return constraints;
    }

    bool IsSubset(List<Vector2Int> subset, List<Vector2Int> superset)
    {
        HashSet<Vector2Int> superSetLookup = new HashSet<Vector2Int>(superset);

        foreach (Vector2Int cell in subset)
        {
            if (!superSetLookup.Contains(cell))
                return false;
        }

        return true;
    }

    List<Vector2Int> Subtract(List<Vector2Int> source, List<Vector2Int> toRemove)
    {
        HashSet<Vector2Int> removeLookup = new HashSet<Vector2Int>(toRemove);
        List<Vector2Int> result = new List<Vector2Int>();

        foreach (Vector2Int cell in source)
        {
            if (!removeLookup.Contains(cell))
                result.Add(cell);
        }

        return result;
    }

    void Reveal(int startX, int startY)
    {
        if (!IsInside(startX, startY))
            return;

        if (_knowledge[startX, startY] == SolverCellState.Revealed)
            return;

        if (_knowledge[startX, startY] == SolverCellState.Flagged)
            return;

        if (_boardRep[startX, startY] == -1)
            return;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int x = current.x;
            int y = current.y;

            if (!IsInside(x, y))
                continue;

            if (_knowledge[x, y] == SolverCellState.Revealed)
                continue;

            if (_knowledge[x, y] == SolverCellState.Flagged)
                continue;

            if (_boardRep[x, y] == -1)
                continue;

            _knowledge[x, y] = SolverCellState.Revealed;

            // Se é blank, abre todos os vizinhos automaticamente
            if (_boardRep[x, y] == 0)
            {
                for (int nx = x - 1; nx <= x + 1; nx++)
                {
                    for (int ny = y - 1; ny <= y + 1; ny++)
                    {
                        if (!IsInside(nx, ny))
                            continue;

                        if (_knowledge[nx, ny] == SolverCellState.Unknown)
                            queue.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }
        }
    }

    int CountAdjacentFlagged(int x, int y)
    {
        int count = 0;

        for (int nx = x - 1; nx <= x + 1; nx++)
        {
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                if (!IsInside(nx, ny))
                    continue;

                if (nx == x && ny == y)
                    continue;

                if (_knowledge[nx, ny] == SolverCellState.Flagged)
                    count++;
            }
        }

        return count;
    }

    List<Vector2Int> GetAdjacentUnknown(int x, int y)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for (int nx = x - 1; nx <= x + 1; nx++)
        {
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                if (!IsInside(nx, ny))
                    continue;

                if (nx == x && ny == y)
                    continue;

                if (_knowledge[nx, ny] == SolverCellState.Unknown)
                    result.Add(new Vector2Int(nx, ny));
            }
        }

        return result;
    }

    bool AllSafeTilesRevealed()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_boardRep[x, y] != -1 &&
                    _knowledge[x, y] != SolverCellState.Revealed)
                {
                    return false;
                }
            }
        }

        return true;
    }

    bool IsInside(int x, int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }
}