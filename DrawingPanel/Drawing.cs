using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using TheShip;
using TheStar;
using Projectile;
using Vector;
using System.IO;
using System.Timers;

namespace DrawingPanel
{
    public class DrawPanel : Panel
    {

        private World World;
        private Image[] shipSprite;
        private Image[] projectileSprite;
        private Image[] shipThrustSprites;
        private Image FireGif1;
        private Image FireGif2;
        private int time = 0;
        private Image Death1;
        private Image Death2;
        private Image Death3;
        private Image Death4;

        public DrawPanel(World w)
        {
            this.DoubleBuffered = true;
            World = w;

            string str = @"../../../Images/";

            shipSprite = new Image[8]
            {
          Image.FromFile(str + "ship-coast-green.png"),
          Image.FromFile(str + "ship-coast-red.png"),
          Image.FromFile(str + "ship-coast-blue.png"),
          Image.FromFile(str + "ship-coast-white.png"),
          Image.FromFile(str + "ship-coast-yellow.png"),
          Image.FromFile(str + "ship-coast-violet.png"),
          Image.FromFile(str + "ship-coast-brown.png"),
          Image.FromFile(str + "ship-coast-grey.png")
            };
            projectileSprite = new Image[8]
            {
          Image.FromFile(str + "shot-green.png"),
          Image.FromFile(str + "shot-red.png"),
          Image.FromFile(str + "shot-blue.png"),
          Image.FromFile(str + "shot-white.png"),
          Image.FromFile(str + "shot-yellow.png"),
          Image.FromFile(str + "shot-violet.png"),
          Image.FromFile(str + "shot-brown.png"),
          Image.FromFile(str + "shot-grey.png")
            };
            shipThrustSprites = new Image[8]
            {
          Image.FromFile(str + "ship-thrust-green.png"),
          Image.FromFile(str + "ship-thrust-red.png"),
          Image.FromFile(str + "ship-thrust-blue.png"),
          Image.FromFile(str + "ship-thrust-white.png"),
          Image.FromFile(str + "ship-thrust-yellow.png"),
          Image.FromFile(str + "ship-thrust-violet.png"),
          Image.FromFile(str + "ship-thrust-brown.png"),
          Image.FromFile(str + "ship-thrust-grey.png")
            };

           FireGif1 = Image.FromFile(str + "FireFrame1.png");
           FireGif2 = Image.FromFile(str + "FireFrame2.png");

           Death1= Image.FromFile(str + "Explo1.png");
           Death2= Image.FromFile(str + "Explo2.png");
           Death3= Image.FromFile(str + "Explo3.png");
           Death4= Image.FromFile(str + "Explo4.png");
            


            System.Timers.Timer GifTimer = new System.Timers.Timer();
            GifTimer.Interval = 250;
            GifTimer.Elapsed += OnTimedEvent;

            GifTimer.Start();


        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            time++;
        }
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }

        public delegate void ObjectDrawer(object o, PaintEventArgs e);

        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            //perform the transformation
            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            //draw the Object
            drawer(o, e);
            // then undo the Transformation 
            e.Graphics.ResetTransform();
        }

        private void ShipDrawer(object o, PaintEventArgs e)
        {
            int width = 35;
            int height = 35;
            Ship s = o as Ship;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            if (s.GetAccel())
            {

                Image image = this.shipThrustSprites[s.GetID() % this.shipThrustSprites.Length];
                e.Graphics.DrawImage(image, -(height / 2), -(width / 2), width, height);
            }
            else
            {
                Image image = this.shipSprite[s.GetID() % this.shipSprite.Length];
                e.Graphics.DrawImage(image, -(height / 2), -(width / 2), width, height);
            }
            
        }
        private void DeathDrawer(object o, PaintEventArgs e)
        {
            int starWidth = 50;
            int StarHeight = 50;
    
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            if (time % 2 == 0)
            {
                e.Graphics.DrawImage(Death1, -(StarHeight / 2), -(starWidth / 2), starWidth, StarHeight);
            }
            if (time % 2 == 1)
            {
                e.Graphics.DrawImage(Death2, -(StarHeight / 2), -(starWidth / 2), starWidth, StarHeight);
            }           
            else
            {
                e.Graphics.DrawImage(Death3, -(StarHeight / 2), -(starWidth / 2), starWidth, StarHeight);
            }

        }
        private void StarDrawer(object o, PaintEventArgs e)
        {
            int starWidth = 50;
            int StarHeight = 50;
            Star s = o as Star;


            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            if(time%2 == 0)
            {
                e.Graphics.DrawImage(FireGif1, -(StarHeight / 2), -(starWidth / 2), starWidth, StarHeight);
            }
            else
            {          
                e.Graphics.DrawImage(FireGif2, -(StarHeight / 2), -(starWidth / 2), starWidth, StarHeight);
            }


        }
        private void projDrawer(object o, PaintEventArgs e)
        {
            int projWidth = 25;
            int projHeight = 25;

            Proj s = o as Proj;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            Image image = projectileSprite[s.GetOwner() % projectileSprite.Length];
            e.Graphics.DrawImage(image, -(projHeight / 2), -(projWidth / 2), projWidth, projHeight);

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            lock (this)
            {
                foreach (Ship s in World.GetShips().Values)
                {
                    DrawObjectWithTransform(e, s, this.Size.Width, s.GetLocation().GetX(), s.GetLocation().GetY(), s.GetOrientation().ToAngle(), ShipDrawer);
                }
                foreach(Ship s in World.GetDead().Values)
                {
                    DrawObjectWithTransform(e, s, this.Size.Width, s.GetLocation().GetX(), s.GetLocation().GetY(), s.GetOrientation().ToAngle(), DeathDrawer);
                }
                foreach (Proj p in World.GetProj().Values)
                {
                    DrawObjectWithTransform(e, p, this.Size.Width, p.GetLocation().GetX(), p.GetLocation().GetY(), p.GetOrientation().ToAngle(), projDrawer);
                }
                foreach (Star s in World.star.Values)
                {
                    DrawObjectWithTransform(e, s, this.Size.Width, s.GetLocation().GetX(), s.GetLocation().GetY(), 0, StarDrawer);
                }
            }
            base.OnPaint(e);
        }
    }
    public class Score : Panel
    {
        private World World;

        private HashSet<int> ShipScore;
        public Dictionary<int, int> ScoreBoard;

        public Score(World w)
        {
            World = w;
            this.DoubleBuffered = true;
            ShipScore = new HashSet<int>();
            ScoreBoard = new Dictionary<int, int>();

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            int axis = 10;

            lock (this.World.Score)
            {
               try
                {
                    foreach (KeyValuePair<int, int> entry in this.World.Score)
                    {
                        Ship s;

                        if (World.GetShips().ContainsKey(entry.Key))
                        {
                           s = World.GetShips()[entry.Key];
                        }
                        else
                        {
                           s = World.GetDead()[entry.Key];
                        }
                     
                        e.Graphics.DrawString(s.GetName()+ " : Score:" + s.GetScore()+" : HP " + s.GetHP(), drawFont, drawBrush, 1.0F,axis);

                        axis = axis + 20;
                    }
                }
                catch
                {
                    return;
                }


            }
            base.OnPaint(e);
        }
    }
}

