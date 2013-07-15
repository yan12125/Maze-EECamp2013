/*
 * Using Depth First Search
 * References: 
 * http://en.wikipedia.org/wiki/Maze_generation_algorithm
 * http://rosettacode.org/wiki/Maze_generation Java version
 */
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using PointXY = System.Tuple<int, int>;

public class MazeGen
{
    protected enum DIR { N, E, W, S, none };
    
    protected int height;
    protected int width;
    protected int finalX;
    protected int finalY;
    protected int hardness;
    protected int[,] maze;
    protected Stack<PointXY> path = new Stack<PointXY>();
    protected PointXY[] solution;
    protected Dictionary<DIR, int> dx = new Dictionary<DIR, int>() { 
        { DIR.N, 0 }, 
        { DIR.E, 1 }, 
        { DIR.W, -1 }, 
        { DIR.S, 0 }, 
        { DIR.none, 0 }
    };
    protected Dictionary<DIR, int> dy = new Dictionary<DIR, int>() {
        { DIR.N, -1 }, 
        { DIR.E, 0 }, 
        { DIR.W, 0 }, 
        { DIR.S, 1 }, 
        { DIR.none, 0 }
    };
    protected DIR[] dirs = { DIR.N, DIR.E, DIR.W, DIR.S };
    
    public MazeGen(int w, int h, int minimalHardness = 0)
    {
        width = w;
        height = h;
        hardness = minimalHardness;
        maze = new int[width, height];
    }

    public virtual int[,] generate(int initX = 0, int initY = 0, int _finalX = -1, int _finalY = -1)
    {
        finalX = (_finalX == -1) ? (width - 1) : _finalX;
        finalY = (_finalY == -1) ? (height - 1) : _finalY;
        do
        {
            generate_impl(initX, initY);
        } while (maze[finalX, finalY] != 0 || solution.Length <= hardness);
        maze[finalX, finalY] = 7;
        return maze;
    }
    
    protected void generate_impl(int initX, int initY)
    {
        for(int i = 0;i < width;i++)
        {
            for(int j = 0;j < height;j++)
            {
                maze[i, j] = 1; // walls everywhere
            }
        }
        maze[initX, initY] = 0; // no wall at the initial location
        int x = initX;
        int y = initY;
        pushPointToPath(x, y);
        bool outerRun = true;
        while (outerRun)
        {
            DIR dir = direction(x, y);
            if (dir != DIR.none)
            {
                x += dx[dir];
                y += dy[dir];
                maze[x, y] = 0;
                pushPointToPath(x, y);
            }
            else
            {
                // no need to check current point again
                path.Pop();
                while (true)
                {
                    Tuple<int, int> lastPoint = path.Pop();
                    x = lastPoint.Item1;
                    y = lastPoint.Item2;
                    DIR dir2 = direction(x, y);
                    if (dir2 == DIR.none)
                    {
                        if (path.Count == 0) // the entire map is filled
                        {
                            outerRun = false;
                            break;
                        }
                        continue;
                    }
                    else
                    {
                        // the original point was just popped 
                        pushPointToPath(x, y);
                        x += dx[dir2];
                        y += dy[dir2];
                        maze[x, y] = 0;
                        pushPointToPath(x, y);
                    }
                }
            }
        }
    }

    protected void pushPointToPath(int x, int y)
    {
        path.Push(new Tuple<int, int>(x, y));
        if (x == finalX && y == finalY)
        {
            solution = new Tuple<int, int>[path.Count];
            path.CopyTo(solution, 0);
        }
    }
    
    protected DIR direction(int x, int y)
    {
        foreach(DIR dir in getRandomDirections())
        {
            int nextX = x + dx[dir];
            int nextY = y + dy[dir];
            if(checkRange(nextX, nextY) && maze[nextX, nextY] != 0)
            {
                bool outerContinue = false;
                foreach (DIR dir2 in dirs)
                {
                    int nextX2 = nextX + dx[dir2];
                    int nextY2 = nextY + dy[dir2];
                    if (checkRange(nextX2, nextY2) && maze[nextX2, nextY2] == 0)
                    {
                        if (!(nextX2 == x && nextY2 == y))
                        {
                            outerContinue = true;
                            break;
                        }
                    }
                }
                if (outerContinue)
                {
                    continue;
                }
                return dir;
            }
        }
        return DIR.none;
    }
    
    protected bool checkRange(int x, int y)
    {
        return (0 <= x && x < width) && (0 <= y && y < height);
    }

    protected IEnumerable<DIR> getRandomDirections()
    {
        // Reference: http://stackoverflow.com/questions/273313
        return dirs.OrderBy(a => Guid.NewGuid());
    }

    public void WriteToFile(string filename)
    {
        string[] output = new string[height];
        for(int i = 0;i < height;i++)
        {
            int[] tempArr = new int[width];
            for (int j = 0; j < width; j++)
            {
                tempArr[j] = maze[j, i];
            }
            output[i] = "{" + String.Join(", ", tempArr) + "}";
        }
        string outputStr = String.Join(", \n", output);
        outputStr += "\n// " + solution.Length + " steps";
        File.WriteAllText(filename, outputStr);
    }

    public void WriteToFile2(string filename, string mapName)
    {
        string output = "EECamp2013 Advanced Program Project -- Maze\nMapEditor by yan12125.\n-- Map Name\n" + mapName + "\n-- Map Details\n";
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                output += maze[j, i].ToString();
            }
        }
        File.WriteAllText(filename, output);
    }
}
