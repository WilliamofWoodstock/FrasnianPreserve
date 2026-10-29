using System.Numerics;
using static DNDBattler.DiceRoller;
using static DNDBattler.Helper;

namespace DNDBattler
{
    internal class Paladin : CommonCharacter
    {
        /// <summary>
        /// Proficiency bonus, Based on level
        /// </summary>
        public int PB { get; }
        /// <summary>
        /// Level, determines if certain features can be used
        /// </summary>
        public int Level { get; }
        /// <summary>
        /// Stores the amount of hp Lay On Hands feature has remaining.
        /// </summary>
        public int LayOnHands { get; set; }
        /// <summary>
        /// Whether Paladin has the Aura of Protection feature
        /// </summary>
        public bool ProtectionAura
        {
            get { return Level >= 6; }
        }
        /// <summary>
        /// Whether Paladin has the Aura of Courage feature.
        /// </summary>
        public bool CourageAura
        {
            get { return Level >= 10; }
        }
        /// <summary>
        /// Whetehr Paladin has the Extra attack feature
        /// </summary>
        public bool ExtraAttack
        {
            get
            {
                return Level >= 5;
            }
        }
        /// <summary>
        /// Tracks whether the paladin has any spell slots remaining.
        /// </summary>
        public bool HasSpellSlots
        {
            get
            {
                bool has = false;
                foreach(int i in spellSlots)
                {
                    if(i > 0)
                    {
                        has = true;
                    }
                }
                return has;
            }
        }
        protected int[] spellSlots = { 0, 0, 0 };

        /// <summary>
        /// Paladin constructor that makes a couple assumptions that being that the paladin 
        /// uses no spells,  uses Defense fighting style,
        /// and uses a shield + 1d8 sword. Only has features up to Level 10.
        /// </summary>
        /// <param name="name">Paladin's name</param>
        /// <param name="baseAC">Base AC (10 + whatever armour you had in mind)</param>
        /// <param name="strength">strength ability score</param>
        /// <param name="dexterity">dexterity ability score</param>
        /// <param name="constitution">constitution ability score</param>
        /// <param name="intelligence">intelligence ability score</param>
        /// <param name="wisdom">wisdom ability score</param>
        /// <param name="charisma">charisma ability score</param>
        /// <param name="level">Paladin's level</param>
        public Paladin(string name, int baseAC, int strength, int dexterity, 
            int constitution, int intelligence, int wisdom, int charisma, int level) :
            base(name, strength, dexterity, constitution,
            intelligence, wisdom, charisma)
        {
            armourClass = baseAC + GetMod("dexterity") + 2;
            Level = level;
            if(level >= 2) { armourClass += 1; }
            PB = level.GetPB();
            saveBonus.Add("Wisdom", PB + GetMod("Wisdom"));
            saveBonus.Add("Charisma", PB + GetMod("Charisma"));
            int extraL = level - 1; //For health calc
            //Uses average health calculation (only fair since same for monsters)
            maxHealth = ((6 + GetMod("Constitution")) * extraL) + 10;
            Health = maxHealth;
            LayOnHands = level * 5;
            
            if(Level == 2) { spellSlots[0] = 2; } 
            else if(Level <= 4) { spellSlots[0] = 3; }
            else if(Level <= 6)
            {
                spellSlots[0] = 4; spellSlots[1] = 2;
            }
            else if(Level <= 8)
            {
                spellSlots[0] = 4; spellSlots[1] = 3;
            } else
            {
                spellSlots[0] = 4;
                spellSlots[1] = 3;
                spellSlots[2] = 2;
            }
            //1d20 + Dexterity modifier
            Initiative = RollD20() + GetMod("dexterity");
        }

