using static System.Console;

namespace DNDBattler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<CommonCharacter> battlers = new List<CommonCharacter>();
            List<CommonCharacter> graveyard = new List<CommonCharacter>();
            List<CommonCharacter> deserters = new List<CommonCharacter>();

            battlers.Add(new Zombie("Zombie 1"));
            battlers.Add(new Zombie("Zombie 2"));
            battlers.Add(new Zombie("Zombie 3"));
            battlers.Add(new Zombie("Zombie 4"));
            battlers.Add(new Zombie("Rob"));
            battlers.Add(new Paladin("Sir Smite", 16, 18, 11, 18, 11, 14, 15, 10));
            battlers.Add(new OgreZombie("Shrek"));
            battlers.Add(new Eyedrake("Beneulem"));
            battlers.Add(new BeholderZombie("Dead Rival"));
            RollInitiative(battlers);
            foreach (CommonCharacter c in battlers)
            {
                WriteLine(c); WriteLine("");
            }

            WriteLine("Welcome to the battler! Whenever the screen is paused, that means "
                + "that you must press any key to continue.\nAbove you can see everyone "
                + "that will be fighting.\nPress a key to get started!");
            ReadKey();
            //For some reason both Clear() and WriteLine("\x1b[3j") is needed to actually clear the console.
            Clear();
            WriteLine("\x1b[3J");

            int turn = 0; int round = 1;
            while (battlers.Count > 1)
            {
                if (turn == battlers.Count)
                {
                    turn = 0; round++;
                }
                WriteLine($"------------Round {round}!------------\n");
                CommonCharacter curr = battlers[turn];
                if (battlers[turn].Is(Conditions.Paralyzed))
                {
                    WriteLine($"{battlers[turn].Name} is paralyzed and can't do anything!.");
                    WriteLine(curr.EndTurn());
                } else if (battlers[turn].Is(Conditions.Incapacitated))
                {
                    WriteLine($"{battlers[turn].Name} is incapacitated and can't do anything!.");
                    WriteLine(curr.EndTurn());
                }
                else
                {
                    if (curr.ReadyToFlee)
                    {
                        WriteLine($"{curr.Name}'s fighting spirit relinquishes, and so "
                            + "they flee to live another day.");
                        deserters.Add(curr);
                        //Turn is decreased to avoid accidentally skipping a turn.
                        battlers.RemoveAt(turn); turn--;
                    } else
                    {
                        CommonCharacter[] targets = curr.CreateTargets(battlers);
                        string printLine = curr.Attack(targets);
                        WriteLine(printLine);
                        WriteLine(curr.EndTurn());
                        turn = CheckDeaths(battlers, graveyard, turn);
                        //Ensures that initiative doesn't jump around
                        //due to combatants being removed.
                        int index = battlers.IndexOf(curr);
                        turn = index;
                    }

                }
                PrintBasic(battlers);
                if(graveyard.Count > 0)
                {
                    WriteLine("\n  The Graveyard");
                    foreach(CommonCharacter d in graveyard)
                    {
                        WriteLine($"     {d.Name}");
                    }
                }
                if(deserters.Count > 0)
                {
                    WriteLine(" \nThose who'll live at least an extra day:");
                    foreach(CommonCharacter f in deserters)
                    {
                        WriteLine($"     {f.Name}");
                    }
                }

                ReadKey();
                Clear();
                WriteLine("\x1b[3J");
                turn++;
            }

            if(battlers.Count == 0)
            {
                WriteLine($"Are you kidding me? {round} rounds of combat just ".PadLeft(40)
                    + "for there to be NO WINNER!?");
                WriteLine("Just Go home.".PadLeft(40).PadRight(20));
            } else
            {
                WriteLine("AND WE HAVE A WINNER!!".PadLeft(55) + $" (After {round} rounds)");
                WriteLine("GIVE IT UP FOR:".PadLeft(42));
                WriteLine(battlers[0]);
            }
        }

        /// <summary>
        /// Loops through the list containing combatants, and transfers them to the graveyard if 
        /// they are dead (using the IsDead) property. Also adjusts turns to account for removals
        /// </summary>
        /// <param name="list"></param>
        /// <param name="graveyard"></param>
        /// <returns>The updated turn count, accounting for characters removed from list.</returns>
        static int CheckDeaths(List<CommonCharacter> list, List<CommonCharacter> graveyard, 
            int turns)
        {
            int updatedTurns = turns;
            for(int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].IsDead)
                {
                    graveyard.Add(list[i]);
                    list.RemoveAt(i);
                    if(i <= updatedTurns)
                    {
                        updatedTurns--;
                    }
                }
            }
            return updatedTurns;
        }
        /// <summary>
        /// Prints and formats the remaining list of combatants
        /// </summary>
        /// <param name="list"></param>
        static void PrintBasic(List<CommonCharacter> list)
        {
            WriteLine("  Combatant list!:");
            foreach(CommonCharacter c in list)
            {
                WriteLine($"     {c.BasicInfo()}");
            }
        }
        /// <summary>
        /// Uses insertion sort to reassign the locations of each creature based on their 
        /// initiative roll.
        /// </summary>
        /// <param name="list"></param>
        static void RollInitiative(List<CommonCharacter> list)
        {
            for(int i = 1; i < list.Count; i++)
            {
                CommonCharacter current = list[i];
                int currentDexMod = current.GetMod("dexterity");
                //This will be used to iterate backwards.
                int index = i;
                //Current object gets pushed back if one of the below conditions are met:
                //1. A previous creature has a higher initiative roll
                //2. They have an equal roll but previous creature has a higher dex modifier
                while(index > 0 && (list[index - 1].Initiative < current.Initiative 
                    || 
                    (list[index - 1].GetMod("dexterity") < currentDexMod 
                    && list[index - 1].Initiative == current.Initiative)))
                {
                    list[index] = list[index - 1];
                    index--;
                }
                list[index] = current;
            }
        }
    }
}
