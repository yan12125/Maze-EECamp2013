using System;
using PointXY = System.Tuple<int, int>;

public class MazeGenWithProps : MazeGen
{
    protected Random gen = new Random();
    // l => larger; s => smaller
    protected enum Props { none = 0, lView = 2, sView, lSpeed, sSpeed, FakeWall };
    
    public MazeGenWithProps(int w, int h, int hardness)
        : base(w, h, hardness)
	{
	}

    public override int[,] generate(int initX = 0, int initY = 0, int finalX = -1, int finalY = -1)
    {
        base.generate(initX, initY, finalX, finalY);
        putFakeWall();
        addPropToGrids(70, Props.lSpeed, Props.sSpeed);
        addPropToGrids(70, Props.lView, Props.sView);
        return maze;
    }

    protected void putFakeWall()
    {
        // fake walls shouldn't be at the first and the last grid
        // and I put it at the second half of the way
        // Note: solution is a Stack
        int start = (int)Math.Round(solution.Length / 6.0);
        int end = (int)Math.Round(5 * solution.Length / 6.0);
        int nFakeWallsOnRoad = gen.Next(6);
        for (int i = 0; i < nFakeWallsOnRoad; )
        {
            int index = gen.Next(start, end);
            int nextX = solution[index].Item1;
            int nextY = solution[index].Item2;
            if(maze[nextX, nextY] == 0)
            {
                maze[nextX, nextY] = (int)Props.FakeWall;
                i++;
            }
        }
        addPropToGrids(80, Props.FakeWall, Props.none, true);
    }

    protected void addPropToGrids(int probability, Props prop1, Props prop2, bool preventSolution = false)
    {
        int nProps = 0;
        int failedTry = 0;
        while (true)
        {
            int x = gen.Next(width);
            int y = gen.Next(height);
            PointXY pt = new PointXY(x, y);
            int idx = Array.IndexOf<PointXY>(solution, pt);
            if (maze[x, y] == 0 && (!preventSolution || idx == -1))
            {
                setPoint(pt, (gen.Next(100) < probability) ? prop1 : prop2);
                nProps++;
            }
            else
            {
                failedTry++;
            }
            if (nProps >= 10 || failedTry >= width * height)
            {
                break;
            }
        }
    }

    protected bool setPoint(PointXY pt, Props prop, DIR dir = DIR.none)
    {
        int nextX = pt.Item1 + dx[dir];
        int nextY = pt.Item2 + dy[dir];
        if (maze[nextX, nextY] != 0)
        {
            return false;
        }
        maze[nextX, nextY] = (int)prop;
        return true;
    }
}
