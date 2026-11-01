using System;
using System.Collections.Generic;
using System.Text;

namespace test
{
    public class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        //Blocks.Player
        public Player()
        {
        }

        public int GroundCount()
        {
            //count -1 (- players head) Its not the same as Z. Z is -2 (- players head and - void block)
            //return grid[Y, X] -1;
            return 0;
        }
        public int Z()
        {
            //return grid[Y, X].Count -2;
            return 0;
        }
        public void MoveTo(int newHeroY, int newHeroX)
        {
            //grid[CurrentHeroY, CurrentHeroX].Pop();
            //grid[newHeroY, newHeroX].Push(Blocks.Player);
        }
    }
}
