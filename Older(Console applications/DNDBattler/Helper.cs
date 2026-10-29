using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNDBattler
{
    /// <summary>
    /// Filled with various static methods that assist calculations or other needs.
    /// </summary>
    internal static class Helper
    {
        /// <summary>
        /// Akin to its 'single string' version but instead does this for a whole array. will let you know if you gave a 'faulty' 
        /// (One of the values was empty, null, or could not be converted) array via throwing an error.
        /// </summary>
        /// <param name="damageTypes">An array of strings to be converted.</param>
        /// <returns>An array of ints corresponding to the string array, or an empty array with one index if 
        /// you did something wrong.</returns>
        internal static int[] ConvertDamage(string[] damageTypes)
        {
            int[] converted = new int[damageTypes.Length];
            for(int i = 0; i < damageTypes.Length; i++)
            {
                converted[i] = ConvertDamage(damageTypes[i]);
            }
            return converted;
        }

        /// <summary>
        /// Converts a string into an int for the DamageType enum. Exists because Enum.Parse is not case insensitive, which this is. 
        /// Throws an error if parameter doesn't match any of the damage types.
        /// </summary>
        /// <param name="damageType">The string to be converted.</param>
        /// <returns>int between 1-13 if string matches a damage type or -1 if None</returns>
        internal static int ConvertDamage(string damageType)
        {
            if (damageType == null || damageType.Length == 0)
            {
                throw new ArgumentException("Damage type cannot be null or empty", damageType);
            }
            else if (damageType.EqualsIgnoreCase("slashing")) { return 1; }
            else if (damageType.EqualsIgnoreCase("piercing")) { return 2; }
            else if (damageType.EqualsIgnoreCase("bludgeoning")) { return 3; }
            else if (damageType.EqualsIgnoreCase("fire")) { return 4; }
            else if (damageType.EqualsIgnoreCase("Cold")) { return 5; }
            else if (damageType.EqualsIgnoreCase("poison")) { return 6; }
            else if (damageType.EqualsIgnoreCase("lightning")) { return 7; }
            else if (damageType.EqualsIgnoreCase("thunder")) { return 8; }
            else if (damageType.EqualsIgnoreCase("acid")) { return 9; }
            else if (damageType.EqualsIgnoreCase("force")) { return 10; }
            else if (damageType.EqualsIgnoreCase("psychic")) { return 11; }
            else if (damageType.EqualsIgnoreCase("radiant")) { return 12; }
            else if (damageType.EqualsIgnoreCase("necrotic")) { return 13; }
            else if(damageType.EqualsIgnoreCase("None"))
            {
                //Console.WriteLine("None"); 
                return -1;
            }
            else
            {
                throw new ArgumentException("Damage type does not match extant damage type", damageType);
            }
        }

        /// <summary>
        /// Converts a string to a DamageType rather than its Enum index. Throws an error if the param 
        /// is null, empty, or 'None'. Case insensitive.
        /// </summary>
        /// <param name="damageType">Damage type.</param>
        /// <returns>The matching DamageTYpe Enum if the string matches.</returns>
        /// <exception cref="ArgumentException">If damageType is empty, null, or "none"</exception>
        internal static DamageType DirectConvert(string damageType)
        {
            if (damageType == null || damageType.Length == 0 || damageType.EqualsIgnoreCase("None"))
            {
                throw new ArgumentException("Damage type can't be null, empty, or 'None'", damageType);
            } else
            {
                return (DamageType)ConvertDamage(damageType);
            }
        }

        /// <summary>
        /// Extension method that compares two strings case-insensitively (uses OrdinalIgnoreCase)
        /// </summary>
        /// <param name="str1">The string that the method is being called on.</param>
        /// <param name="str2">The string that the string being called on is compared to</param>
        /// <returns>Whether the two strings are case-insensitively equivalent (without cultural aspects)</returns>
        internal static bool EqualsIgnoreCase(this string str1, string str2)
        {
            if (str1.Equals(str2, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets up the attack method by creating an array of all combatants except for the attacker.
        /// </summary>
        /// <param name="attacker">Character that is about to attack</param>
        /// <param name="list">List containing all possible targets + attacker</param>
        /// <returns></returns>
        internal static CommonCharacter[] CreateTargets(this CommonCharacter attacker, List<CommonCharacter> list)
        {
            return list.Where(c => c != attacker).ToArray();
        }

        /// <summary>
        /// Converts a string into a DiceState Enum, useful for rolling. 
        /// Method name stands for Disadvantage, Advantage and Normal.
        /// </summary>
        /// <param name="state">The dice state to be applied</param>
        /// <returns>DiceState enum corresponding to string contents</returns>
        internal static DiceState DAN(string state)
        {
            if(state.EqualsIgnoreCase("disadvantage"))
            {
                return DiceState.Disadvantage;
            } else if(state.EqualsIgnoreCase("advantage"))
            {
                return DiceState.Advantage;
            } else
            {
                return DiceState.Normal;
            }
        }

        /// <summary>
        /// Converts a string into the corresponding Conditions Enum (case insensitive). Throws an error 
        /// if it could not match.
        /// </summary>
        /// <param name="condition">Condition to be converted</param>
        /// <exception cref="ArgumentException">If string doesn't match any condition names.</exception>
        /// <returns>Conditions Enum corresponding to string if a match was found.</returns>
        internal static Conditions ConConvert(string condition)
        {
            if (condition.EqualsIgnoreCase("poisoned")) { return (Conditions)1; }
            else if(condition.EqualsIgnoreCase("paralyzed")) { return (Conditions)2; }
            else if(condition.EqualsIgnoreCase("frightened")) { return (Conditions)3; }
            else if(condition.EqualsIgnoreCase("Incapacitated")) { return (Conditions)4; }
            else { throw new ArgumentException("String must match one of the conditions", condition); }
        }

        /// <summary>
        /// Gets the proficiency bonus based on character's level
        /// </summary>
        /// <param name="level">Character's level.</param>
        /// <returns>Character's proficiency bonus</returns>
        /// <exception cref="ArgumentException">If level is > 20 or <= 0.</exception>
        internal static int GetPB(this int level)
        {
            if (level > 20 || level <= 0)
            {
                throw new ArgumentException("Level has to be 1-20", $"Level: {level}");
            }
            else if (level <= 4) { return 2; }
            else if (level <= 8) { return 3; }
            else if (level <= 12) { return 4; }
            else if (level <= 16) { return 5; }
            else { return 6; }
        }

        /// <summary>
        /// Converts an ability score into a modifier, based on 5e's rules (starts at -5 
        /// , increases by 1 for every two levels, caps out at +10 when ability score is 30). 
        /// Has a failsafe for illegal scores (less than 1 or greater than 30)
        /// </summary>
        /// <param name="stat">Ability score.</param>
        /// <returns></returns>
        internal static int StatToMod(this int stat)
        {
            if(stat > 30) { return 10; }
            else if(stat < 1) { return -5; }
            return (stat - 10) / 2;
        }
    }
}
