namespace Program
{
    class Node
    {
        public int _id;
        public int _x;
        public int _y;

        public Node(int id, int x, int y)
        {
            _id = id;
            _x = x;
            _y = y;
        }
    }

    class CaveSolver
    {
        public Node? _startNode;
        public Node? _endNode;
        public string _fileName;
        public double? _distance;

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

            _startNode = new Node(1, allValues[1], allValues[2]);
            _endNode = new Node(totalAmount, allValues[totalAmount * 2 - 1], allValues[totalAmount * 2]);

            var nodesToChooseList = new PriorityQueue<Node, int>();
            var closedNodesList = new List<int> {};
            var accessDictionary = CreateDictionary(totalAmount, allValues.TakeLast(totalAmount * totalAmount).ToList());

            nodesToChooseList.Enqueue(_startNode, 0);

            // Remove idead nodes
            var keysToRemove = accessDictionary.Where(kv => kv.Value.Count == 0 && kv.Key != totalAmount - 1).Select(kv => kv.Key + 1).ToList();

            while (nodesToChooseList.Count > 0)
            {
                // Pick current node with lowest f
                var currentNode = nodesToChooseList.Dequeue();

                // Add the node to the results
                closedNodesList.Add(currentNode._id);

                // If we found the end return
                if (currentNode._id == _endNode._id)
                {
                    PrintPath(closedNodesList);
                    return;
                }

                // Remove nodes from the list
                nodesToChooseList.Clear();

                // Find nodes that can be reached
                foreach(var nodeId in accessDictionary[currentNode._id - 1])
                {
                    if (closedNodesList.Any(node => node == nodeId) || keysToRemove.Any(x => x == nodeId))
                        continue;

                    var x = allValues[nodeId * 2 - 1];
                    var y = allValues[nodeId * 2];
                    var distance = Math.Pow(currentNode._x - x, 2) + Math.Pow(currentNode._y - y, 2);

                    var f = CalculateF(distance, x, y);

                    nodesToChooseList.Enqueue(new Node(nodeId, x, y), f);
                }
            }

            Console.WriteLine("0");
        }

        public static Dictionary<int, List<int>> CreateDictionary(int size, List<int> accesses)
        {
            var dictionary = new Dictionary<int, List<int>>();

            for (int i = 0; i < size; i++)
            {
                dictionary[i] = new List<int>();
                for (int j = 0; j < size; j++)
                {
                    if (accesses[j * size + i] == 1)
                    {
                        dictionary[i].Add(j + 1);
                    }
                }
            }

            return dictionary;
        }

        public void PrintPath(List<int> nodeIds)
        {
            foreach (var nodeId in nodeIds)
            {
                Console.Write(nodeId + " ");
            }
        }

        public int CalculateF(double distance, int x, int y)
        {
            // Maybe use absolute value instead?
            return Convert.ToInt32(distance +
                                   Math.Pow(_endNode._x - x, 2) +
                                   Math.Pow(_endNode._y - y, 2));
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