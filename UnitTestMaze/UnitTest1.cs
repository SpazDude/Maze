using MazeLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace UnitTestMaze
{
    [TestClass]
    public class UnitTest1
    {

        public void PrintMaze(MazeBuilder m)
        {
            for (int x = 0; x < m.x; x++)
            {
                for (int y = 0; y < m.y; y++)
                {
                    char c = (char)(m.nodes[x, y].group + (int)'a');
                    Console.Write(c);
                    if (y < m.y - 1)
                    {
                        var n1 = m.nodes[x, y];
                        var n2 = m.nodes[x, y + 1];
                        Console.Write(m.HasEdge(n1, n2) ? '-' : ' ');
                    }
                }
                Console.WriteLine();
                for (int y = 0; y < m.y; y++)
                {
                    if (x < m.x - 1)
                    {
                        var n1 = m.nodes[x, y];
                        var n2 = m.nodes[x + 1, y];
                        Console.Write(m.HasEdge(n1, n2) ? "| " : "  ");
                    }
                }
                Console.WriteLine();
            }
        }


        [TestMethod]
        public void TestMethod1()
        {
            var m = new MazeBuilder(2, 2);
            m.AddEdge(m.nodes[0, 0], m.nodes[0, 1]);
            m.AddEdge(m.nodes[0, 1], m.nodes[1, 1]);
            PrintMaze(m);

        }

        [TestMethod]
        public void TestMethod2()
        {
            var m = new MazeBuilder(16,16);
            m.CreateMazeWithOnePath();
            PrintMaze(m);
        }

        [TestMethod]
        public void TestAreAdjacentEdges()
        {
            var m = new MazeBuilder(2, 2);
            Assert.IsTrue(m.AreAdjacentEdges(m.nodes[0, 0], m.nodes[0, 1]));
            Assert.IsTrue(m.AreAdjacentEdges(m.nodes[0, 0], m.nodes[1, 0]));
            Assert.IsFalse(m.AreAdjacentEdges(m.nodes[0, 0], m.nodes[1, 1]));
        }

        [TestMethod]
        public void TestHasEdge1()
        {
            var m = new MazeBuilder(2, 2);
            var n1 = m.nodes[0, 0];
            var n2 = m.nodes[0, 1];
            var n3 = m.nodes[1, 0];
            m.AddEdge(n1, n2);
            Assert.IsTrue(m.HasEdge(n1, n2));
            Assert.IsFalse(m.HasEdge(n1, n3));
            Assert.IsTrue(n1.right); Assert.IsTrue(n1.left); Assert.IsTrue(n1.top); Assert.IsFalse(n1.bottom);
            Assert.IsTrue(n2.left); Assert.IsTrue(n2.left); Assert.IsFalse(n2.top); Assert.IsTrue(n2.bottom);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestHasEdge2()
        {
            var m = new MazeBuilder(2, 2);
            var n1 = m.nodes[0, 0];
            var n3 = m.nodes[1, 1];
            m.AddEdge(n1, n3);
        }

        [TestMethod]
        public void TestCreateImage()
        {
            const string imageName = "./testImg.png";
            var m = new MazeBuilder(60, 40);
            m.CreateMazeWithOnePath();
            if (File.Exists(imageName)) File.Delete(imageName);
            var stream = new FileStream(imageName, FileMode.Create);
            m.CreateImage(stream);
            stream.Close();
        }
    }
}
