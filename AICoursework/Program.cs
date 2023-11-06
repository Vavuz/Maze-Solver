namespace Program
{
    class Node
    {
        public int _id;
        public int _x;
        public int _y;
        public double _g { get; set;}
        public double _f {  get; set; }
        public int _parent { get; set;}

        // should it store f too
        public Node(int id, int x, int y, double g = double.MaxValue, double f = double.MaxValue, int parent = 0)
        {
            _id = id;
            _x = x;
            _y = y;
            _g = g;
            _f = f;
            _parent = parent;
        }
    }

    class CaveSolver
    {
        // could add endnodex and endnodey and remove x and y from the node class
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

            _endNode = new Node(totalAmount, allValues[totalAmount * 2 - 1], allValues[totalAmount * 2]);
            _startNode = new Node(1, allValues[1], allValues[2], 0, CalculateF(0, allValues[1], allValues[2]));

            var nodesToChooseList = new List<Node>();
            var closedNodesList = new List<Node>();
            var accessDictionary = CreateDictionary(totalAmount, allValues.TakeLast(totalAmount * totalAmount).ToList());

            nodesToChooseList.Add(_startNode);

            while (nodesToChooseList.Count > 0)
            {
                // Pick current node with lowest f
                var currentNode = nodesToChooseList.OrderBy(node => node._f).First();

                // Add current node to the closed list
                closedNodesList.Add(currentNode);

                // If we found the end return
                if (currentNode._id == _endNode._id)
                {
                    PrintPath(closedNodesList);
                    return;
                }

                // Loop through nodes that can be reached
                foreach(var nodeId in accessDictionary[currentNode._id - 1])
                {
                    // If it is already in the closed list or if it is a dead node, ignore
                    if (closedNodesList.Any(node => node._id == nodeId))
                        continue;

                    var x = allValues[nodeId * 2 - 1];
                    var y = allValues[nodeId * 2];
                    var distance = Math.Sqrt(Math.Pow(currentNode._x - x, 2) + Math.Pow(currentNode._y - y, 2));
                    var g = currentNode._g + distance;
                    var f = CalculateF(g, x, y);

                    // If it is already in the visited list and has a better g, update it, otherwise add it
                    var openNode = nodesToChooseList.FirstOrDefault(node => node._id == nodeId);
                    if (openNode == null)
                    {
                        nodesToChooseList.Add(new Node(nodeId, x, y, g, f, currentNode._id));
                    }
                    else if (g < openNode._g)
                    {
                        openNode._g = g;

                        if (f < openNode._f)
                            openNode._f = f;

                        openNode._parent = currentNode._id;
                        nodesToChooseList[nodesToChooseList.FindIndex(node => node._id == openNode._id)] = openNode;
                    }
                }

                // Remove current node from open list
                nodesToChooseList.Remove(currentNode);
            }

            Console.WriteLine("0");
        }

        public static Dictionary<int, List<int>> CreateDictionary(int size, List<int> accesses)
        {
            var dictionary = new Dictionary<int, List<int>>();

            // more meaningful names
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

        public static void PrintPath(List<Node> nodeList)
        {
            var currentIndex = nodeList.Count - 1;
            var output = nodeList[currentIndex]._id + " ";

            while (currentIndex > 1)
            {
                currentIndex = FindParentIndex(nodeList, currentIndex);
                output = nodeList[currentIndex]._id + " " + output;
            }
            
            Console.WriteLine(output);
        }

        public static int FindParentIndex(List<Node> nodeList, int currentIndex)
        {
            // more meaningful names
            int parentId = nodeList[currentIndex]._parent;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i]._id == parentId)
                    return i;
            }

            return 0;
        }

        public int CalculateF(double g, int x, int y)
        {
            return Convert.ToInt32(g + Math.Sqrt(Math.Pow(_endNode._x - x, 2) + Math.Pow(_endNode._y - y, 2)));
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