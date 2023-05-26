using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace SFMLTesting
{
    class Program
    {
        static RenderWindow window;

        static RectangleShape line;
        static List<Sprite> nodes = new List<Sprite>();
        static List<RectangleShape> edges = new List<RectangleShape>();

        static int numberOfNodes = 3;
        static int nodeCircleRadius = 300;
        static int selectedNode;
        static bool movingNode = false;
        static Vector2i movingNodeOffset;
        static bool dragAndDrop = true;
        static int edgeWidth = 10;

        static bool leftMouseButtonDown = false;

        static bool debugTextEnabled = false;
        static Text debugText;
        static Clock fpsClock = new Clock();


        static void Main(string[] args)
        {
            window = new RenderWindow(new VideoMode(1280, 720), "Hello wurld");
            //window.SetFramerateLimit(60);
            //window.SetVerticalSyncEnabled(true);

            window.Closed += (sender, e) =>
            {
                window.Close();
            };

            Texture nodeTexture = new Texture("Circle.png");
            Font poppinsFont = new Font("Poppins-Regular.ttf");

            debugText = new Text("FPS : ", poppinsFont);
            debugText.Position = new Vector2f(0, 0);
            debugText.Color = Color.Black;

            for (int i = 0; i < numberOfNodes; i++)
            {
                Sprite node = new Sprite(nodeTexture);
                node.Origin = new Vector2f(node.GetLocalBounds().Width / 2, node.GetLocalBounds().Height / 2);
                node.Scale = new Vector2f(3, 3);

                float angle = (float)(2 * Math.PI) / numberOfNodes * (i + 1);
                float x = (window.Size.X / 2) + (nodeCircleRadius * (float)Math.Cos(angle - (Math.PI / 2)));
                float y = (window.Size.Y / 2) + (nodeCircleRadius * (float)Math.Sin(angle - (Math.PI / 2)));

                node.Position = numberOfNodes > 1 ? new Vector2f(x, y) : new Vector2f(window.Size.X / 2, window.Size.Y / 2);

                nodes.Add(node);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = nodes.Count - 1; j >= i; j--)
                {
                    edges.Add(VectorLine(nodes[i].Position, nodes[j].Position, 1));
                }
            }

            int deltaFrame = 0;

            while (window.IsOpen)
            {
                window.DispatchEvents();

                if (fpsClock.ElapsedTime.AsSeconds() >= 1)
                {
                    debugText.DisplayedString = $"FPS : {deltaFrame}";
                    fpsClock.Restart();
                    deltaFrame = 0;
                }

                Update();

                window.Clear(Color.Cyan);
                Render();
                window.Draw(debugText);
                window.Display();

                deltaFrame++;
            }
        }

        static void Update()
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left) && !leftMouseButtonDown)
            {
                bool swapped = false;
                leftMouseButtonDown = true;

                for (int i = 0; i < nodes.Count; i++)
                {
                    if (Math.Sqrt(Math.Pow(nodes[i].Position.X - Mouse.GetPosition(window).X, 2) + Math.Pow(nodes[i].Position.Y - Mouse.GetPosition(window).Y, 2)) < (nodes[i].GetGlobalBounds().Width / 2))
                    {
                        movingNodeOffset = (Vector2i)nodes[i].Position - Mouse.GetPosition(window);

                        if (!dragAndDrop)
                        {
                            movingNode = !movingNode;
                        }

                        else
                        {
                            movingNode = true;
                        }

                        if (selectedNode != i && !swapped)
                        {
                            selectedNode = i;
                            swapped = true;
                        }
                    }
                }
            }

            else if (!Mouse.IsButtonPressed(Mouse.Button.Left) && leftMouseButtonDown)
            {
                if (dragAndDrop)
                {
                    movingNode = false;
                }

                leftMouseButtonDown = false;
            }

            if (movingNode)
            {
                nodes[selectedNode].Position = (Vector2f)(Mouse.GetPosition(window) + movingNodeOffset);
            }





            edges.Clear();

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = nodes.Count - 1; j >= i; j--)
                {
                    edges.Add(VectorLine(nodes[i].Position, nodes[j].Position, edgeWidth));
                }
            }
        }

        static void Render()
        {
            foreach (RectangleShape edge in edges)
            {
                window.Draw(edge);
            }

            foreach (Sprite node in nodes)
            {
                window.Draw(node);
            }

            if (debugTextEnabled)
            {
                window.Draw(debugText);
            }
        }

        static RectangleShape VectorLine(Vector2f pos1, Vector2f pos2, float width)
        {
            float length = (float)Math.Sqrt(Math.Pow(pos2.Y - pos1.Y, 2) + Math.Pow(pos2.X - pos1.X, 2));

            RectangleShape newLine = new RectangleShape(new Vector2f(width, length));

            newLine.Position = pos1;

            float angle = (float)Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X);
            newLine.Rotation = (float)(angle * (180 / Math.PI)) - 90;

            newLine.FillColor = Color.Red;

            newLine.Origin = new Vector2f(newLine.GetLocalBounds().Width / 2, 0);

            return newLine;
        }
    }
}
