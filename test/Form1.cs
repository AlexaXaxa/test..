namespace test
{
    public partial class Form1 : Form
    {
        private Bitmap frameBuffer = new Bitmap(bitmapWidth, bitmapHeight);
        static int bitmapWidth = 320;
        static int bitmapHeight = 240;
        


        private bool IsSharp = false;


        static int tilesize = 20;
        private static int amountOfGridColumns = bitmapWidth/tilesize;
        static int amountOfGridRows = bitmapHeight/tilesize;


        //grid has amountOfGridColumns elements (arrays) and each of them have amountOfGridRows 
        Stack<int>[,] grid = new Stack<int>[amountOfGridRows, amountOfGridColumns];
        

        private int CurrentHeroX = 0;
        private int CurrentHeroY = 0;

        static int[] inventory = new int[3];
        static int selectedSlot = 1;

        public Form1()
        {
            InitializeComponent();

            Size = new Size(800, 600);

            DoubleBuffered = true;

            GridInitializer();
            WorldGeneration();
            CreateAnInventory();

            KeyDown += Form1_KeyDown;
            MouseDown += Form1_MouseDown;

            System.Windows.Forms.Timer timer = new();
            timer.Interval = 16;
            timer.Tick += Timer_Tick;
            timer.Start();            
        }

        private void GridInitializer()
        {
            for (int Row = 0; Row < grid.GetLength(0); Row += 1)
            {
                for (int Column = 0; Column < grid.GetLength(1); Column += 1)
                {
                    grid[Row, Column] = new();
                    grid[Row, Column].Push(4);

                }
            }
        }
        private void WorldGeneration()
        {

            for (int Row = 0; Row < grid.GetLength(0); Row += 1)
            {
                for (int Column = 0; Column < grid.GetLength(1); Column += 1)
                {
                    grid[Row, Column].Push(0);

                }
            }

            grid[1, 1].Push(1);
            grid[1, 2].Push(1);
            grid[1, 3].Push(1);

            grid[2, 1].Push(1);
            grid[2, 2].Push(3);
            grid[2, 3].Push(1);

            grid[3, 1].Push(1);
            grid[3, 2].Push(1);
            grid[3, 3].Push(1);

            grid[CurrentHeroY, CurrentHeroX].Push(2);

        }
        private void CreateAnInventory()
        {
            inventory[0] = 1; //tool
            inventory[1] = 2; //block
            inventory[2] = 3; //empty
        }


        private void Form1_MouseDown(object? sender, MouseEventArgs e)
        {
            float scaleX = (float) bitmapWidth / ClientSize.Width;
            float scaleY = (float) bitmapHeight / ClientSize.Height;

            int bitmapMouseX = (int)(e.X * scaleX);
            int bitmapMouseY = (int)(e.Y * scaleY);

            int tileX = bitmapMouseX / tilesize;
            int tileY = bitmapMouseY / tilesize;


            int distanseX = Math.Abs(CurrentHeroX - tileX);
            int distanseY = Math.Abs(CurrentHeroY - tileY);

            int currentItem = inventory[selectedSlot];

            //if not equal to 
            if
                (distanseX <= 2 && distanseY <= 2 && 
                    (grid[tileY, tileX].Peek() != 1 && grid[tileY, tileX].Peek() != 3 && grid[tileY, tileX].Peek() !=2 
                    )
                )
            {
                // кирка
                if(currentItem== 1)
                {  
                    if(grid[tileY, tileX].Count > 1)
                    {
                        grid[tileY, tileX].Pop();
                    }                      
                }
                // планки
                else if(currentItem == 2)
                {
                    grid[tileY, tileX].Push(5); //planks
                }
                else if (currentItem == 3)
                {
                    grid[tileY, tileX].Push(0); //grass
                }

            }

        }


        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D)
            {
                DoubleBuffered = !DoubleBuffered;
                return;
            }
            else if(e.Control && e.KeyCode == Keys.S)
            {
                IsSharp = !IsSharp;
                return;
            }

            if (e.KeyCode == Keys.D1) selectedSlot = 0;
            if (e.KeyCode == Keys.D2) selectedSlot = 1;
            if (e.KeyCode == Keys.D3) selectedSlot = 2;

            int newHeroX = CurrentHeroX;
            int newHeroY = CurrentHeroY;

            switch (e.KeyCode)
            {
                case (Keys.W):
                    newHeroY--;
                    break;
                case (Keys.D):
                    newHeroX++;
                    break;
                case (Keys.S):
                    newHeroY++;
                    break;
                case (Keys.A):
                    newHeroX--;
                    break;

            }
            if(newHeroX> amountOfGridColumns-1)
            {
                newHeroX = CurrentHeroX;
            }
            if (newHeroY > amountOfGridRows-1)
            {
                newHeroY = CurrentHeroY;
            }
            if (newHeroX < 0)
            {
                newHeroX = 0;
            }
            if (newHeroY < 0)
            {
                newHeroY = 0;
            }

            if (grid[newHeroY, newHeroX].Peek() == 1 || grid[newHeroY, newHeroX].Peek() == 5 || grid[newHeroY, newHeroX].Peek() == 4)
            {
                newHeroX = CurrentHeroX;
                newHeroY = CurrentHeroY;    
            }
            //more than 2 because beneath player must be 1)void 2)grass or something else
            if (grid[CurrentHeroY, CurrentHeroX].Count > 2) 
            { 
                grid[CurrentHeroY, CurrentHeroX].Pop(); 
            }
            grid[newHeroY, newHeroX].Push(2);

            CurrentHeroX = newHeroX;
            CurrentHeroY = newHeroY;
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {

            using (Graphics g = Graphics.FromImage(frameBuffer))
            {
                g.Clear(Color.Black);
                Brush color; 

                for (int x = 0; x< amountOfGridColumns; x++)
                {
                    for (int y = 0; y< amountOfGridRows; y++)
                    {
                        int columnPixelCoords = x * tilesize;
                        int RowPixelCoords = y * tilesize;
                        

                        switch (grid[y, x].Peek())
                        {
                            //water
                            case (1):
                                color = Brushes.Blue;
                                break;
                            //player
                            case (2):
                                color = Brushes.Red;
                                break;
                            //deepw water
                            case (3):
                                color = Brushes.DarkBlue;
                                break;
                            //void
                            case (4):
                                color = Brushes.Black;
                                break;
                            //planks
                            case (5):
                                color = Brushes.Yellow;
                                break;
                            default:
                                //grass
                                color = Brushes.LimeGreen;
                                break;
                        }

                        g.FillRectangle(color, columnPixelCoords, RowPixelCoords, tilesize - 1, tilesize - 1);
                    }
                }


                int slotSize = 22;
                int startX = (bitmapWidth - (3 * slotSize)) / 2;
                int startY = bitmapHeight - slotSize - 5;

                for (int i = 0; i < 3; i++)
                {
                    int x = startX + (i * slotSize);
                    g.FillRectangle(Brushes.DarkGray, x, y: startY, slotSize-2, slotSize-2);

                    if(i == selectedSlot)
                    {
                        g.DrawRectangle(Pens.Yellow, x, startY, slotSize -2, slotSize -2);
                    }
                    else
                    {
                        g.DrawRectangle(Pens.Black, x, startY, slotSize - 2, slotSize - 2);
                    }

                    int itemId = inventory[i];
                    if(itemId == 1)
                    {
                        g.FillRectangle(Brushes.Black, x + 4, startY + 4, slotSize-2-7, slotSize-2-7);
                    }
                    else if (itemId == 2) 
                    {
                        g.FillRectangle(Brushes.Yellow, x + 4, startY + 4, slotSize - 2 - 7, slotSize - 2 - 7);
                    }
                }
            }

            Invalidate();
        }




        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (IsSharp)
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                e.Graphics.DrawImage(frameBuffer, 0, 0, ClientSize.Width, ClientSize.Height);
            }
            else
            {
                e.Graphics.DrawImage(frameBuffer, 0, 0, ClientSize.Width, ClientSize.Height);
            }

        }
    }
}
