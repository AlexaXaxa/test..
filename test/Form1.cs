namespace test
{
    public partial class Form1 : Form
    {
        #region vars
        private Bitmap frameBuffer = new Bitmap(bitmapWidth, bitmapHeight);
        static int bitmapWidth = 320;
        static int bitmapHeight = 240;
        


        private bool IsSharp = false;


        static int tilesize = 20;
        static int amountOfGridColumns = bitmapWidth/tilesize;
        static int amountOfGridRows = bitmapHeight/tilesize;

        //grid has amountOfGridColumns elements (arrays) and each of them have amountOfGridRows 
        Stack<Blocks>[,] grid = new Stack<Blocks>[amountOfGridRows, amountOfGridColumns];
        


        private int setHeroX = 5;
        private int setHeroY = 5;

        static int[] inventory = new int[3];
        static int selectedSlot = 1;

        Label coordsHero;
        Label coordsMouse;

        Player player;
        #endregion
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



        /// <summary>
        /// coords display method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
       /// Inventory method
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
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

            player = new Player(setHeroY, setHeroX, grid);
        }
        private void CreateAnInventory()
        {
            inventory[0] = 1; //tool
            inventory[1] = 2; //block
            inventory[2] = 3; //empty
        }

        /// <summary>
        /// Building method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseDown(object? sender, MouseEventArgs e)
        {
            float scaleX = (float) bitmapWidth / ClientSize.Width;
            float scaleY = (float) bitmapHeight / ClientSize.Height;

            int bitmapMouseX = (int)(e.X * scaleX);
            int bitmapMouseY = (int)(e.Location.Y * scaleY);

            int tileX = bitmapMouseX / tilesize;
            int tileY = bitmapMouseY / tilesize;


            int distanseX = Math.Abs(player.X - tileX);
            int distanseY = Math.Abs(player.Y - tileY);

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

        /// <summary>
        /// Walking/hotkeys method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            int newHeroX = player.X;
            int newHeroY = player.Y;

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
                newHeroX = player.X;
            }
            if (newHeroY > amountOfGridRows-1)
            {
                newHeroY = player.Y;
            }
            if (newHeroX < 0)
            {
                newHeroX = 0;
            }
            if (newHeroY < 0)
            {
                newHeroY = 0;
            }
           

            if (grid[newHeroY, newHeroX].Peek() != Blocks.Water 
                && grid[newHeroY, newHeroX].Peek() != Blocks.Void 
                && (grid[newHeroY, newHeroX].Count() - player.GroundCount() < 2))
            {
                player.MoveTo(newHeroY, newHeroX); 
            }
            #endregion
        }



        #region Rendering
        /// <summary>
        /// Rendering method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                                color = Brushes.Brown;
                                break;
                            default:
                                //grass
                                color = Brushes.LimeGreen;
                                break;
                        }

                        g.FillRectangle(color, columnPixelCoords, RowPixelCoords, tilesize - 1, tilesize - 1);
                        DeepnessFilter(g,y, x, columnPixelCoords, RowPixelCoords, tilesize - 1, tilesize - 1);
                    }
                }

                #region inventory
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
                #endregion

                DisplayCoords();
            }

            Invalidate();
        }

        private void DeepnessFilter(Graphics g, int y, int x, int columnPixelCoords, int RowPixelCoords, int ts, int ts2)
        {
            using (SolidBrush black = new SolidBrush(Color.Black))
            using(SolidBrush white = new SolidBrush(Color.White))
            {
                if (grid[y, x].Count < player.Count())
                {
                    int difference = player.Count() - grid[y, x].Count;
                    int alpha = difference * 50;
                    black.Color = Color.FromArgb(alpha, 0, 0, 0);

                    g.FillRectangle(black, columnPixelCoords, RowPixelCoords, ts, ts2);

                }
                else if (grid[y, x].Count > player.Count())
                {
                    int difference = grid[y, x].Count - player.Count();
                    int alpha = difference * 50;
                    white.Color = Color.FromArgb(alpha, 255, 255, 255);

                    g.FillRectangle(white, columnPixelCoords, RowPixelCoords, ts, ts2);
                }
            }
        }

        private void DisplayCoords()
        {
            int Zhero = player.Z();
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
        #endregion 
    }
}
