using static DNDBattler.DiceRoller;
using static DNDBattler.Helper;

namespace DNDBattler
{
    internal class BeholderZombie : Zombie
    {
        public BeholderZombie(string name) :
        base(name, "Beholder Zombie", 93, 15, 14, 8, 16, 3, 8, 5, 
            new string[] { "None" }, new string[] { "Poison" }, new string[] { "none" }, 
            new int[] { 0, 0, 0, 0, 2, 0 })
        {

        }

        /// <summary>
        /// randomly chooses a target then decides between biting one target or Eye beaming 2.
        /// </summary>
        /// <param name="targets">List of all possible targets.</param>
        /// <returns>The result of the attack (miss, hit, etc)</returns>
        public override string Attack(CommonCharacter[] targets)
        {
            int decision = RollD4();
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
                    (target.Is(Conditions.Paralyzed) && target.AC <= attackRoll + 5))
                {
                    int damage = RollD6(8, "Normal") + 2;
                    string result = target.TakeDamage(damage, DirectConvert("Piercing"), true);
                    return $"{Name} lands a critical bite on {target.Name}. {result}";
                }
                else if (target.AC <= attackRoll + 5)
                {
                    int damage = RollD6(4, "Normal") + 2;
                    string result = target.TakeDamage(damage, DirectConvert("Piercing"), false);
                    return $"{Name} bites {target.Name}. {result}";
                }
                else
                {
                    return $"{Name} attempts to bite {target.Name}, but missed.";
                }
            } else
            {
                string result = EyeRays(targets);
                return result;
            }
            
        }

        /// <summary>
        /// Handles the logic behind the beholder zombie's eye rays attack. The 1 restriction it follows is 
        /// that it can't use two of the same beam type in the same method call.
        /// </summary>
        /// <param name="targets">Array containing all viable targets. Only 1-2 of them will be attacked.</param>
        /// <returns>String summarizing the result of this attack.</returns>
        public string EyeRays(CommonCharacter[] targets)
        {
            //represents the two eyebeams that the zombie beholder chose.
            int[] beams = { RollD4(), RollD4() };
            while (beams[1] == beams[0]) { beams[1] = RollD4(); }
            CommonCharacter[] choices = { targets[Next(0, targets.Length)], targets[Next(0, targets.Length)] };
            string result = "";
            for(int i = 0; i < choices.Length; i++)
            {
                CommonCharacter target = choices[i];
                string saveRoll; int damage;
                if(beams[i] == 1) //paralyzing ray
                {
                    if(target.SavingThrow(14, "Constitution", out saveRoll))
                    {
                        result += $"{Name} fires a paralyzing ray, but {target.Name} resisted its power "
                            + $"({saveRoll}).";
                    } else
                    {
                        target.AddCon(new Condition("paralyzed", 10, this, target, true, 14, "Constitution"));
                        result += $"{Name} fires a paralyzing ray, sucessfully afflicting {target.Name} with "
                            + $"the PARALYZED condition ({saveRoll}).";
                    }
                } else if(beams[i] == 2) //frightening ray
                {
                    if(target.SavingThrow(14, "Wisdom", out saveRoll))
                    {
                        damage = (RollD8(3, "Normal") / 2);
                        result += $"{Name} uses a frightening ray on {target.Name}, but their resolve halves "
                            + $"the damage and keeps them brave ({saveRoll}).";
                    } else
                    {
                        damage = RollD8(3, "Normal");
                        if(target is Paladin)
                        {
                            Paladin pal = (Paladin)target;
                            if(pal.CourageAura)
                            {
                                result += $"{Name} hits {target.Name} with a frightening ray, but " +
                                    $"their Auro of Courage negates all fear.";
                            } else
                            {
                                target.AddCon(new Condition("Frightened", 2, this, target));
                                result += $"{Name} hits {target.Name} with a frightening ray, and they "
                                    + $"become FRIGHTENED" + $" ({saveRoll}).";
                            }
                        } else
                        {
                            target.AddCon(new Condition("Frightened", 2, this, target));
                            result += $"{Name} hits {target.Name} with a frightening ray, and they "
                                + $"become FRIGHTENED" + $" ({saveRoll}).";
                        }
                    }
                    result += $"\n{target.TakeDamage(damage, DamageType.Psychic, false)}";
                } else if(beams[i] == 3) //Enervation ray
                {
                    if(target.SavingThrow(14, "Constitution", out saveRoll))
                    {
                        damage = (RollD6(3, "Normal") / 2);
                        result += $"{Name} shoots an enervation ray, but {target.Name} stays resilient ({saveRoll}).";
                    } else
                    {
                        damage = RollD6(3, "Normal");
                        if (target is Zombie)
                        {
                            result += $"{Name} shoots an enervation ray at {target.Name} with a direct hit ({saveRoll})" +
                                $", but zombies are immune to poison . . .";
                        }
                        else
                        {
                            result += $"{Name} hits {target.Name} with an enervation ray, granting them the "
                            + $"POISONED condition and inhibiting their ability to heal until recovery ({saveRoll}).";
                            target.AddCon(new Condition("Poisoned", 2, this, target));
                        }
                    }
                    result += $"\n{target.TakeDamage(damage, DamageType.Necrotic, false)}";
                } else //disintegration ray
                {
                    if(target.SavingThrow(14, "Dexterity", out saveRoll))
                    {
                        damage = (RollD10(5, "Normal") / 2);
                        result += $"{Name} strikes {target.Name} with a disintegrating ray, but {target.Name} "
                            + "partially dodged (save success), taking only half damage.";
                    }
                    else
                    {
                        damage = RollD10(5, "Normal");
                        result += $"{Name} strikes {target.Name} with a disintegrating ray.";
                    }
                    result += $"\n{target.TakeDamage(damage, DamageType.Force, true)}";
                }
                result += "\n\n";
            }
            return result;
        }
    }
}
