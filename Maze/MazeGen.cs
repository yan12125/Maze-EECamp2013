using System;
using System.Linq;
using System.Collections.Generic;

public class MazeGen
{
    protected enum DIR { N, E, W, S, none };
    
    protected int height;
    protected int width;
    protected int[,] maze;
    protected Dictionary<DIR, int> dx = new Dictionary<DIR, int>() { 
        { DIR.N, 0 }, 
        { DIR.E, 1 }, 
        { DIR.W, -1 }, 
        { DIR.S, 0 }
    };
    protected Dictionary<DIR, int> dy = new Dictionary<DIR, int>() {
        { DIR.N, -1 }, 
        { DIR.E, 0 }, 
        { DIR.W, 0 }, 
        { DIR.S, 1 }
    };
    
    public MazeGen(int w, int h)
    {
        width = w;
        height = h;
        maze = new int[width, height];
    }
    
    public int[,] generate(int initX, int initY)
    {
        for(int i = 0;i < width;i++)
        {
            for(int j = 0;j < height;j++)
            {
                maze[i, j] = 1; // walls everywhere
            }
        }
        maze[initX, initY] = 0; // no wall at the initial location
        int x = initX, y = initY;
        Stack<Tuple<int, int>> path = new Stack<Tuple<int, int>>();
        path.Push(new Tuple<int, int>(x, y));
        bool outerRun = true;
        while (outerRun)
        {
            DIR dir = direction(x, y);
            if (dir != DIR.none)
            {
                x += dx[dir];
                y += dy[dir];
                maze[x, y] = 0;
                path.Push(new Tuple<int, int>(x, y));
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
                        path.Push(new Tuple<int, int>(x, y));
                        x += dx[dir2];
                        y += dy[dir2];
                        maze[x, y] = 0;
                        path.Push(new Tuple<int, int>(x, y));
                    }
                }
            }
        }
        return maze;
    }
    
    protected DIR direction(int x, int y)
    {
        DIR[] dirs = { DIR.N, DIR.E, DIR.W, DIR.S };
        IEnumerable<DIR> _dirs = dirs.OrderBy(a => Guid.NewGuid());
        foreach(DIR dir in _dirs)
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
}
