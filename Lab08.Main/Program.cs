namespace Lab08;
class Program
{
    public static void Main()
    {
        new Tests();

        UserInterface _interface = new();

        Console.Write("What type of world would you like to play? (obstacle, monster) ");
        string type = _interface.WorldType(Console.ReadLine());
        GameLoop game = new();
        
        if (type == "obstacle") game.ObstacleMap();
        else if (type == "monster") game.MonsterFighter();
    }
}