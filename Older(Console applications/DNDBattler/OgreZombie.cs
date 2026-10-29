using static DNDBattler.DiceRoller;
using static DNDBattler.Helper;

namespace DNDBattler
{
    internal class OgreZombie : Zombie
    {
        public OgreZombie(string name) : 
        base(name, "Ogre Zombie", 85, 8, 19, 6, 18, 3, 6, 5, 
            new string[] { "None" }, new string[] { "Poison" }, new string[] { "none" }, 
            new int[] {0, 0, 0, 0, 0, 0})
        {
            //No instance Data really
        }

        /// <summary>
        /// randomly chooses a target then replicates zombie's Slam attack.
        /// </summary>
        /// <param name="targets">List of all possible targets.</param>
        /// <returns>The result of the attack (miss, hit, etc)</returns>
        public override string Attack(CommonCharacter[] targets)
        {
            CommonCharacter target = targets[Next(0, targets.Length)];
            string state = "Normal";
            //Determines dice state (disadvantage vs advantage)
            //Based on whether one factor has more than another
            int DABalance = 0;
            if (Is(Conditions.Poisoned)) { DABalance -= 1; }
            if (Is(Conditions.Frightened)) { DABalance -= 1; }
            if (target.Is(Conditions.Paralyzed)) { DABalance += 1; }

            if (DABalance < 0) { state = "disadvantage"; }
            else if (DABalance > 0) { state = "adavantage"; }
            int attackRoll = RollD20(1, state);

            if (attackRoll == 20 ||
                (target.Is(Conditions.Paralyzed) && target.AC <= attackRoll + 6))
            {
                int damage = RollD8(4, "normal") + 4;
                string result = target.TakeDamage(damage, DirectConvert("Bludgeoning"), true);
                return $"{Name} lands a critical slam on {target.Name}. \n{result}";
            }
            else if (target.AC <= attackRoll + 6)
            {
                int damage = RollD8(2, "normal") + 4;
                string result = target.TakeDamage(damage, DirectConvert("Bludgeoning"), false);
                return $"{Name} slams {target.Name}. \n{result}";
            }
            else
            {
                return $"{Name} attempts to slam {target.Name}, but missed.";
            }
        }
    }
}
