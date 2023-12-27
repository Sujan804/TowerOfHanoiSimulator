using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TowerOfHanoiSimulation
{
    public class Disk
    {
        public int Id { get; private set; }
        public int Size { get; private set; }
        public Color Color { get; set; }
        public Disk(int id, int size, Color color)
        {
            Id = id;
            Size = size;
            Color = color;
        }
    }

    public class Tower
    {
        public Stack<Disk> Disks { get; private set; }

        public Tower()
        {
            Disks = new Stack<Disk>();
        }

        public void AddDisk(Disk disk)
        {
            Disks.Push(disk);
        }

        public Disk RemoveDiskFromBottom()
        {
            Stack<Disk> tempStack = new Stack<Disk>();
            Disk bottomDisk = null;
            while (Disks.Count > 0)
            {
                tempStack.Push(Disks.Pop());
            }

            if (tempStack.Count > 0)
            {
                bottomDisk = tempStack.Pop();
            }

            while (tempStack.Count > 0)
            {
                Disks.Push(tempStack.Pop());
            }

            return bottomDisk; 
        }
        public void AddDiskToBottom(Disk disk)
        {
            Stack<Disk> tempStack = new Stack<Disk>();
            while (Disks.Count > 0)
            {
                tempStack.Push(Disks.Pop());
            }

            Disks.Push(disk);
            while (tempStack.Count > 0)
            {
                Disks.Push(tempStack.Pop());
            }
        }
    }


    public class TowerOfHanoiGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Tower[] towers;
        private int numberOfDisks = 5;
        private Disk[] disks;
        private const int TowerCount = 3;
        private const int BaseY = 400;
        private const int DiskHeight = 20;
        private const int DiskWidthUnit = 30;
        private const int DiskHeightUnit = 10;
        private const int TowerWidth = 10;
        List<(int, int)> moves;
        private Texture2D towerTexture;
        private SpriteFont _defaultFont;
        private SpriteFont font;
        private SpriteFont smallFont;
        private int score = 0;

        private double delayTime = 1; 
        private double elapsedDelayTime = 0.0;
        private bool isMovingDisk = false; 
        private int disksToMove = 0;
        int listCnt = 0;
        private bool IsNumberGive = false;
        private string inputText = string.Empty;
        private bool takingInput = true;
        private bool isPaused = false;
        private MouseState previousMouseState;
        private string status = "Running";

        public TowerOfHanoiGame(int num)
        {
            numberOfDisks = num;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            towers = new Tower[TowerCount];
            for (int i = 0; i < TowerCount; i++)
            {
                towers[i] = new Tower();
            }

            base.Initialize();
        }
       
        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("Font");
            smallFont = Content.Load<SpriteFont>("SmallFont");


            moves = SolveTowerOfHanoi(numberOfDisks);
            Console.WriteLine(moves[0].Item1);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            towerTexture = CreateColoredTexture(GraphicsDevice, Color.Wheat);

            disks = new Disk[numberOfDisks];
            Random rand = new Random();
            for (int i = 0; i < numberOfDisks; i++)
            {
                Color randomColor = new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                disks[i] = new Disk(i+1,i + 1, randomColor);
                towers[0].AddDisk(disks[i]);
                
            }
            Console.WriteLine(towers[0]);           
        }
        
        protected override  void Update(GameTime gameTime)
        {


            MouseState currentMouseState = Mouse.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if ((currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released) )
                isPaused = !isPaused;

            previousMouseState = currentMouseState;
            if (!isPaused)
            {
                if (listCnt < moves.Count && elapsedDelayTime >= delayTime)
                {

                    // Inside an asynchronous method

                    DiskUpdate(gameTime, towers[moves[listCnt].Item2 - 1], towers[moves[listCnt].Item1 - 1]);
                    listCnt++;
                    Draw(gameTime);
                    elapsedDelayTime = 0.0;


                }
                else
                {
                    // Increment elapsed delay time based on the game's elapsed time
                    elapsedDelayTime += gameTime.ElapsedGameTime.TotalSeconds;
                }

                //MoveDisks(numberOfDisks, towers[0], towers[2], towers[1], gameTime);
            }

            base.Update(gameTime);


        }
        private string GetStatus()
        {
            if (listCnt == moves.Count)
                return "Finished";
            return (isPaused ? "Paused" : "Running");
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Status" + " :" + GetStatus(), new Vector2(500, 20), Color.White);
            spriteBatch.DrawString(font, "Number of Disk :" + " " + numberOfDisks, new Vector2(20, 20), Color.White);
            spriteBatch.DrawString(smallFont, "Mouse Left Click to pause.\nEsc to Exit", new Vector2(500, 50), Color.Yellow);
            spriteBatch.DrawString(font, "Move" + " : "+ (listCnt+1), new Vector2(20, 60), Color.White);
            // Draw towers
            int towerX = GraphicsDevice.Viewport.Width / 4;
            for (int i = 0; i < TowerCount; i++)
            {
                spriteBatch.Draw(towerTexture,
                    new Rectangle(towerX+10, BaseY-200, TowerWidth, BaseY), Color.White);
                towerX += GraphicsDevice.Viewport.Width / 4;
            }

            // Draw disks
            for (int i = 0; i < TowerCount; i++)
            {
                int diskX = (GraphicsDevice.Viewport.Width / 4) * (i + 1);
                int diskY = BaseY;
                Texture2D pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
                pixelTexture.SetData(new[] { Color.White }); 
                foreach (var disk in towers[i].Disks)
                {
                    if (disk!=null)
                    {
                        spriteBatch.Draw(pixelTexture, new Rectangle(diskX - (DiskWidthUnit * disk.Size / 2) + 16, diskY + 58, DiskWidthUnit * disk.Size, DiskHeight), disk.Color);
                        diskY -= DiskHeight;
                    }
                    
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private async Task DiskUpdate(GameTime gameTime, Tower destination, Tower source)
        {
            

                destination.AddDiskToBottom(source.RemoveDiskFromBottom());
          
            
        }
        public List<(int, int)> SolveTowerOfHanoi(int numberOfDisks)
        {
            List<(int, int)> moves = new List<(int, int)>();

            MoveDisks(numberOfDisks, 1, 3, 2, moves);

            return moves;
        }

        private void MoveDisks(int disksToMove, int source, int destination, int auxiliary, List<(int, int)> moves)
        {
            if (disksToMove > 0)
            {
                MoveDisks(disksToMove - 1, source, auxiliary, destination, moves);

                moves.Add((source, destination));

                MoveDisks(disksToMove - 1, auxiliary, destination, source, moves);
            }
            else
            {
                return;
            }
        }
        private Texture2D CreateColoredTexture(GraphicsDevice graphicsDevice, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            Color[] colorData = new Color[1] { color };
            texture.SetData(colorData);
            return texture;
        }

    }
}
