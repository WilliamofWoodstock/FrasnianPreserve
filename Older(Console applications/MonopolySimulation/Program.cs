using static System.Console;
using static MonopolySimulation.MonopolySimulator;

namespace MonopolySimulation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int numPlayers = 25;
            int traversals = 100000;
            WriteLine($"Welcome to the Monopoly simulator! \nWe'll be simulating {numPlayers} players"
                + $"\nWho will each be traversing the board {traversals} times for the sake of statistical normality");
            Write("Ready to start? ");
            ReadKey();
            for (int i = 1; i <= 4; i++)
            {
                double[] data = { }; string jail = ""; string speeding = "";
                switch(i)
                {
                    case 1:
                        jail = "ON"; speeding = "ON";
                        data = Simulate(numPlayers, traversals, true, true);
                        break;
                    case 2:
                        jail = "ON"; speeding = "OFF";
                        data = Simulate(numPlayers, traversals, true, false);
                        break;
                    case 3:
                        jail = "OFF"; speeding = "ON";
                        data = Simulate(numPlayers, traversals, false, true);
                        break;
                    case 4:
                        jail = "OFF"; speeding = "OFF";
                        data = Simulate(numPlayers, traversals, false, false);
                        break;
                }
                WriteLine("\n--------------------------------------------------------------------------------------"
                + $"\nSimulating Monopoly  Go to Jail : {jail}  Speeding : {speeding}");
                PrintResults(data);
                Write("Press any key to move on to the next simulation . . . ");
                ReadKey();
            }
        }
    }
}
