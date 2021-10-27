using System;
using System.Collections.Generic;
using System.Text;

namespace Task3
{
    public class Rules
    {
        string playerName, pcName;
        int size;
        private string[] gameElements;
        private string[,] rulesGrid;
        public Rules(string[] gameEls, string playerName, string pcName)
        {
            this.playerName = playerName;
            this.pcName = pcName;
            size = gameEls.Length;
            gameElements = new string[size];
            for (int i = 0; i < size; i++)
            {
                gameElements[i] = gameEls[i];
            }
            rulesGrid = new string[size, size];
            SetRules();
        }
        private void SetRules()
        {
            
            for (int i = 0; i < size; i++)
            {
                int n = (size - 1) / 2;
                int left = n;
                for (int j = i; j < size; j++)
                {
                    if (i == j)
                    {
                        rulesGrid[i, j] = "DRAW";
                    }
                    else
                    {
                        if(size - j + i > n)
                            rulesGrid[i, j] = pcName;
                        else
                            rulesGrid[i, j] = playerName;
                        left--;
                    }
                }
                for (int j = 0; j < i; j++)
                {
                    if (left > 0)
                    {
                        rulesGrid[i, j] = pcName;
                        left--;
                    }
                    else
                    {
                        rulesGrid[i, j] = playerName;
                    }
                }
            }
        }
        
        public string WinnerChoose(string playerTurn, string pcTurn)
        {
            int x = 0, y = 0;
            for (int i = 0; i < size; i++)
            {
                if (gameElements[i] == playerTurn)
                {
                    x = i;
                }
                if (gameElements[i] == pcTurn)
                {
                    y = i;
                }
            }
            return rulesGrid[x, y];
        }

        public string[,] GetRules()
        {
            return rulesGrid;
        }
        public string[] GetElements()
        {
            return gameElements;
        }
    }
}
