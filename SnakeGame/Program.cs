using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace SnakeGame
{

    static class Screen
    {
        public const int screen_width = 1800, screen_height = 900;
        public static RenderWindow win;
        public static SFML.System.Clock Clock;
        public static Random rand = new Random();
        public static int speed = 30;
    }
    static class Draw
    {
        public static void DrawSnake(SnakeNode snake)
        {
            try
            {
                Screen.win.Draw(snake.GetCircleForDraw());
                DrawSnake(snake.GetNext());
            }
            catch
            {
                return;
            }
        }
        public static void DrawEat()
        {
            foreach(var food in Food.food)
            {
                Screen.win.Draw(food.GetCircleForDraw());
            }
        }
    }
    class SnakeNode
    {
        private int Vect_X = 0;
        private int Vect_Y = 0;
        private SnakeNode Next;
        private static CircleShape circle = new CircleShape
        {
            FillColor = Color.White,
            Radius = 15,
        };

        public SnakeNode(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Next = null;

        }
        public SnakeNode GetNext()
        {
            return this.Next;
        }
        public void SetVect(int x, int y)
        {
            Vect_X = x;
            Vect_Y = y;
        }
        public void AddNext()
        {
            if (this.Next == null)
            {
                this.Next = new SnakeNode(this.X - this.Vect_X, this.Y - this.Vect_Y);
                this.Next.SetVect(this.Vect_X, this.Vect_Y);
            }
            else this.Next.AddNext();
        }
        public void Update()
        {
            this.X += this.Vect_X;
            this.Y += this.Vect_Y;

            if (this.X >= Screen.screen_width) this.X -= Screen.screen_width;
            if (this.Y >= Screen.screen_height) this.Y -= Screen.screen_height;

            if (this.X < 0) this.X += Screen.screen_width;
            if (this.Y < 0) this.Y += Screen.screen_height;

            if (this.Next != null && this.Next.CheckCollisionSelf(this.X, this.Y))
            {
                this.Damage();
            }

            this.Eat();

            if (this.Next == null) return;

            this.Next.Update();
            this.Next.SetVect(this.Vect_X, this.Vect_Y);
        }
        public CircleShape GetCircleForDraw()
        {
            circle.Position = new SFML.System.Vector2f(this.X, this.Y);
            return circle;
        }
        public int GetLenght(int i)
        {
            if (this.Next == null) return i;
            return this.Next.GetLenght(i + 1);
        }

        public int X { get; set; }
        public int Y { get; set; }
        private void Damage()
        {
            if (this.Next.Next == null)
            {
                this.Next = null;
            }
        }
        private void Eat()
        {
            foreach (var food in Food.food)
            {
                var a = Math.Abs((this.X + 15) - (food.X + 2.5));
                var b = Math.Abs((this.Y + 15) - (food.Y + 2.5));
                if (Math.Sqrt(a * a + b * b) <= 17.5)
                {
                    this.AddNext();
                    food.Spawn(Screen.rand);
                    Screen.speed -= 1;
                    return;
                }
            }
        }
        private bool CheckCollisionSelf(int x, int y) 
        {
            if (this.X == x && this.Y == y)
            {
                return true;
            }
            return false;
        }
    }
    class Food
    {
        private static CircleShape circle = new CircleShape
        {
            FillColor = Color.Red,
            Radius = 5,
        };

        static public Food[] food = new Food[15];
        public int X { get; private set; }
        public int Y { get; private set; }
        public void Spawn(Random rand)
        {
            int x = rand.Next(1800);
            int y = rand.Next(900);
            this.X = x - (x % 30) + 12;
            this.Y = y - (y % 30) + 12;
        }
        public CircleShape GetCircleForDraw()
        {
            circle.Position = new SFML.System.Vector2f(this.X, this.Y);
            return circle;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            SnakeNode sn1 = new SnakeNode(30, 30);

            //sn1.AddNext();
            //sn1.AddNext();
            //sn1.AddNext();

            Screen.win = new RenderWindow(new SFML.Window.VideoMode(Screen.screen_width, Screen.screen_height), "snake game");
            Screen.win.SetVerticalSyncEnabled(true);
            Screen.win.Closed += WinClosed;
            Screen.Clock = new SFML.System.Clock();

            RectangleShape rect = new RectangleShape
            {
                FillColor = new Color(70, 70, 70),
            };

            for (int i = 0; i < Food.food.Length; i++)
            {
                Food.food[i] = new Food();
                Food.food[i].Spawn(Screen.rand);
            }

            int Sc = 0;

            // main loop
            while (Screen.win.IsOpen)
            {
                Sc += 1;
                if (Sc == 6) Sc = 0;
                Screen.Clock.Restart();
                Screen.win.DispatchEvents();
                Screen.win.Clear(Color.Black);

                if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.Up)) { sn1.SetVect(0, -30); }
                if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.Down)) { sn1.SetVect(0, 30); }
                if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.Right)) { sn1.SetVect(30, 0); }
                if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.Left)) { sn1.SetVect(-30, 0); }

                if (Sc == 5) sn1.Update();
                Draw.DrawSnake(sn1);
                Draw.DrawEat();

                rect.Size = new SFML.System.Vector2f(1, 900);
                for (int i = 0; i < 1801; i += 30)
                {
                    rect.Position = new SFML.System.Vector2f(i, 0);
                    Screen.win.Draw(rect);
                }
                rect.Size = new SFML.System.Vector2f(1800, 1);
                for (int i = 0; i < 901; i += 30)
                {
                    rect.Position = new SFML.System.Vector2f(0, i);
                    Screen.win.Draw(rect);
                }

                Screen.win.Display();
                Screen.win.SetTitle(Convert.ToString(1000 / Math.Max(1, Screen.Clock.ElapsedTime.AsMilliseconds())) + " - " + 
                    Convert.ToString(sn1.GetLenght(1)));
            }
        }
        private static void WinClosed(object sender, EventArgs e)
        {
            Screen.win.Close();
        }
    }
}