        /// <summary>
        /// Simulates a paladin's attack round. Paladin will heal themselves under certain 
        /// conditions and tactically chooses attack targets if they intend to use large smites.
        /// </summary>
        /// <param name="targets">array of viable targets</param>
        /// <returns>String summarizing the events.</returns>
        public override string Attack(CommonCharacter[] targets)
        {
            string result = "";
            /*Paladin uses Lay on Hands on themself under the following conditions
             * 1. There are still health points that can be used
             * 2. Paladin isn't heal-restricted
             * 3. Paladin is at or less than half health
             * 4. There are still at least 3 enemies around OR LOH can still heal at least 
                  20% health */
            if(LayOnHands > 0 && !CantHeal && Health < (maxHealth / 2) && 
                ((Enemies(targets) !< 3) || LayOnHands >= (maxHealth * 0.2)))
            {
                int healDiff = maxHealth - Health;
                if(healDiff >= LayOnHands)
                {
                    Health += LayOnHands;
                    string heal = $"{Name} expends all of their healing power to heal {LayOnHands}";
                    LayOnHands = 0;
                    return heal;
                } else
                {
                    Health += healDiff;
                    LayOnHands -= healDiff;
                    return $"{Name} expends some healing power to heal {healDiff}";
                }
            } else
            {
                int slot = FindBiggestSlot();
                CommonCharacter target = DecideTarget(targets, slot);

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
                (target.Is(Conditions.Paralyzed) && target.AC <= attackRoll + GetMod("Strength") + PB))
                {
                    int damage = RollD8(2, "normal") + GetMod("Strength");
                    result += $"{Name} lands a critical stab on {target.Name}. ";
                    result += target.TakeDamage(damage, DirectConvert("Slashing"), true);
                    if(slot != -1 && !target.IsDead)
                    {
                        spellSlots[slot]--;
                        //slot is index so its already 1 less than the actual spell level.
                        int rollNum = (2 + slot) * 2;
                        damage = RollD8(rollNum, "normal");
                        result += $"\n{Name} activates a brutal Divine Smite. " +
                            $"\n{target.TakeDamage(damage, DamageType.Radiant, true)}";
                    }
                }
                else if (target.AC <= attackRoll + GetMod("Strength") + PB)
                {
                    int damage = RollD8(1, "normal") + GetMod("Strength");
                    result += $"{Name} slashes {target.Name}. ";
                    result += target.TakeDamage(damage, DirectConvert("Slashing"), false);
                    if (slot != -1 && !target.IsDead)
                    {
                        spellSlots[slot]--;
                        //slot is index so its already 1 less than the actual spell level.
                        int rollNum = 2 + slot;
                        damage = RollD8(rollNum, "normal");
                        result += $"\n{Name} activates a Divine Smite. " +
                            $"\n{target.TakeDamage(damage, DamageType.Radiant, false)}";
                    }
                }
                else
                {
                    result += $"{Name} attempts to slash {target.Name}, but missed.";
                }
                
                //If Paladin is level 5 then we basically repeat the process
                if(ExtraAttack)
                {
                    slot = FindBiggestSlot();
                    target = DecideTarget(targets, slot);
                    attackRoll = RollD20(1, state);

                    if (attackRoll == 20 ||
                (target.Is(Conditions.Paralyzed) && target.AC <= attackRoll + GetMod("Strength") + PB))
                    {
                        int damage = RollD8(2, "normal") + GetMod("Strength");
                        result += $"\n{Name} lands a critical stab on {target.Name}. ";
                        result += target.TakeDamage(damage, DirectConvert("Slashing"), true);
                        if (slot != -1 && !target.IsDead)
                        {
                            spellSlots[slot]--;
                            //slot is index so its already 1 less than the actual spell level.
                            int rollNum = (2 + slot) * 2;
                            damage = RollD8(rollNum, "normal");
                            result += $"\n{Name} activates a brutal Divine Smite. " +
                                $"\n{target.TakeDamage(damage, DamageType.Radiant, true)}";
                        }
                    }
                    else if (target.AC <= attackRoll + GetMod("Strength") + PB)
                    {
                        int damage = RollD8(1, "normal") + GetMod("Strength");
                        result += $"\n{Name} slashes {target.Name}. ";
                        result += target.TakeDamage(damage, DirectConvert("Slashing"), false);
                        if (slot != -1 && !target.IsDead)
                        {
                            spellSlots[slot]--;
                            //slot is index so its already 1 less than the actual spell level.
                            int rollNum = 2 + slot;
                            damage = RollD8(rollNum, "normal");
                            result += $"\n{Name} activates a Divine Smite. " +
                                $"\n{target.TakeDamage(damage, DamageType.Radiant, false)}";
                        }
                    }
                    else
                    {
                        result += $"\n{Name} attempts to slash {target.Name}, but missed.";
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Paladin picks their target based on what spellslot they're using for smite.
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        internal CommonCharacter DecideTarget(CommonCharacter[] targets, int slot)
        {
            if(slot == 2)
            {
                CommonCharacter tankiest = targets[0];
                foreach(CommonCharacter c in targets)
                {
                    if(c.Health > tankiest.Health)
                    {
                        tankiest = c;
                    }
                }
                return tankiest;
            } else
            {
                CommonCharacter choice = targets[Next(0, targets.Length)];
                while(choice.IsDead)
                {
                    choice = targets[Next(0, targets.Length)];
                }
                return choice;
            }
        }
        /// <summary>
        /// Finds the highest index in spellSLots with a number above 0
        /// </summary>
        /// <returns>index corresponding to the highest index with a slot, 
        /// or -1 if all spell slots are expended.</returns>
        internal int FindBiggestSlot()
        {
            if (spellSlots[2] > 0)
            {
                return 2;
            } else if (spellSlots[1] > 0)
            {
                return 1;
            } else if (spellSlots[0] > 0)
            {
                return 0;
            } else
            {
                return -1;
            }
        }

        /// <summary>
        /// Same as the parent method except adjusting for Aura of Protection (if the 
        /// paladin's level is high enough).
        /// </summary>
        /// <param name="dc">Minimum value for succeeding the saving throw.</param>
        /// <param name="ability">Determines which ability modifier will be added to the roll.</param>
        /// <returns>True if roll + mod + protection (if applicable) 
        /// meets or beats dc, false if otherwise.</returns>
        public override bool SavingThrow(int dc, string ability)
        {
            if (Is(Conditions.Paralyzed))
            {
                if (ability.EqualsIgnoreCase("strength") || ability.EqualsIgnoreCase("dexterity")) { return false; }
            }
            int roll = RollD20();
            int bonus = GetMod(ability);
            if (saveBonus.ContainsKey(ability)) { bonus = saveBonus[ability]; }
            if(ProtectionAura) { bonus += GetMod("Charisma"); }

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
        /// Paladins are generally brave so their margins are much smaller (10% at <30% hp),
        ///  35% at 10% hp or less.
        /// </summary>
        public override bool ReadyToFlee
        {
            get
            {
                int roll = Next(1, 101);
                if (Health < (maxHealth * 0.3))
                {
                    if(roll <= 10) { return true; }
                    else { return false; }
                } else if(Health <= (maxHealth * 0.1))
                {
                    if (roll <= 35) { return true; }
                    else { return false; }
                } else { return false;}
            }
        }

        /// <summary>
        /// Allows paladin to assess the number of enemies left.
        /// </summary>
        /// <param name="battlefield"></param>
        /// <returns></returns>
        public int Enemies(CommonCharacter[] battlefield)
        {
            return battlefield.Length;
        }

        public override string ToString()
        {
            return $"{Name}, a Level {Level} Paladin: " +
                $"\n{base.ToString()}\nWields a sword & shield.";
        }
    }
}
