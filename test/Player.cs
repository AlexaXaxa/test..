using System;
using System.Collections.Generic;
using System.Text;

namespace test
{
    public class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Stack<Blocks>[,] Grid;

        //Blocks.Player
        public Player(int y, int x, Stack<Blocks>[,] grid)
        {
            Y = y;
            X = x;
           
            Grid = grid;
            InitializingPosition(Y, X);
        }
        public void InitializingPosition(int y, int x)
        {
            Grid[y, x].Push(Blocks.Player);
        }
        /// <summary>
        /// Count -1 (- players head)
        /// </summary>
        /// <returns></returns>
        public int GroundCount()
        {
            return Grid[Y, X].Count() -1; 
        }
        /// <summary>
        /// - players head and - void block
        /// </summary>
        /// <returns></returns>
        public int Z()
        {
            return Grid[Y, X].Count -2;
        }
       
        public void MoveTo(int newHeroY, int newHeroX)
        {
            Grid[Y, X].Pop();
            Y = newHeroY;
            X = newHeroX;
            Grid[Y, X].Push(Blocks.Player);
        }
    }
}
