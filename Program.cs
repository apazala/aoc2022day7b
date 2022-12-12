internal class Program
{
    const int TOTAL_CAPACITY = 70000000;
    const int NEEDED_SIZE = 30000000;
    class DirTree
    {
        public int DirectSize { get; set; }
        public string Name { get; }
        Dictionary<string, DirTree> childDirectories;

        public DirTree(string name)
        {
            Name = name;
            DirectSize = 0;
            childDirectories = new Dictionary<string, DirTree>();
        }

        public void AddChild(string name)
        {
            childDirectories[name] = new DirTree(name);
        }

        public DirTree GetChild(string name)
        {
            return childDirectories[name];
        }

        public int TotalSize(List<int> sizesList)
        {
            int totalS = DirectSize;
            foreach(var entry in childDirectories)
            {
                totalS+= entry.Value.TotalSize(sizesList);
            }

            sizesList.Add(totalS);

            return totalS;
        }
    }

    private static void Main(string[] args)
    {
        List<DirTree> path = new List<DirTree>();
        DirTree currDir = new DirTree("/");
        path.Add(currDir);

        string[] readText = File.ReadAllLines(@"input.txt");
        for (int lineNumber = 0; lineNumber < readText.Length; lineNumber++)
        {
            string[] tokens = readText[lineNumber].Split(' ');

            //Assume no errors: first token always a "$" at this point
            if (tokens[1][0] == 'c') //cd
            {
                ChangeDir(path, tokens[2]);
                
            }
            else  //ls
            {
                currDir = path[path.Count-1];
                int directSize = 0;
                while (lineNumber + 1 < readText.Length && readText[lineNumber + 1][0] != '$')
                {
                    lineNumber++;
                    tokens = readText[lineNumber].Split(' ');
                    if(tokens[0][0] == 'd')//dir
                    {
                        currDir.AddChild(tokens[1]);
                    }else{
                        directSize += int.Parse(tokens[0]);
                    }
                }
                currDir.DirectSize = directSize;
            }

        }

        List<int> sizesList = new List<int>();
        int rootSize = path[0].TotalSize(sizesList);

        int target = NEEDED_SIZE - (TOTAL_CAPACITY - rootSize);

        sizesList.Sort();

        int ind = sizesList.BinarySearch(target);
        if(ind<0) ind = ~ind;

        Console.WriteLine(sizesList[ind]);

    }


    private static void ChangeDir(List<DirTree> path, string dirName)
    {
        switch (dirName)
        {
            case "..":
                path.RemoveAt(path.Count - 1);
                break;
            case "/":
                path.RemoveRange(1, path.Count - 1);
                break;
            default:
                DirTree child = path[path.Count - 1].GetChild(dirName);
                path.Add(child);
                break;
        }
    }
}