namespace Program
{
    class Node
    {
        public int _id;
        public int _x;
        public int _y;
        public double _f;

        public Node(int id, int x, int y, double f)
        {
            _id = id;
            _x = x;
            _y = y;
            _f = f;
        }
    }

    class CaveSolver
    {
        public Node _startNode;
        public Node _endNode;
        public int _distance = 0;
        public string _fileName;

        public CaveSolver(string fileName)
        {
            _fileName = fileName + ".cav";
        }

        public string ReadFile()
        {
            return File.ReadAllText(_fileName);
        }

        public void Solve()
        {
            var mainString = ReadFile();

            var allValues = mainString.Split(',')
                .Select(int.Parse)
                .ToList();

            var totalAmount = allValues[0];

            var matrix = allValues.TakeLast(totalAmount * totalAmount)
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / totalAmount)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            _startNode = new Node(1, allValues[1], allValues[2], 0);
            _endNode = new Node(totalAmount, allValues[totalAmount * 2 - 1], allValues[totalAmount * 2], 0);

            var nodesToChooseList = new PriorityQueue<Node, int>();
            nodesToChooseList.Enqueue(_startNode, 0);
            var closedNodesList = new List<Node> {};

            var previousNode = _startNode;

            while (nodesToChooseList.Count > 0)
            {
                // Pick current node with lowest f
                var currentNode = nodesToChooseList.Dequeue();

                // Add the node to the results
                closedNodesList.Add(currentNode);

                // If we found the end return
                if (currentNode._id == _endNode._id)
                {
                    PrintPath(closedNodesList);
                    return;
                }

                // Remove nodes from the list
                nodesToChooseList.Clear();

                // Find nodes that can be reached
                foreach (var accessibility in matrix)
                {
                    if (accessibility[currentNode._id - 1] == 1)
                    {
                        var index = matrix.IndexOf(accessibility) + 1;
                        nodesToChooseList.Add(new Node(index, allValues[index * 2 - 1], allValues[index * 2], CalculateF(allValues[index * 2], allValues[index * 2 + 1])));
                    }
                }

                // Remove previous node
                nodesToChooseList.RemoveAll(node => node._id == previousNode._id);
                previousNode = currentNode;

                // if it's equal to end node end, otherwise continue
                if (nodesToChooseList.Exists(node => node._id == totalAmount))
                {
                    PrintPath(closedNodesList);
                    return;
                }
            }

            Console.WriteLine("0");
        }

        public void PrintPath(List<Node> nodes)
        {
            var path = "";
            foreach (var node in nodes)
            {
                Console.Write(node._id + " ");
            }
        }

        public double CalculateF(int x, int y)
        {
            return _distance + Math.Pow(_endNode._x - x, 2) +
                               Math.Pow(_endNode._y - y, 2);
        }
    }

    public class MainProgram
    {
        public static void Main(string[] args)
        {
            //var solver = new CaveSolver(args[0]);
            var solver = new CaveSolver("banana");
            solver.Solve();
        }
    }
}