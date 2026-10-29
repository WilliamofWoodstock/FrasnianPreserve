using System.Xml.Linq;
using static DNDBattler.Helper;

namespace DNDBattler
{
    enum Conditions
    {
        Poisoned = 1,
        Paralyzed,
        Frightened,
        Incapacitated
    }

    internal class Condition
    {
        public Conditions Type { get; }
        public int Duration { get; set; }
        public CommonCharacter Source { get; }
        public CommonCharacter Target { get; }
        public bool RepeatSave { get; }
        public int SaveDC { get; }
        public string SaveAbility { get; }

        /// <summary>
        /// "Default" constructor in which it is assumed that no additional saves are done.
        /// </summary>
        /// <param name="type">string corresponding to condition type (case insensitive)</param>
        /// <param name="duration">how many rounds the condition lasts. D</param>
        /// <param name="source">Character who caused the condition.</param>
        /// <param name="target">CHaracter being afflicted by this condition.</param>
        /// <exception cref="ArgumentException">If duration is less than 2.</exception>
        public Condition(string type, int duration, CommonCharacter source, CommonCharacter target)
        {
            if (duration <= 0)
            {
                throw new ArgumentException("Duration must be at least 2", $"Duration: {duration}");
            }
            Type = ConConvert(type);
            Duration = duration;
            Source = source; Target = target;
            SaveDC = 0;
            SaveAbility = "None";
            if(source is BeholderZombie && type.EqualsIgnoreCase("Poisoned"))
            {
                target.CantHeal = true;
            }
        }

        /// <summary>
        /// Alternate constructor, in practice only used if the target can roll a save at the end of each 
        /// round.
        /// </summary>
        /// <param name="type">string corresponding to condition type (case insensitive)</param>
        /// <param name="duration">how many rounds the condition lasts. D</param>
        /// <param name="source">Character who caused the condition.</param>
        /// <param name="target">Character being afflicted by this condition.</param>
        /// <param name="repeatable">Whether target can roll a save to </param>
        /// <param name="saveDC">Minimum roll req to end condition</param>
        /// <param name="saveAbility">Ability modifier used.</param>
        /// <exception cref="ArgumentException">IF duration is less than 2.</exception>
        public Condition(string type, int duration, CommonCharacter source, CommonCharacter target,
            bool repeatable, int saveDC, string saveAbility)
        {
            if(duration <= 0)
            {
                throw new ArgumentException("Duration must be at least 2", $"Duration: {duration}");
            }
            Type = ConConvert(type);
            Duration = duration;
            Source = source; Target = target;
            RepeatSave = repeatable;
            SaveDC = saveDC; SaveAbility = saveAbility;
            if (source is BeholderZombie && type.EqualsIgnoreCase("Poisoned"))
            {
                target.CantHeal = true;
            }
        }

        /// <summary>
        /// Always call before ending a condition, especially if the condition had extra rules 
        /// due to its source.
        /// </summary>
        /// <returns>Summarizes result of condition ending</returns>
        public string End()
        {
            string result = $"{Target.Name} shook off {Type}.";
            if(Type == Conditions.Poisoned && Source is BeholderZombie)
            {
                result += " They've regained the ability to heal";
                Target.CantHeal = false;
            }
            return result;
        }
    }
}
