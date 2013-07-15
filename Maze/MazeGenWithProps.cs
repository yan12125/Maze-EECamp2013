using System;
using PointXY = System.Tuple<int, int>;

public class MazeGenWithProps : MazeGen
{
    protected Random gen = new Random();
    // l => larger; s => smaller
    protected enum Props { lView = 2, sView, lSpeed, sSpeed, FakeWall };
    
    public MazeGenWithProps(int w, int h, int hardness)
        : base(w, h, hardness)
	{
	}

    public override int[,] generate(int initX = 0, int initY = 0, int finalX = -1, int finalY = -1)
    {
        base.generate(initX, initY, finalX, finalY);
        putFakeWall();
        putSpeedTools();
        putViewTools();
        return maze;
    }

    protected void putFakeWall()
    {
        // fake walls shouldn't be at the first and the last grid
        // and I put it at the second half of the way
        // Note: solution is a Stack
        int start = (int)Math.Round(solution.Length / 4.0);
        int end = (int)Math.Round(3 * solution.Length / 4.0);
        int index = gen.Next(start, end);
        maze[solution[index].Item1, solution[index].Item2] = (int)Props.FakeWall;
    }

    protected void putSpeedTools()
    {
        for (int i = 0; i < solution.Length; i++)
        {
            DIR dir = checkDirection(solution[i]);
            if (dir != DIR.none && gen.Next(100) < 10)
            {
                setPoint(solution[i], Props.lSpeed, dir);
            }
            if (gen.Next(100) < 5)
            {
                setPoint(solution[i], Props.sSpeed);
            }
        }
        addPropToNormalGrids(70, Props.lSpeed, Props.sSpeed);
    }

    protected void putViewTools()
    {
        for (int i = 0; i < solution.Length; i++)
        {
            DIR dir = checkDirection(solution[i]);
            if(dir != DIR.none)
            {
                int d = gen.Next(100);
                if (d < 10)
                {
                    setPoint(solution[i], Props.lView, dir);
                }
                else if (d < 30)
                {
                    setPoint(solution[i], Props.sView, dir);
                }
            }
        }
        addPropToNormalGrids(70, Props.lView, Props.sView);
    }

    protected void addPropToNormalGrids(int probability, Props prop1, Props prop2)
    {
        int nProps = 0;
        int failedTry = 0;
        while (true)
        {
            int x = gen.Next(width);
            int y = gen.Next(height);
            PointXY pt = new PointXY(x, y);
            // http://www.dotnetperls.com/array-indexof
            if (maze[x, y] == 0 && Array.IndexOf<PointXY>(solution, pt) == -1)
            {
                setPoint(pt, (gen.Next(100) < probability) ? prop1 : prop2);
                nProps++;
            }
            else
            {
                failedTry++;
            }
            if (nProps >= 20 || failedTry >= width * height)
            {
                break;
            }
        }
    }

    protected DIR checkDirection(int x, int y)
    {
        if (!checkRange(x, y))
        {
            return DIR.none;
        }
        foreach (DIR dir in getRandomDirections())
        {
            int nextX = x + dx[dir];
            int nextY = y + dy[dir];
            PointXY pt = new PointXY(nextX, nextY);
            if (checkRange(nextX, nextY) &&
                Array.IndexOf<PointXY>(solution, pt) != -1 &&
                maze[nextX, nextY] == 0)
            {
                return dir;
            }
        }
        return DIR.none;
    }

    protected DIR checkDirection(PointXY pt)
    {
        return checkDirection(pt.Item1, pt.Item2);
    }

    protected bool setPoint(PointXY pt, Props prop, DIR dir = DIR.none)
    {
        return setPoint(pt.Item1, pt.Item2, prop, dir);
    }

    protected bool setPoint(int x, int y, Props prop, DIR dir = DIR.none)
    {
        int nextX = x + dx[dir];
        int nextY = y + dy[dir];
        if (maze[nextX, nextY] != 0)
        {
            return false;
        }
        maze[nextX, nextY] = (int)prop;
        return true;
    }
}
