using static DNDBattler.Helper;

namespace DNDBattler
{
    /// <summary>
    /// Stores the three states of dice rolling, that being disadvantage (roll twice, take lowest), normal, 
    /// and advantage (roll twice, take highest). Ranges from -1 to 1.
    /// </summary>
    enum DiceState
    {
        Disadvantage = -1,
        Normal,
        Advantage
    }

    /// <summary>
    /// Class that handles all dice-rolling since it's more efficient than giving one dice to each creature.
    /// </summary>
    internal static class DiceRoller
    {
        private static Random rnd = new Random();

        /// <summary>
        /// Simulates rolling a 4-sided die
        /// </summary>
        /// <returns>An integer between 1-4</returns>
        internal static int RollD4() { return rnd.Next(1, 5); }
        /// <summary>
        /// Simulates rolling a number of 4-sided dice depending on param
        /// </summary>
        /// <param name="times"># of dices to be rolled.</param>
        /// <param name="rule">Whether this roll is made with advantage, disadvantage, or normal</param>
        /// <returns>The sum of dices rolled.</returns>
        internal static int RollD4(int times, string rule)
        {
            if(times <= 0)
            {
                throw new ArgumentException("'Times' must be at least 1", $"times: {times}");
            }
            DiceState trueRule = DAN(rule);
            int roll = 0;
            for (int i = 0; i < times; i++)
            {
                int roll1 = RollD4(); int roll2 = RollD4();
                if (trueRule == DiceState.Disadvantage) { roll += Math.Min(roll1, roll2); }
                else if(trueRule == DiceState.Advantage) { roll += Math.Max(roll1, roll2); }
                else { roll += roll1;  }
            }
            return roll;
        }

        /// <summary>
        /// Simulates rolling a 6-sided die
        /// </summary>
        /// <returns>An integer between 1-6</returns>
        internal static int RollD6() { return rnd.Next(1, 7);  }
        /// <summary>
        /// Simulates rolling a number of 6-sided dice depending on param
        /// </summary>
        /// <param name="times"># of dices to be rolled.</param>
        /// <param name="rule">Whether this roll is made with advantage, disadvantage, or normal</param>
        /// <returns>The sum of dices rolled.</returns>
        internal static int RollD6(int times, string rule)
        {
            if (times <= 0)
            {
                throw new ArgumentException("'Times' must be at least 1", $"times: {times}");
            }
            DiceState trueRule = DAN(rule);
            int roll = 0;
            for (int i = 0; i < times; i++)
            {
                int roll1 = RollD6(); int roll2 = RollD6();
                if (trueRule == DiceState.Disadvantage) { roll += Math.Min(roll1, roll2); }
                else if (trueRule == DiceState.Advantage) { roll += Math.Max(roll1, roll2); }
                else { roll += roll1; }
            }
            return roll;
        }

        /// <summary>
        /// Simulates rolling a 8-sided die
        /// </summary>
        /// <returns>An integer between 1-8</returns>
        internal static int RollD8() { return rnd.Next(1, 9); }
        /// <summary>
        /// Simulates rolling a number of 8-sided dice depending on param
        /// </summary>
        /// <param name="times"># of dices to be rolled.</param>
        /// <param name="rule">Whether this roll is made with advantage, disadvantage, or normal</param>
        /// <returns>The sum of dices rolled.</returns>
        internal static int RollD8(int times, string rule)
        {
            if (times <= 0)
            {
                throw new ArgumentException("'Times' must be at least 1", $"times: {times}");
            }
            DiceState trueRule = DAN(rule);
            int roll = 0;
            for (int i = 0; i < times; i++)
            {
                int roll1 = RollD8(); int roll2 = RollD8();
                if (trueRule == DiceState.Disadvantage) { roll += Math.Min(roll1, roll2); }
                else if (trueRule == DiceState.Advantage) { roll += Math.Max(roll1, roll2); }
                else { roll += roll1; }
            }
            return roll;
        }

        /// <summary>
        /// Simulates rolling a 10-sided die
        /// </summary>
        /// <returns>An integer between 1-10</returns>
        internal static int RollD10() { return rnd.Next(1, 11); }
        /// <summary>
        /// Simulates rolling a number of 10-sided dice depending on param
        /// </summary>
        /// <param name="times"># of dices to be rolled.</param>
        /// <param name="rule">Whether this roll is made with advantage, disadvantage, or normal</param>
        /// <returns>The sum of dices rolled.</returns>
        internal static int RollD10(int times, string rule)
        {
            if (times <= 0)
            {
                throw new ArgumentException("'Times' must be at least 1", $"times: {times}");
            }
            DiceState trueRule = DAN(rule);
            int roll = 0;
            for (int i = 0; i < times; i++)
            {
                int roll1 = RollD10(); int roll2 = RollD10();
                if (trueRule == DiceState.Disadvantage) { roll += Math.Min(roll1, roll2); }
                else if (trueRule == DiceState.Advantage) { roll += Math.Max(roll1, roll2); }
                else { roll += roll1; }
            }
            return roll;
        }

        /// <summary>
        /// Simulates rolling a 12-sided die
        /// </summary>
        /// <returns>An integer between 1-12</returns>
        internal static int RollD12() { return rnd.Next(1, 13); }
        /// <summary>
        /// Simulates rolling a number of 12-sided dice depending on param
        /// </summary>
        /// <param name="times"># of dices to be rolled.</param>
        /// <param name="rule">Whether this roll is made with advantage, disadvantage, or normal</param>
        /// <returns>The sum of dices rolled.</returns>
        internal static int RollD12(int times, string rule)
        {
            if (times <= 0)
            {
                throw new ArgumentException("'Times' must be at least 1", $"times: {times}");
            }
            DiceState trueRule = DAN(rule);
            int roll = 0;
            for (int i = 0; i < times; i++)
            {
                int roll1 = RollD12(); int roll2 = RollD12();
                if (trueRule == DiceState.Disadvantage) { roll += Math.Min(roll1, roll2); }
                else if (trueRule == DiceState.Advantage) { roll += Math.Max(roll1, roll2); }
                else { roll += roll1; }
            }
            return roll;
        }

        /// <summary>
        /// Simulates rolling a 20-sided die.
        /// </summary>
        /// <returns>An integer between 1-20</returns>
        internal static int RollD20() { return rnd.Next(1, 21); }
        /// <summary>
        /// Simulates rolling a number of 20-sided dice depending on param
        /// </summary>
        /// <param name="times"># of dices to be rolled.</param>
        /// <param name="rule">Whether this roll is made with advantage, disadvantage, or normal</param>
        /// <returns>The sum of dices rolled.</returns>
        internal static int RollD20(int times, string rule)
        {
            if (times <= 0)
            {
                throw new ArgumentException("'Times' must be at least 1", $"times: {times}");
            }
            DiceState trueRule = DAN(rule);
            int roll = 0;
            for (int i = 0; i < times; i++)
            {
                int roll1 = RollD20(); int roll2 = RollD20();
                if (trueRule == DiceState.Disadvantage) { roll += Math.Min(roll1, roll2); }
                else if (trueRule == DiceState.Advantage) { roll += Math.Max(roll1, roll2); }
                else { roll += roll1; }
            }
            return roll;
        }

        /// <summary>
        /// 1:1 replica of Random's Next() for cases that don't involve standard rolling.
        /// </summary>
        /// <param name="min">Minimum value (Inclusive)</param>
        /// <param name="max">Maximum value (Exclusive)</param>
        internal static int Next(int min, int max) { return rnd.Next(min, max); }
    }
}