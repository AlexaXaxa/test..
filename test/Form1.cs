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
        Stack<Blocks>[,] grid = new Stack<Blocks>[amountOfGridRows, amountOfGridColumns];
        

        private int CurrentHeroX = 5;
        private int CurrentHeroY = 5;

        static int[] inventory = new int[3];
        static int selectedSlot = 1;
        Label coordsHero;
        Label coordsMouse;
       
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
            MouseWheel += Form1_MouseWheel;
            MouseMove += Form1_MouseMove;

            
            
            coordsHero = new();
            coordsMouse = new();
            coordsHero.AutoSize = true;
            coordsMouse.AutoSize = true;
            coordsMouse.Dock =DockStyle.Right;
            Controls.Add(coordsHero);
            Controls.Add(coordsMouse);

            System.Windows.Forms.Timer timer = new();
            timer.Interval = 16;
            timer.Tick += Timer_Tick;
            timer.Start();

            
        }

        private void Form1_MouseMove(object? sender, MouseEventArgs e)
        {
            float scaleX = (float)bitmapWidth / ClientSize.Width;
            float scaleY = (float)bitmapHeight / ClientSize.Height;

            int bitmapMouseX = (int)(e.X * scaleX);
            int bitmapMouseY = (int)(e.Y * scaleY);

            int tileX = bitmapMouseX / tilesize;
            int tileY = bitmapMouseY / tilesize;
            Stack<Blocks> cell = grid[tileY, tileX];
            int Zmouse = cell.Count - 1;

            coordsMouse.Text = String.Format("Mouse Z - level: {0}", Zmouse.ToString());
        }

        private void Form1_MouseWheel(object? sender, MouseEventArgs e)
        {
            int notches = e.Delta / 120;
            selectedSlot = selectedSlot + notches;

            if (selectedSlot < 0)
            {
                while(selectedSlot < -(inventory.Length))
                {
                    selectedSlot = selectedSlot + inventory.Length;
                }
                selectedSlot = inventory.Length + selectedSlot;
            }
            else
            {
                while (selectedSlot > inventory.Length - 1)
                {
                    selectedSlot = selectedSlot - inventory.Length;
                }
            }
            
          
        }

        private void GridInitializer()
        {
            for (int Row = 0; Row < grid.GetLength(0); Row += 1)
            {
                for (int Column = 0; Column < grid.GetLength(1); Column += 1)
                {
                    grid[Row, Column] = new();
                    grid[Row, Column].Push(Blocks.Void);

                }
            }
        }
        private void WorldGeneration()
        {
            for (int i = 0; i<2; i++)
            {
                for (int Row = 0; Row < grid.GetLength(0); Row += 1)
                {
                    for (int Column = 0; Column < grid.GetLength(1); Column += 1)
                    {
                        grid[Row, Column].Push(Blocks.Grass);

                    }
                }
            }
            
            for (int i = 0; i < 4; i++)
            {
                for (int x = 0; x < 4; x++)
                {
                    grid[i, x].Pop();
                    grid[i, x].Push(Blocks.Water);
                }
            }

            //Player.Y = CurrentHeroY
            //Player.X = CurrentHeroX
            grid[CurrentHeroY, CurrentHeroX].Push(Blocks.Player);

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
            int bitmapMouseY = (int)(e.Location.Y * scaleY);

            int tileX = bitmapMouseX / tilesize;
            int tileY = bitmapMouseY / tilesize;


            int distanseX = Math.Abs(CurrentHeroX - tileX);
            int distanseY = Math.Abs(CurrentHeroY - tileY);

            int currentItem = inventory[selectedSlot];

            //if not equal to 
            if
                (distanseX <= 2 && distanseY <= 2 && 
                    (grid[tileY, tileX].Peek() != Blocks.Water && grid[tileY, tileX].Peek() != Blocks.DeepWater && grid[tileY, tileX].Peek() != Blocks.Player 
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
                    grid[tileY, tileX].Push(Blocks.Planks); //planks
                }
                else if (currentItem == 3)
                {
                    grid[tileY, tileX].Push(Blocks.Grass); //grass
                }

            }

        }


        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            #region HotKeys
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

            #endregion

            #region HeroMovement
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
            
            Console.WriteLine("new hero count " + grid[newHeroY, newHeroX].Count());

            //Console.WriteLine("current hero count " + (grid[CurrentHeroY, CurrentHeroX].Count()-1).ToString()+"\n");
            Console.WriteLine("current hero count " + (grid[CurrentHeroY, CurrentHeroX].Count()-1).ToString()+"\n");

            if (grid[newHeroY, newHeroX].Peek() != Blocks.Water && grid[newHeroY, newHeroX].Peek() != Blocks.Void && (grid[newHeroY, newHeroX].Count() - (grid[CurrentHeroY, CurrentHeroX].Count()-1)) < 2)
            {
                //i dont remember why i wrote this but i leave it here
                //more than 2 because beneath player must be 1)void 2)grass or something else
                //if (grid[CurrentHeroY, CurrentHeroX].Count > 2)
                //{

                //}

                //Player(MoveTo(newHeroY, newHeroX));
                grid[CurrentHeroY, CurrentHeroX].Pop();
                grid[newHeroY, newHeroX].Push(Blocks.Player);

                CurrentHeroX = newHeroX;
                CurrentHeroY = newHeroY;
 
            }


            newHeroX = CurrentHeroX;
            newHeroY = CurrentHeroY;


            #endregion
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
                           
                            case (Blocks.Water):
                                color = Brushes.Blue;
                                break;
                           
                            case (Blocks.Player):
                                color = Brushes.Red;
                                break;
                            
                            case (Blocks.DeepWater):
                                color = Brushes.DarkBlue;
                                break;
                        
                            case (Blocks.Void):
                                color = Brushes.Black;
                                break;
                           
                            case (Blocks.Planks):
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

                DisplayCoords();
            }

            Invalidate();
        }

        //private void DeepnessFilter()
        //{
        //    using (Graphics g = Graphics.FromImage(frameBuffer));

        //    for (int x = 0; x < amountOfGridColumns; x++)
        //    {
        //        for (int y = 0; y < amountOfGridRows; y++)
        //        {
        //            grid[y, x]
        //            grid[y, x].Count();
        //        }
        //    }
        //            Brush brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));


            

        //}
        private void DisplayCoords()
        {
            // int Zhero = Player.Z
            int Zhero = grid[CurrentHeroY, CurrentHeroX].Count() -2;
            string s = "Hero Z-level: " + Zhero.ToString();
            coordsHero.Text = s;
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
