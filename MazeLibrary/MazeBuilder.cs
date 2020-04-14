using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Primitives;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace MazeLibrary
{
    public class MazeBuilder
    {
        public int x, y;
        public Node[,] nodes;
        public List<Edge> edges;

        public MazeBuilder(int _x, int _y)
        {
            this.x = _x; this.y = _y;
            nodes = new Node[x, y];
            edges = new List<Edge>();
            InitializeMaze();
        }

        public void InitializeMaze()
        {
            for (int x = 0; x < this.x; x++)
            {
                for (int y = 0; y < this.y; y++)
                {
                    nodes[x, y] = new Node() { x = x, y = y, group = x * this.y + y };
                }
            }
        }

        public void CreateMazeWithOnePath()
        {
            var list = new List<Node>();
            foreach (var node in nodes)
            {
                list.Add(node);
            }

            var rnd = new Random();
            for (int i = 0; i < this.x * this.y; i++)
            {
                var n = list[rnd.Next(list.Count - 1)];
                var direction = rnd.Next(100);
                try
                {
                    if (direction < 40)
                    {
                        AddEdge(n, nodes[n.x + 1, n.y]);
                    }
                    if (direction > 60)
                    {
                        AddEdge(n, nodes[n.x, n.y + 1]);
                    }
                }
                catch (IndexOutOfRangeException) { }
                finally
                {
                    list.Remove(n);
                }
            }
            while (HasMoreThanOneGroup())
            {
                foreach (var m in NodesInGroup(0))
                {
                    try
                    {
                        if (nodes[m.x + 1, m.y].group != 0)
                        {
                            AddEdge(m, nodes[m.x + 1, m.y]);
                        }
                    }
                    catch (IndexOutOfRangeException) { };
                    try
                    {
                        if (nodes[m.x, m.y + 1].group != 0)
                        {
                            AddEdge(m, nodes[m.x, m.y + 1]);
                        }
                    }
                    catch (IndexOutOfRangeException) { }
                }
            }
        }

        public bool HasMoreThanOneGroup()
        {
            foreach (var node in nodes)
            {
                if (node.group > 0) return true;
            }
            return false;
        }

        public IEnumerable<Node> NodesInGroup(int group)
        {
            foreach (var node in nodes)
            {
                if (node.group == group)
                    yield return node;
            }
        }

        public void MergeGroups(int g1, int g2)
        {
            foreach (var node in nodes)
            {
                if (node.group == g1 || node.group == g2)
                    node.group = Math.Min(g1, g2);
            }
        }

        public bool HasEdge(Node n1, Node n2)
        {
            foreach (var edge in edges)
            {
                if ((edge.nodes[0] == n1 && edge.nodes[1] == n2) || (edge.nodes[1] == n1 && edge.nodes[0] == n2))
                    return true;
            }
            return false;
        }

        public bool AreAdjacentEdges(Node n1, Node n2)
        {
            return (n1.x == n2.x && Math.Abs(n1.y - n2.y) <= 1)
                || (Math.Abs(n1.x - n2.x) <= 1 && n1.y == n2.y);
        }

        public void AddEdge(Node n1, Node n2)
        {
            if (!AreAdjacentEdges(n1, n2))
            {
                throw new InvalidOperationException("These nodes are not adjacent; no edge can be created.");
            }
            if (!HasEdge(n1, n2))
            {
                edges.Add(new Edge(n1, n2));
                if (n1.x < n2.x)
                {
                    n1.right = false;
                    n2.left = false;
                }
                if (n1.x > n2.x)
                {
                    n1.left = false;
                    n2.right = false;
                }
                if (n1.y < n2.y)
                {
                    n1.bottom = false;
                    n2.top = false;
                }
                if (n1.y > n2.y)
                {
                    n1.top = false;
                    n2.bottom = false;
                }
                MergeGroups(n1.group, n2.group);
            }
        }

        public void CreateImage(Stream stream)
        {
            const int scale = 20;
            var image = new Image<Rgba32>(x * scale, y * scale);
            var builder = new PathBuilder();
            image.Mutate(ctx => ctx.Fill(Color.White));
            foreach (var n in nodes)
            {
                var origin = new PointF(n.x * scale, n.y * scale);
                builder.SetOrigin(origin);
                if (n.left)
                {
                    builder.StartFigure();
                    builder.AddLine(0, 0, 0, scale);
                    builder.CloseFigure();
                }
                if (n.bottom)
                {
                    builder.StartFigure();
                    builder.AddLine(0, scale, scale, scale);
                    builder.CloseFigure();
                }
                if (n.right)
                {
                    builder.StartFigure();
                    builder.AddLine(scale, scale, scale, 0);
                    builder.CloseFigure();
                }
                if (n.top)
                {
                    builder.StartFigure();
                    builder.AddLine(scale, 0, 0, 0);
                    builder.CloseFigure();
                }
            }
            var path = builder.Build();
            image.Mutate(ctx => ctx.Draw(Color.Blue, 2, path));
            image.SaveAsPng(stream);
        }
    }
}
