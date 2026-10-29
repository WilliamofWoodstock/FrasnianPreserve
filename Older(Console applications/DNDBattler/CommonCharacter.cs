using static DNDBattler.Helper;
using static DNDBattler.DiceRoller;

namespace DNDBattler
{
    /// <summary>
    /// Enum representing all 13 damage types in DND 5e. ints range from 1-13.
    /// </summary>
    enum DamageType
    {
        Slashing = 1,
        Piercing,
        Bludgeoning,
        Fire,
        Cold,
        Poison,
        Lightning,
        Thunder,
        Acid,
        Force,
        Psychic,
        Radiant,
        Necrotic
    }

    internal abstract class CommonCharacter
    {
        protected int maxHealth; protected int armourClass;
        protected int[] resistances; protected int[] immunities; protected int[] vulnerabilities;
        //stores ability scores
        internal Dictionary<string, int> abilities = new Dictionary<string, int>();
        //tracks abilities with a bonus to their save.
        protected Dictionary<string, int> saveBonus = new Dictionary<string, int>();
        internal List<Condition> conditions = new List<Condition>();
        /// <summary>
        /// Character's current (not max) health value.
        /// </summary>
        public int Health { get; set; }
        /// <summary>
        /// Minimum attack roll needed to damage character
        /// </summary>
        public int AC { get { return armourClass; } }
        /// <summary>
        /// Character's name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// If a character is afflicted by a feature that prevents them from healing. Needed for certain features.
        /// </summary>
        public bool CantHeal { get; set; }
        /// <summary>
        /// Initiative roll
        /// </summary>
        public int Initiative { get; set; }

        /// <summary>
        /// Primary constructor in which everything in decided by parameters.
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
        public CommonCharacter(string name, int maxHealth, int armourClass, 
            int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma, 
            string[] resistances, string[] immunities, string[] vulnerabilities, int[] bonuses)
        {
            this.maxHealth = maxHealth; Health = maxHealth; Name = name;
            this.armourClass = armourClass;
            abilities.Add("Strength", strength); abilities.Add("Dexterity", dexterity); abilities.Add("Constitution", constitution);
            abilities.Add("Intelligence", intelligence); abilities.Add("Wisdom", wisdom); abilities.Add("Charisma", charisma);
            this.resistances = ConvertDamage(resistances);
            this.immunities = ConvertDamage(immunities);
            this.vulnerabilities = ConvertDamage(vulnerabilities);

            string[] abilityArr = { "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma" };
            for(int i = 0; i < bonuses.Length; i++)
            {
                int mod = GetMod(abilityArr[i]);
                if(bonuses[i] > mod)
                {
                    saveBonus.Add(abilityArr[i], bonuses[i]);
                }
            }
            //1d20 + Dexterity modifier
            Initiative = RollD20() + GetMod("dexterity");
        }

        /// <summary>
        /// Constructor primarily for human player characters who have their own rules for deciding 
        /// health, AC, initiative, etc. They also don't have resistances and the like by default
        /// </summary>
        /// <param name="name"></param>
        /// <param name="strength"></param>
        /// <param name="dexterity"></param>
        /// <param name="constitution"></param>
        /// <param name="intelligence"></param>
        /// <param name="wisdom"></param>
        /// <param name="charisma"></param>
        public CommonCharacter(string name, int strength, int dexterity, int constitution, 
            int intelligence, int wisdom, int charisma)
        {
            Name = name; maxHealth = 1; armourClass = 10;
            abilities.Add("Strength", strength); abilities.Add("Dexterity", dexterity); abilities.Add("Constitution", constitution);
            abilities.Add("Intelligence", intelligence); abilities.Add("Wisdom", wisdom); abilities.Add("Charisma", charisma);
            resistances = new int[] { -1 }; immunities = new int[] { -1 }; vulnerabilities = new int[] { -1 };
        }

        /// <summary>
        /// Alt constructor that does not deal with damage types (resistances, immunities).
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
        /// <param name="bonuses">array of any saving throw bonuses character might have. 
        /// Index corresponds to each ability.</param>
        public CommonCharacter(string name, int maxHealth, int armourClass,
            int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma, int[] bonuses)
        {
            this.maxHealth = maxHealth; Health = maxHealth; Name = name;
            this.armourClass = armourClass;
            abilities.Add("Strength", strength); abilities.Add("Dexterity", dexterity); abilities.Add("Constitution", constitution);
            abilities.Add("Intelligence", intelligence); abilities.Add("Wisdom", wisdom); abilities.Add("Charisma", charisma);
            resistances = new int[] { -1 }; immunities = new int[] { -1 }; vulnerabilities = new int[] { -1 };

            string[] abilityArr = { "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma" };
            for (int i = 0; i < bonuses.Length; i++)
            {
                int mod = GetMod(abilityArr[i]);
                if (bonuses[i] > mod)
                {
                    saveBonus.Add(abilityArr[i], bonuses[i]);
                }
            }
            //1d20 + Dexterity modifier
            Initiative = RollD20() + GetMod("dexterity");
        }
        /// <summary>
        /// The closest thing to a 'default constructor' in which only name, health and AC are provided. 
        /// All abilities default to 10 (+0) and no resistances or immunities.
        /// </summary>
        /// <param name="name">Character's name.</param>
        /// <param name="maxHealth">Maximum health value</param>
        /// <param name="armourClass">Minimum attack value needed to hit.</param>
        public CommonCharacter(string name, int maxHealth, int armourClass)
        {
            this.maxHealth = maxHealth; Health = maxHealth; Name = name;
            this.armourClass = armourClass;
            abilities.Add("Strength", 10); abilities.Add("Dexterity", 10); abilities.Add("Constitution", 10);
            abilities.Add("Intelligence", 10); abilities.Add("Wisdom", 10); abilities.Add("Charisma", 10);
            resistances = new int[] { -1 };
            immunities = new int[] { -1 };
            vulnerabilities = new int[] { -1 };
            //1d20 + Dexterity modifier
            Initiative = RollD20() + GetMod("dexterity");
        }

        /// <summary>
        /// Checks whether the character is dead, returning true if they're health is 0 or less.
        /// </summary>
        public bool IsDead
        {
            get
            {
                if(Health <= 0)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Searches (using .Any()) to see if the creature has a certain condition.
        /// </summary>
        /// <param name="type">Conditions enum to be searched for.</param>
        /// <returns>true if character has a certain condition, false if otherwise.</returns>
        public bool Is(Conditions type)
        {
            if (conditions.Any(con => con.Type == type)) { return true; }
            return false;
        }

        /// <summary>
        /// Gets modifier for an ability (case insensitive).
        /// </summary>
        /// <param name="ability">one of the six abilities</param>
        /// <exception cref="ArgumentException">String doesn't match any ability.</exception>"
        /// <returns>the specific ability's modifier</returns>
        public int GetMod(string ability)
        {
            if(ability.EqualsIgnoreCase("strength")) { return abilities["Strength"].StatToMod(); }
            else if (ability.EqualsIgnoreCase("dexterity")) { return abilities["Dexterity"].StatToMod(); }
            else if (ability.EqualsIgnoreCase("constitution")) { return abilities["Constitution"].StatToMod(); }
            else if (ability.EqualsIgnoreCase("intelligence")) { return abilities["Intelligence"].StatToMod(); }
            else if (ability.EqualsIgnoreCase("wisdom")) { return abilities["Wisdom"].StatToMod(); }
            else if (ability.EqualsIgnoreCase("charisma")) { return abilities["Charisma"].StatToMod(); }
            throw new ArgumentException("String doesn't match any of the 6 abilities", ability);
        }

        /// <summary>
        /// Tracks whether a character is ready to flee. At a base, they have a 1/3 probability once 
        /// they are less than half heaelth, and then guaranteed when at 25% or less.
        /// </summary>
        public virtual bool ReadyToFlee
        {
            get
            {
                if(Health <= (maxHealth * 0.25))
                {
                    return true;
                } else if (Health < (maxHealth / 2))
                {
                    int roll = RollD6();
                    if(roll < 3) { return true; } else { return false; }
                } else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Common code for giving characters a condition. New conditions with the same type as another replaces the older 
        /// condition.
        /// </summary>
        /// <param name="condition"></param>
        public void AddCon(Condition condition)
        {
            for(int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].Type == condition.Type)
                {
                    conditions.Remove(conditions[i]);
                }
            }
            conditions.Add(condition);
        }

        public abstract string Attack(CommonCharacter[] targets);

        /// <summary>
        /// Common code for taking damage, accountinf for resistances and immunities.
        /// </summary>
        /// <param name="amount">amount of damage being taken.</param>
        /// <param name="type">Damage type</param>
        /// <param name="crit">Whether the attack was a critical hit. Used for certain abilities.</param>
        /// <returns>A string that sumamrizes the result.</returns>
        public virtual string TakeDamage(int amount, DamageType type, bool crit)
        {
            string result = "";
            if(resistances.Contains((int)type))
            {
                Health -= amount / 2;
                result += $"{Name} resists {type.ToString()}, and only took {amount / 2} damage.";
            } else if(immunities.Contains((int)type))
            {
                result += $"{Name} is immune to {type.ToString()} damage.";
            } else
            {
                Health -= amount;
                result += $"{Name} takes {amount} damage.";
            }

            if(IsDead)
            {
                result += $" {Name} has been felled by the attack.";
            }
            return result;
        }

        /// <summary>
        /// Common code for making a saving throw. Now case insensitive!
        /// </summary>
        /// <param name="dc">Minimum value for succeeding the saving throw.</param>
        /// <param name="ability">Determines which ability modifier will be added to the roll.</param>
        /// <returns>True if roll + mod meets or beats dc, false if otherwise.</returns>
        public virtual bool SavingThrow(int dc, string ability)
        {
            if(Is(Conditions.Paralyzed))
            {
                if(ability.EqualsIgnoreCase("strength") || ability.EqualsIgnoreCase("dexterity")) { return false; }
            }
            int roll = RollD20();
            int bonus = GetMod(ability);
            if(saveBonus.ContainsKey(ability)) { bonus = saveBonus[ability]; }
            
            if (roll + bonus >= dc)
            {
                return true;
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Savingthrow overload with the ability to report on what the roll was
        /// </summary>
        /// <param name="dc">Minimum value for succeeding the saving throw.</param>
        /// <param name="ability">Determines which ability modifier will be added to the roll.</param>
        /// <param name="result">string containing the sum of the roll and its bonus.</param>
        /// <returns>True if roll + mod meets or beats dc, false if otherwise.</returns>
        public virtual bool SavingThrow(int dc, string ability, out string result)
        {
            if (Is(Conditions.Paralyzed))
            {
                if (ability.EqualsIgnoreCase("strength") || ability.EqualsIgnoreCase("dexterity")) 
                {
                    result = "Paralyzed creatures fail str or dex saves.";
                    return false; 
                }
            }
            int roll = RollD20();
            int bonus = GetMod(ability);
            if (saveBonus.ContainsKey(ability)) { bonus = saveBonus[ability]; }
            result = $"rolled {roll+bonus}({bonus}) vs dc {dc}";

            if (roll + bonus >= dc)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handles any conditions or features that change as the creature's turn ends.
        /// </summary>
        /// <returns>A string summarizing the result</returns>
        public virtual string EndTurn()
        {
            string result = "";
            for(int i = conditions.Count - 1; i >= 0; i--)
            {
                conditions[i].Duration--;
                if(conditions[i].Duration <= 0)
                {
                    result += $"{conditions[i].Type} wore off.";
                    conditions.RemoveAt(i);
                } else if(conditions[i].RepeatSave)
                {
                    if (SavingThrow(conditions[i].SaveDC, conditions[i].SaveAbility))
                    {
                        result += conditions[i].End();
                        conditions.RemoveAt(i);
                    } else
                    {
                        result += $"{Name} tried unsuccessfully to end its {conditions[i].Type}";
                    }
                }
                result += "\n";
            }

            return result;
        }

        /// <summary>
        /// Like ToString except only for their name and health values. For display purposes.
        /// </summary>
        /// <returns>A string detailing the characters name and health.</returns>
        public string BasicInfo()
        {
            return $"{Name}: {Health}/{maxHealth}";
        }
        /// <summary>
        /// Crunches all basic info (stats, resistances, health, etc) into a string;
        /// </summary>
        /// <returns>Strign containing all basic info.</returns>
        public override string ToString()
        {
            string info = $"Has {Health}/{maxHealth} health and {armourClass} AC.\n"
                + $"Initiative roll: {Initiative}\n";
            foreach(var ele in abilities)
            {
                info += $"{ele.Key}: {ele.Value} | ";
            }

            info += "\nResists: ";
            for(int i = 0; i < resistances.Length; i++)
            {
                if (resistances[i] == -1)
                {
                    info += "None";
                    break;
                } else if (i == resistances.Length - 1)
                {
                    if(resistances.Length > 1) { info += "and "; }
                    info += $"{(DamageType)resistances[i]}.";
                } else
                {
                    info += $"{(DamageType)resistances[i]}, ";
                }
            }

            info += "\nImmunities: ";
            for(int i = 0; i < immunities.Length; i++)
            {
                if (immunities[i] == -1)
                {
                    info += "None";
                    break;
                }
                else if (i == immunities.Length - 1)
                {
                    if (immunities.Length > 1) { info += "and ";  }
                    info += $"{(DamageType)immunities[i]}.";
                }
                else
                {
                    info += $"{(DamageType)immunities[i]}, ";
                }
            }
            info += $"\nIs Dead? {IsDead}";
            return info;
        }
    }
}
