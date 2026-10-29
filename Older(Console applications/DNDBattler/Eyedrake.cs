using static DNDBattler.DiceRoller;
using static DNDBattler.Helper;

namespace DNDBattler
{
    internal class Eyedrake : CommonCharacter
    {
        /// <summary>
        /// Tracks whether Anti-Magic breath is ready
        /// </summary>
        public bool Recharge { get; set; } = true;
        /// <summary>
        /// Eyedrake constructor (they have very little unique instance data as a monster)
        /// </summary>
        /// <param name="name">Eyedrake's name</param>
        public Eyedrake(string name) :
            base(name, 119, 16, 16, 10, 16, 14, 14, 16, 
                new string[] { "None" }, new string[] { "None" }, new string[] { "None" },
                new int[] { 0, 0, 6, 0, 5, 0 })
        {

        }

        /// <summary>
        /// Simulates one turn of an Eyedrake, including recharge roll and attacks.
        /// </summary>
        /// <param name="targets">All viable targets</param>
        /// <returns>String summarizing events </returns>
        public override string Attack(CommonCharacter[] targets)
        {
            int rechargeRoll = RollD6();
            if(rechargeRoll == 6) { Recharge = true; }
            string result = "";
            if(Recharge)
            {
                result = AntiMagicBreath(targets);
                Recharge = false;
            } else
            {
                int decision = RollD10();
                if(decision == 1)
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
                    //Melee attacks on paralyzed targets are critical by default
                    if (attackRoll == 20 ||
                        (target.Is(Conditions.Paralyzed) && target.AC <= attackRoll + 6))
                    {
                        int damage = RollD6(6, "Normal") + 3;
                        result = target.TakeDamage(damage, DirectConvert("Piercing"), true);
                        return $"{Name} lands a critical bite on {target.Name}. {result}";
                    }
                    else if (target.AC <= attackRoll + 6)
                    {
                        int damage = RollD6(3, "Normal") + 3;
                        result = target.TakeDamage(damage, DirectConvert("Piercing"), false);
                        return $"{Name} bites {target.Name}. {result}";
                    }
                    else
                    {
                        return $"{Name} attempts to bite {target.Name}, but missed.";
                    }
                } else
                {
                    result = EyeRays(targets);
                }
            }

            return result;
        }

        /// <summary>
        /// Simulates the Eyedrake's eye ray attacks
        /// </summary>
        /// <param name="targets">All viable targets, up to 3 will be damaged</param>
        /// <returns>string summarizing events.</returns>
        public string EyeRays(CommonCharacter[] targets)
        {
            int[] beams = new int[3];
            for(int i = 0; i < 3; i++)
            {
                int roll = RollD6();
                while(beams.Contains(roll)) { roll = RollD6(); }
                beams[i] = roll;
            }
            CommonCharacter[] choices = AssignTargets(beams, targets);
            string result = "";

            for(int i = 0; i< choices.Length; i++)
            {
                int damage; string saveRoll;
                if(i == 1)
                {
                    if (choices[i].SavingThrow(14, "Constitution", out saveRoll))
                    {
                        damage = RollD6(5, "normal") / 2;
                        result += $"\n{Name} fires a freezing ray, but {choices[i].Name} endured its chill " +
                            $"({saveRoll}).";
                        result += $"\n{choices[i].TakeDamage(damage, DamageType.Cold, false)}";
                    } else
                    {
                        damage = RollD6(5, "normal");
                        result += $"\n{Name} fires a freezing ray, chilling {choices[i].Name} to the bone " +
                            $"({saveRoll}).";
                        result += $"\n{choices[i].TakeDamage(damage, DamageType.Cold, false)}";
                    }
                } else if(i == 2)
                {
                    if (choices[i].SavingThrow(14, "Constitution", out saveRoll))
                    {
                        result += $"\n{Name} fires a debilitating ray, but {choices[i].Name} remained oriented" +
                            $" ({saveRoll}).";
                    } else
                    {
                        damage = RollD6(2, "normal");
                        choices[i].AddCon(new Condition("Incapacitated", 2, this, choices[i]));
                        result += $"\n{Name} fires a debilitating ray, leaving {choices[i].Name} INCAPACITATED " +
                            $"({saveRoll})!";
                        result += $"\n{choices[i].TakeDamage(damage, DamageType.Thunder, false)}";
                    }
                } else if(i == 3)
                {
                    if (choices[i].SavingThrow(14, "Strength", out saveRoll))
                    {
                        result += $"\n{Name} fires a repulsion ray, but {choices[i].Name} is not " +
                            $"so easily moved ({saveRoll}).";
                    } else
                    {
                        damage = RollD6(4, "normal");
                        result += $"\n{Name} fires a repulsion ray, violently displacing " +
                            $"{choices[i].Name} ({saveRoll}).";
                        result += $"\n{choices[i].TakeDamage(damage, DamageType.Force, false)}";
                    }
                } else if(i == 4)
                {
                    if (choices[i].SavingThrow(14, "Dexterity", out saveRoll))
                    {
                        damage = RollD6(6, "normal") / 2;
                        result += $"\n{Name} releases a fire ray, but {choices[i].Name}'s quickness " +
                            $"allowed only less sever singes ({saveRoll}).";
                        result += $"\n{choices[i].TakeDamage(damage, DamageType.Fire, false)}";
                    } else
                    {
                        damage = RollD6(6, "normal");
                        result += $"\n{Name} unleashed a fire ray, thoroughly scorching " +
                            $"{choices[i].Name} ({saveRoll}).";
                        result += $"\n{choices[i].TakeDamage(damage, DamageType.Fire, false)}";
                    }
                } else if(i == 5)
                {
                    if (choices[i].SavingThrow(14, "Constitution", out saveRoll))
                    {
                        result += $"\n{Name} fires a paralyzing ray, but {choices[i].Name} resisted its power "
                            + $"({saveRoll}).";
                    } else
                    {
                        choices[i].AddCon(new Condition("paralyzed", 10, this, choices[i], true, 14, "Constitution"));
                        result += $"\n{Name} fires a paralyzing ray, sucessfully afflicting {choices[i].Name} with "
                            + $"the PARALYZED condition ({saveRoll}).";
                    }
                } else
                {
                    if (choices[i].SavingThrow(14, "Dexterity", out saveRoll))
                    {
                        damage = RollD6(8, "normal") / 2;
                        result += $"\n{Name} unveils a Death ray upon {choices[i].Name} though " +
                            $"their agility handicaps their loss of lifeforce ({saveRoll}).";
                        result += $"\n{choices[i].TakeDamage(damage, DamageType.Necrotic, false)}";
                    } else
                    {
                        damage = RollD6(8, "normal");
                        result += $"\n{Name} unveils a Death ray upon {choices[i].Name}, " +
                            $"wracking their soul with an onslaught of negative energy ({saveRoll}).";
                        result += $"\n{choices[i].TakeDamage(damage, DamageType.Necrotic, false)}";
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Allows the Eyedrake to intelligently assign targets based on what 
        /// eye rays they have to use. One creature can be a target multiple times.
        /// </summary>
        /// <param name="beams">The assigned ray types</param>
        /// <param name="targets">All viable targets</param>
        /// <returns>A character array with each index corresponding to a beam.</returns>
        public CommonCharacter[] AssignTargets(int[] beams, CommonCharacter[] targets)
        {
            CommonCharacter[] choices = new CommonCharacter[3];
            for(int i = 0; i < beams.Length; i++)
            {
                if (beams[i] == 4 || beams[i] == 6)
                {
                    CommonCharacter pick = FindArmoured(targets);
                    choices[i] = pick;
                } else if(Health < (maxHealth / 2) && beams[i] == 5)
                {
                    CommonCharacter pick = FindHealthiest(targets);
                    choices[i] = pick;
                } else if (beams[i] == 3)
                {
                    CommonCharacter pick = FindStrongest(targets);
                    choices[i] = pick;
                } else
                {
                    CommonCharacter pick = targets[Next(0, targets.Length)];
                    choices[i] = pick;
                }
            }
            return choices;
        }

        /// <summary>
        /// Simulates the Anti-Magic breath. Selects 5 targets (or as close to that as it can get), 
        /// prioritizing the enemies with the highest AC and/or Strength score.
        /// </summary>
        /// <param name="targets">All viable targets. Up to 5 will be damaged.</param>
        /// <returns>String summarizing result</returns>
        public string AntiMagicBreath(CommonCharacter[] targets)
        {
            CommonCharacter[] victims = new CommonCharacter[5];
            if(victims.Length > targets.Length) { victims = targets; }
            else
            {
                victims[0] = FindArmoured(targets);
                if (!ReferenceEquals(victims[0], FindStrongest(targets)))
                {
                    victims[1] = FindStrongest(targets);
                }

                for (int i = 1; i < victims.Length; i++)
                {
                    if (victims[i] == null)
                    {
                        CommonCharacter pick = targets[Next(0, targets.Length)];
                        while (pick == this || victims.Contains(pick))
                        {
                            pick = targets[Next(0, targets.Length)];
                        }
                        victims[i] = pick;
                    }
                }
            }

            string result = $"{Name} uses Anti-Magic Breath!";
            for(int i = 0; i < victims.Length; i++)
            {
                string save; int damage;
                if (victims[i].SavingThrow(14, "Constitution", out save))
                {
                    damage = RollD12(6, "normal") / 2;
                    result += $"\n{victims[i].Name} endured the onslaught of magic ({save})";
                    result += $"\n{victims[i].TakeDamage(damage, DamageType.Force, false)}\n";
                } else
                {
                    damage = RollD12(6, "normal");
                    result += $"\n{victims[i].Name} took the blast head on (not in a good way).";
                    result += $"\n{victims[i].TakeDamage(damage, DamageType.Force, false)}\n";
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the enemy with the most remaining health points. 
        /// </summary>
        /// <param name="targets">All viable targets</param>
        /// <returns>Character with the highest current Health value.</returns>
        public CommonCharacter FindHealthiest(CommonCharacter[] targets)
        {
            CommonCharacter tankiest = targets[0];
            foreach (CommonCharacter c in targets)
            {
                if (c.Health > tankiest.Health)
                {
                    tankiest = c;
                }
            }
            return tankiest;
        }
        /// <summary>
        /// Finds the enemy with the highest strength score (not modifier)
        /// </summary>
        /// <param name="targets">viable targets</param>
        /// <returns>The enemy object with the highest strength score.</returns>
        public CommonCharacter FindStrongest(CommonCharacter[] targets)
        {
            CommonCharacter strongest = targets[0];
            for(int i = 0; i < targets.Length; i++)
            {
                if (targets[i].abilities["Strength"] > strongest.abilities["Strength"])
                {
                    strongest = targets[i];
                }
            }
            return strongest;
        }

        /// <summary>
        /// Finds the enemy with the highest AC
        /// </summary>
        /// <param name="targets">viable targets</param>
        /// <returns>The enemy object with the highest AC</returns>
        public CommonCharacter FindArmoured(CommonCharacter[] targets)
        {
            CommonCharacter armoured = targets[0];
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].AC > armoured.AC)
                {
                    armoured = targets[i];
                }
            }
            return armoured;
        }
        /// <summary>
        /// Sadly it seems that these guys in lore don't really flee either. 
        /// Gues I'll have to choose mosnters smarter next time . . .
        /// </summary>
        public override bool ReadyToFlee
        {
            get { return false; }
        }

        public override string ToString()
        {
            return $"{Name}, an Eyedrake: \n{base.ToString()}";
        }
    }
}
