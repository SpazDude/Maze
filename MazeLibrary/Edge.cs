namespace MazeLibrary
{
    public class Edge
    {
        public Node[] nodes = new Node[2];

        public Edge(Node n1, Node N2)
        {
            nodes[0] = n1;
            nodes[1] = N2;
        }
    }
}
