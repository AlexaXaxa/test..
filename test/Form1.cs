

namespace test
{
    public partial class Form1 : Form
    {
        static int bitmapWidth = 320;
        static int bitmapHeight = 240;
        static int tilesize = 20;
        private Bitmap frameBuffer = new Bitmap(bitmapWidth, bitmapHeight);
        private static int gridSizeX = bitmapWidth/tilesize;
        static int gridSizeY = bitmapHeight/tilesize;
        int[,] grid = new int[gridSizeX, gridSizeY];
        
        private bool IsSharp = false;

        private int CurrentHeroX = 0;
        private int CurrentHeroY = 0;

        public Form1()
        {
            InitializeComponent();

            Size = new Size(800, 600);

            DoubleBuffered = true;

    

            grid[1, 1] = 1;
            grid[1, 2] = 1;
            grid[1, 3] = 1;

            grid[2, 1] = 1;
            grid[2, 2] = 3;
            grid[2, 3] = 1;

            grid[3, 1] = 1;
            grid[3, 2] = 1;
            grid[3, 3] = 1;
           
            grid[CurrentHeroX, CurrentHeroY] = 2;


            KeyDown += Form1_KeyDown;
            MouseDown += Form1_MouseDown;

            System.Windows.Forms.Timer timer = new();
            timer.Interval = 16;
            timer.Tick += Timer_Tick;
            timer.Start();

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


            if
                (distanseX <=2 && distanseY<= 2 && 
                    (grid[tileX, tileY] != 1 && grid[tileX, tileY] != 3 && grid[tileX, tileY]!=2
                    )
                )
            {
                grid[tileX, tileY] = 4;
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
            if(newHeroX> gridSizeX-1)
            {
                newHeroX = CurrentHeroX;
            }
            if (newHeroY > gridSizeY-1)
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

            if (grid[newHeroX, newHeroY] == 1)
            {
                newHeroX = CurrentHeroX;
                newHeroY = CurrentHeroY;
            }

            grid[CurrentHeroX, CurrentHeroY] = 0;
            grid[newHeroX, newHeroY] = 2;

            CurrentHeroX = newHeroX;
            CurrentHeroY = newHeroY;
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {

            using (Graphics g = Graphics.FromImage(frameBuffer))
            {
                g.Clear(Color.Black);
                Brush color; 

                for (int  x = 0; x< gridSizeX; x++)
                {
                    for (int y = 0; y< gridSizeY; y++)
                    {
                        int columnPixelCoords = x * tilesize;
                        int RowPixelCoords = y * tilesize;
                        

                        switch (grid[x, y])
                        {
                            case (1):
                                color = Brushes.Blue;
                                break;
                            case (2):
                                color = Brushes.Red;
                                break;
                            case (3):
                                color = Brushes.DarkBlue;
                                break;
                            case (4):
                                color = Brushes.Gray;
                                break;
                            default:
                                color = Brushes.LimeGreen;
                                break;
                        }
                        g.FillRectangle(color, columnPixelCoords, RowPixelCoords, tilesize - 1, tilesize - 1);
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
