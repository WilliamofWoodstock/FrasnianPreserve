namespace MonopolySimulation
{
    internal static class MonopolySimulator
    {
        static Random rnd = new Random();
        static string[] propertyNames = {
        "Go",
        "Mediterranean Avenue",
        "Community Chest (1)",
        "Baltic Avenue",
        "Income Tax",
        "Reading Railroad",
        "Oriental Avenue",
        "Chance (1)",
        "Vermont Avenue",
        "Connecticut Avenue",
        "In Jail",
        "St. Charles Place",
        "Electric Company",
        "States Avenue",
        "Virginia Avenue",
        "Pennsylvania Railroad",
        "St. James Place",
        "Community Chest (2)",
        "Tennessee Avenue",
        "New York Avenue",
        "Free Parking",
        "Kentucky Avenue",
        "Chance (2)",
        "Indiana Avenue",
        "Illinois Avenue",
        "B&O Railroad",
        "Atlantic Avenue",
        "Ventnor Avenue",
        "Water Works",
        "Marvin Gardens",
        "Go To Jail",
        "Pacific Avenue",
        "North Carolina Avenue",
        "Community Chest (3)",
        "Pennsylvania Avenue",
        "Short Line Railroad",
        "Chance (3)",
        "Park Place",
        "Luxury Tax",
        "Boardwalk"
        };

        /// <summary>
        /// Simulates the rolling of a six-sided dice and returns the value
        /// </summary>
        /// <returns>An integer between 1-6</returns>
        internal static int RollDice()
        {
            return rnd.Next(1, 7);
        }

        /// <summary>
        /// Simulates a number of players traversing a whole monopoly board a certain number of times, creating an array
        /// storing the percentage of times each tile was moved on by a player. Can adjust for certain Monopoly rules.
        /// Does not account for chance or community chest cards.
        /// </summary>
        /// <param name="players"># of players involved. Each player increases traversals done by a factor of 
        /// traversalsPerPlayer</param>
        /// <param name="traversalsPerPlayer"># of times each player must travel around the board. Each additional
        /// traversal required increases traversals done by a factor of players</param>
        /// <param name="goToJail">Indicates whether tile 30 (Go to Jail) will force a player to go to tile 10 (Jail).
        /// Getting sent to jail in this way counts as completing one traversal, but it counts as moving to tile 30,
        /// NOT tile 10.</param>
        /// <param name="speeding">Indicates whether rolling 3 'doubles' (both dices same value) in a row immediately 
        /// sends the player to jail. Going to jail this way does NOT count as completing one traversal, but 
        /// it does count as moving to tile 10 (since you go to jail before you move).</param>
        /// <returns></returns>
        internal static double[] Simulate(int players, int traversalsPerPlayer, bool goToJail, bool speeding)
        {
            int moveNum = 0; //keeps track of the grand total of moves made this game
            double[] results = new double[40];
            int doublesInARow = 0;
            for(int i = 1; i<= players; i++)
            {
                int traversalNum = 0;
                int pos = 1;
                //Console.WriteLine($"This is Player {i}");
                while(traversalNum < traversalsPerPlayer)
                {
                    int diceRoll1 = RollDice(); int diceRoll2 = RollDice();
                    if (diceRoll1 == diceRoll2 && speeding)
                    {
                        doublesInARow++;
                        //Console.WriteLine("Double!");
                        if(doublesInARow >= 3)
                        {
                            pos = 10;
                            //Console.WriteLine($"Player {i} caught speeding.");
                        }
                    }
                    else
                    {
                        doublesInARow = 0;
                    }

                    pos += diceRoll1 + diceRoll2;
                    //Console.WriteLine(pos);
                    if (pos > 40)
                    {
                        pos -= 40;
                        traversalNum++;
                        //Console.WriteLine($"One traversal done! Only {traversalsPerPlayer - traversalNum} more to go!");
                    }

                    results[pos - 1] += 1.0;
                    if(goToJail && pos == 30)
                    {
                        pos = 10;
                        traversalNum++;
                        //Console.WriteLine("Bad Luck!");
                    }
                    moveNum++;
                    //Console.WriteLine($"That was move {moveNum}");
                }
            }
            /*Previously the array stored the number of times each tile was visited
            so this finally converts each value into a percentage.*/
            for(int i = 0; i < results.Length; i++)
            {
                results[i] = (results[i] / moveNum) * 100;
            }
            return results;
        }

        /// <summary>
        /// Prints data generate by the Simulate() method. Formatted such that tiles 1-20 and their percentages 
        /// are printed on one column, and tiles 21-40 and their percentages are printed on another column 
        /// right next to the first one. This also prints the lowest %, highest %, adn the difference between the two.
        /// This only prints to the hundreth decimal via ToString("#.00")
        /// </summary>
        /// <param name="visitPercents">Double array containing all of the Simulate() percentages</param>
        internal static void PrintResults(double[] visitPercents)
        {
            int place1End = 30; //The character index that all col1 values must end at
            int column2Indent = 25; //Space between the end of place1 and the ':' of place2
            int jump = visitPercents.Length / 2;

            for(int i = 0; i < jump; i++)
            {
                string place1 = $"  {propertyNames[i]} : {visitPercents[i].ToString("#.00")}";
                string place2 = $"{propertyNames[i + jump]} : {visitPercents[i + jump].ToString("#.00")}";
                int divider2 = place2.IndexOf(":") + 1; //Character position of ':' within place2

                string printLine = place1.PadLeft(place1End);
                //Measures how many spaces are needed to achieve the desired indent and appends spaces accordingly
                int spacesNeeded = column2Indent - divider2;
                for(int j = 1; j <= spacesNeeded; j++)
                {
                    printLine += " ";
                }
                printLine += place2;

                Console.WriteLine(printLine);
            }

            Console.WriteLine($"Lowest : {SmallestPercentage(visitPercents).ToString("#.00")}".PadLeft(40));
            Console.WriteLine($"Highest : {LargestPercentage(visitPercents).ToString("#.00")}".PadLeft(40));
            Console.WriteLine($"Difference : {(LargestPercentage(visitPercents) - SmallestPercentage(visitPercents)).ToString("#.00")}".PadLeft(40));
        }

        /// <summary>
        /// A simple search through a double array to find the greatest value.
        /// </summary>
        /// <param name="results">An array of doubles to be searched.</param>
        /// <returns>The greatest value contained within the double[]</returns>
        internal static double LargestPercentage(double[] results)
        {
            double largest = results[0];
            foreach (double num in results)
            {
                if (num > largest)
                {
                    largest = num;
                }
            }

            return largest;
        }

        /// <summary>
        /// A simple search through an array of doubles to find the smallest value. 
        /// </summary>
        /// <param name="results">An array of doubles to be searched.e</param>
        /// <returns>The lowest value contained within the double[]</returns>
        internal static double SmallestPercentage(double[] results)
        {
            double smallest = results[0];
            foreach (double num in results)
            {
                if (num < smallest)
                {
                    smallest = num;
                }
            }

            return smallest;
        }
    }
}
