using static DNDBattler.Helper;
using static DNDBattler.DiceRoller;

namespace DNDBattler
{
    /// <summary>
    /// Features common code for all zombies, though it also represents the default 5e zombie monster.
    /// </summary>
    internal class Zombie : CommonCharacter
    {
        protected string type; //exists to differentiate zombie type
        /// <summary>
        /// Almost default constructor in which the 'Zombie' object represents a stereotypical 5e zombie. 
        /// Still gotta give a name.
        /// </summary>
        /// <param name="name">Character's name.</param>
        public Zombie(string name) :
        base(name, 15, 8, 13, 6, 16, 3, 6, 5, 
            new string[] { "none" }, new string[] { "poison" }, new string[] { "none" }, 
            new int[] {0, 0, 0, 0, 0, 0})
        {
            type = "Zombie";
        }

        /// <summary>
        /// Alt constructor mainly for children with different stats
        /// </summary>
        /// <param name="name">Character's name.</param>
        /// <param name="maxHealth">The creature's maximum health value</param>
        /// <param name="armourClass">Minimum attack value needed to damage the character.</param>
        /// <param name="strength">strength ability score</param>
        /// <param name="dexterity">dexterity ability score</param>
        /// <param name="constitution">constitution ability score</param>
        /// <param name="intelligence">intelligence ability score</param>
        /// <param name="wisdom">wisdom ability score</param>
        /// <param name="charisma">charisma ability score</param>
        /// <param name="resistances">Damage types character takes half damage from</param>
        /// <param name="immunities">Damage types character takes no damage from</param>
        /// <param name="vulnerabilities">Damage types character takes double damage from.</param>
        /// <param name="bonuses">array of any saving throw bonuses character might have. 
        /// Index corresponds to each ability.</param>
        public Zombie(string name, string type, int maxHealth, int armourClass, 
            int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma, 
            string[] resistances, string[] immunities, string[] vulnerabilities, int[] bonuses) : 
            base(name, maxHealth, armourClass, strength, dexterity, constitution, 
                intelligence, wisdom, charisma, resistances, immunities, vulnerabilities, bonuses)
        {
            this.type = type;
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
            if(Is(Conditions.Poisoned)) { DABalance -= 1; }
            if(Is(Conditions.Frightened)) { DABalance -= 1; }
            if(target.Is(Conditions.Paralyzed)) { DABalance += 1; }

            if(DABalance < 0) { state = "disadvantage"; }
            else if(DABalance > 0) { state = "adavantage"; }
            int attackRoll = RollD20(1, state);

            if (attackRoll == 20 ||
                (target.Is(Conditions.Paralyzed) && target.AC <= attackRoll + 3))
            {
                int damage = RollD6(2, "normal") + 1;
                string result = target.TakeDamage(damage, DirectConvert("Bludgeoning"), true);
                return $"{Name} lands a critical slam on {target.Name}. {result}";
            }
            else if (target.AC <= attackRoll + 3)
            {
                int damage = RollD6() + 1;
                string result = target.TakeDamage(damage, DirectConvert("Bludgeoning"), false);
                return $"{Name} slams {target.Name}. {result}";
            }
            else
            {
                return $"{Name} attempts to slam {target.Name}, but missed.";
            }
        }
        /// <summary>
        /// Calculates the results of zombie taking damage. If the zombie succeeds on a saving throw 
        /// (and the damage dealt wasn't radiant or a crit) it can cheat death.
        /// </summary>
        /// <param name="amount">amount of damage dealt.</param>
        /// <param name="type">Damage type being dealt.</param>
        /// <param name="crit">Whether the damage was a crit</param>
        /// <returns></returns>
        public override string TakeDamage(int amount, DamageType type, bool crit)
        {
            string result = "";
            int finalDmg = amount;
            if(immunities.Contains((int)type))
            {
                return $"Target is immune to {type.ToString()} damage.";
            } else
            {
                if (resistances.Contains((int)type))
                {
                    Health -= amount / 2;
                    finalDmg = amount / 2;
                    result += $"{Name} resists {type.ToString()}, and only took {amount / 2} damage.";
                } else
                {
                    Health -= amount;
                    result += $"{Name} takes {amount} damage.";
                }

                /*If zombie is "killed" and conditions are met (damage isn't crit or radiant) the
                 zombie rolls a saving throw, result is adjusted based on the reasons for failure*/
                if(Health <= 0 && !type.ToString().EqualsIgnoreCase("radiant") && !crit)
                {
                    int dc = 5 + finalDmg;
                    string saveRoll;
                    if (SavingThrow(dc, "Constitution", out saveRoll))
                    {
                        Health = 1;
                        result += $"\n{Name}'s Undead Fortitude allows them to continue fighting despite "
                            + $"the lethal hit ({saveRoll}).";
                    } else
                    {
                        result += $"\n{Name} failed to channel their Undead Fortitude ({saveRoll})!";
                    }
                } else if(Health <= 0 && (type.ToString().EqualsIgnoreCase("radiant") || crit))
                {
                    result += " The attack was either radiant or critical, so Undead Fortitude failed!";
                }
                if (IsDead)
                {
                    result += $" {Name} has succumbed to lethal damage.";
                }
            }
            return result;
        }

        /// <summary>
        /// Unintelligent undead don't really flee.
        /// </summary>
        public override bool ReadyToFlee { get { return false; } }

        public override string ToString()
        {
            return $"{Name}, a {type}: \n{base.ToString()} \nDoth Undead flee? {ReadyToFlee}";
        }
    }
}
