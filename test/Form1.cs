

namespace test
{
    public partial class Form1 : Form
    {
        private Bitmap frameBuffer = new Bitmap(320, 420);
        int[,] grid = new int[10, 10];
        private int tilesize = 20;
        private bool IsSharp = false;

        public Form1()
        {
            InitializeComponent();

            Size = new Size(800, 600);


            DoubleBuffered = true;

            //for (int i = 0; i < 10; i++)
            //{
            //    for (int q = 0; q < 10; q++)
            //    {
            //        grid[i, q] = 0;
            //    }

            //}

            grid[5, 9] = 1;
            grid[3, 5] = 1; grid[3, 6] = 1;

            grid[3, 7] = 2;


          
            KeyDown += Form1_KeyDown;

            System.Windows.Forms.Timer timer = new();
            timer.Interval = 16;
            timer.Tick += Timer_Tick;
            timer.Start();

            
            

        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D)
            {
                DoubleBuffered = !DoubleBuffered;
            }
            else if(e.Control && e.KeyCode == Keys.S)
            {
                IsSharp = !IsSharp;
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(frameBuffer))
            {
                g.Clear(Color.Black);
                Brush color; 

                for (int  y = 0; y<10; y++)
                {
                    for (int x = 0; x<10; x++)
                    {
                        int columnPixelCoords = x * tilesize;
                        int RowPixelCoords = y * tilesize;
                        

                        switch (grid[y, x])
                        {
                            case (1):
                                color = Brushes.LimeGreen;
                                break;
                            case (2):
                                color = Brushes.Red;
                                break;
                            default:
                                color = Brushes.Black;
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
