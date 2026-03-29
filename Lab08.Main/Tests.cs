using System.Diagnostics;
using Lab08;

class Tests
{
    public Tests()
    {
        //new Monsters.Monster(new Monsters());

        //UserInterface userInterface = new();
        //foreach (string item in new string[] {"monster", "obstacle", "exit"}) Debug.Assert(UserInterface.WorldType(item) == item);
        Random rand = new();
        
        TestMonsterMap();
    }

    static bool TestMonsterMap()
    {
        GameLoop game = new(true);

        
        return true;
    }

    static bool Test1()
    {
        Dictionary<(int x, int y), string> dict = [];
        Obstacles ob = new();
        //GameLoop.PrintGrid(GameLoop.Ran(6, dict), 6, (0,0));
        return true;
    }
}