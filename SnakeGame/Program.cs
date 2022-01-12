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
        public const int ScreenWidth = 1800, ScreenHeight = 900;
        public static RenderWindow Win;
        public static SFML.System.Clock Clock;
        public static SFML.System.Clock Timer;
        public static Random Rand = new Random();
        public static int Speed = 30;
        public static int MaxTimer = 3000;
        public static void RestartTimer()
        {
            Screen.Timer.Restart();
            Screen.MaxTimer -= 10;
        }
    }
    static class Draw
    {
        public static void DrawSnake(SnakeNode snake)
        {
            try
            {
                Screen.Win.Draw(snake.GetCircleForDraw());
                DrawSnake(snake.GetNext());
            }
            catch
            {
                return;
            }
        }
        public static void DrawEat()
        {
            foreach(var food in Food.FoodArray)
            {
                Screen.Win.Draw(food.GetCircleForDraw());
            }
        }
    }
    class SnakeNode
    {
        private int Vect_X = 0;
        private int Vect_Y = 0;
        private SnakeNode Next;
        private static CircleShape Circle = new CircleShape
        {
            FillColor = Color.White,
            Radius = 15,
        };

        public SnakeNode(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public void Update()
        {
            this.X += this.Vect_X;
            this.Y += this.Vect_Y;

            if (this.X >= Screen.ScreenWidth) this.X -= Screen.ScreenWidth;
            if (this.Y >= Screen.ScreenHeight) this.Y -= Screen.ScreenHeight;

            if (this.X < 0) this.X += Screen.ScreenWidth;
            if (this.Y < 0) this.Y += Screen.ScreenHeight;

            this.CheckCollisionSelf(this.X, this.Y);

            if (this.Next != null)
            {
                this.Next.Update();
                this.Next.SetVect(this.Vect_X, this.Vect_Y);
            }

            this.Eat();
        }
        public SnakeNode GetNext() // try to delete
        {
            return this.Next;
        }
        public CircleShape GetCircleForDraw()
        {
            Circle.Position = new SFML.System.Vector2f(this.X, this.Y);
            return Circle;
        }
        public int GetLenght(int i)
        {
            if (this.Next == null) return i;
            return this.Next.GetLenght(i + 1);
        }
        public void SetVect(int x, int y) // try to upgrade
        {
            Vect_X = x;
            Vect_Y = y;
        }
        public void Died()
        {
            this.Next = null;
        }

        private int X { get; set; }
        private int Y { get; set; }
        private void AddNext()
        {
            if (this.Next == null)
            {
                this.Next = new SnakeNode(this.X - this.Vect_X, this.Y - this.Vect_Y);
                this.Next.SetVect(this.Vect_X, this.Vect_Y);
            }
            else this.Next.AddNext();
        }
        private void Eat()
        {
            foreach (var food in Food.FoodArray)
            {
                var a = Math.Abs((this.X + 15) - (food.X + 2.5));
                var b = Math.Abs((this.Y + 15) - (food.Y + 2.5));
                if (Math.Sqrt(a * a + b * b) <= 17.5)
                {
                    this.AddNext();
                    food.Spawn(Screen.Rand);
                    Screen.RestartTimer();
                    return;
                }
            }
        }
        private void CheckCollisionSelf(int x, int y) 
        {
            if (this.Next == null) return;
            if (x == this.Next.X && y == this.Next.Y)
            {
                this.Next = null;
                return;
            }
            this.Next.CheckCollisionSelf(x, y);
        }
    }
    class Food
    {
        private static CircleShape Circle = new CircleShape
        {
            FillColor = Color.Red,
            Radius = 5,
        };
        public static Food[] FoodArray = new Food[15];
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
            Circle.Position = new SFML.System.Vector2f(this.X, this.Y);
            return Circle;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            SnakeNode sn1 = new SnakeNode(30, 30);

            Screen.Win = new RenderWindow(new SFML.Window.VideoMode(Screen.ScreenWidth, Screen.ScreenHeight), "snake game");
            Screen.Win.SetVerticalSyncEnabled(true);
            Screen.Win.Closed += WinClosed;
            Screen.Clock = new SFML.System.Clock();
            Screen.Timer = new SFML.System.Clock();

            RectangleShape rect = new RectangleShape
            {
                FillColor = new Color(70, 70, 70),
            };

            for (int i = 0; i < Food.FoodArray.Length; i++)
            {
                Food.FoodArray[i] = new Food();
                Food.FoodArray[i].Spawn(Screen.Rand);
            }

            int Sc = 0;

            // main loop
            while (Screen.Win.IsOpen)
            {
                Sc += 1;
                if (Sc == 6) Sc = 0;
                Screen.Clock.Restart();
                Screen.Win.DispatchEvents();
                Screen.Win.Clear(Color.Black);

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
                    Screen.Win.Draw(rect);
                }
                rect.Size = new SFML.System.Vector2f(1800, 1);
                for (int i = 0; i < 901; i += 30)
                {
                    rect.Position = new SFML.System.Vector2f(0, i);
                    Screen.Win.Draw(rect);
                }

                if (Screen.Timer.ElapsedTime.AsMilliseconds() > Screen.MaxTimer)
                {
                    sn1.Died();
                }

                Screen.Win.Display();
                Screen.Win.SetTitle(Convert.ToString(1000 / Math.Max(1, Screen.Clock.ElapsedTime.AsMilliseconds())) + " - " + 
                    Convert.ToString(Screen.MaxTimer - Screen.Timer.ElapsedTime.AsMilliseconds()) + " - " +
                    Convert.ToString(sn1.GetLenght(1)));
            }
        }
        private static void WinClosed(object sender, EventArgs e)
        {
            Screen.Win.Close();
        }
    }
}