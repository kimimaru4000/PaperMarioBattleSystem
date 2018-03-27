using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    #region Enums

    /// <summary>
    /// The result of elemental damage dealt on an entity based on its weaknesses and/or resistances
    /// </summary>
    public enum ElementInteractionResult
    {
        Damage, KO, Heal
    }

    /// <summary>
    /// The ways to handle weaknesses
    /// </summary>
    public enum WeaknessTypes
    {
        None, PlusDamage, KO
    }

    /// <summary>
    /// The ways to handle resistances
    /// </summary>
    public enum ResistanceTypes
    {
        None, MinusDamage, NoDamage, Heal
    }

    /// <summary>
    /// The types of collision shapes.
    /// </summary>
    public enum CollisionShapeTypes
    {
        None,
        Rectangle,
        Circle
    }

    #endregion

    #region Structs

    /// <summary>
    /// Holds immutable data about a collision.
    /// </summary>
    public struct CollisionResponseHolder
    {
        /// <summary>
        /// The object that provided the collision data.
        /// </summary>
        public ICollisionHandler ResponseObj { get; private set; }

        /// <summary>
        /// The data for the collision.
        /// </summary>
        public object CollisionData { get; private set; }

        public CollisionResponseHolder(ICollisionHandler responseObj, object collisionData)
        {
            ResponseObj = responseObj;
            CollisionData = collisionData;
        }
    }

    public struct WeaknessHolder
    {
        public WeaknessTypes WeaknessType;
        public int Value;

        public static WeaknessHolder Default => new WeaknessHolder(WeaknessTypes.None, 0);

        public WeaknessHolder(WeaknessTypes weaknessType, int value)
        {
            WeaknessType = weaknessType;
            Value = value;
        }

        #region Overloaded Operators

        public override bool Equals(object obj)
        {
            return (obj is WeaknessHolder) && this == (WeaknessHolder)obj;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                hash = (hash * 25) + WeaknessType.GetHashCode();
                hash = (hash * 25) + Value.GetHashCode();
                return hash;
            }
        }

        public static bool operator==(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1.WeaknessType == holder2.WeaknessType && holder1.Value == holder2.Value);
        }

        public static bool operator!=(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1.WeaknessType != holder2.WeaknessType || holder1.Value != holder2.Value);
        }

        public static bool operator>(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1.WeaknessType > holder2.WeaknessType || (holder1.WeaknessType == holder2.WeaknessType && holder1.Value > holder2.Value));
        }

        public static bool operator<(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1.WeaknessType < holder2.WeaknessType || (holder1.WeaknessType == holder2.WeaknessType && holder1.Value < holder2.Value));
        }

        public static bool operator>=(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1 > holder2 || holder1 == holder2);
        }

        public static bool operator<=(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1 < holder2 || holder1 == holder2);
        }

        #endregion
    }

    public struct ResistanceHolder
    {
        public ResistanceTypes ResistanceType;
        public int Value;

        public static ResistanceHolder Default => new ResistanceHolder(ResistanceTypes.None, 0);

        public ResistanceHolder(ResistanceTypes resistanceType, int value)
        {
            ResistanceType = resistanceType;
            Value = value;
        }

        #region Overloaded Operators

        public override bool Equals(object obj)
        {
            return (obj is ResistanceHolder) && this == (ResistanceHolder)obj;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 20;
                hash = (hash * 26) + ResistanceType.GetHashCode();
                hash = (hash * 26) + Value.GetHashCode();
                return hash;
            }
        }

        public static bool operator==(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1.ResistanceType == holder2.ResistanceType && holder1.Value == holder2.Value);
        }

        public static bool operator!=(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1.ResistanceType != holder2.ResistanceType || holder1.Value != holder2.Value);
        }

        public static bool operator>(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1.ResistanceType > holder2.ResistanceType || (holder1.ResistanceType == holder2.ResistanceType && holder1.Value > holder2.Value));
        }

        public static bool operator<(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1.ResistanceType < holder2.ResistanceType || (holder1.ResistanceType == holder2.ResistanceType && holder1.Value < holder2.Value));
        }

        public static bool operator>=(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1 > holder2 || holder1 == holder2);
        }

        public static bool operator<=(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1 < holder2 || holder1 == holder2);
        }

        #endregion
    }

    public struct StrengthHolder
    {
        public int Value;

        public static StrengthHolder Default => new StrengthHolder(0);

        public StrengthHolder(int value)
        {
            Value = value;
        }
    }

    public struct ContactResultInfo
    {
        public StatusGlobals.PaybackHolder Paybackholder;
        public Enumerations.ContactResult ContactResult;

        public static ContactResultInfo Default => 
            new ContactResultInfo(StatusGlobals.PaybackHolder.Default, Enumerations.ContactResult.Success);

        public ContactResultInfo(StatusGlobals.PaybackHolder paybackHolder, Enumerations.ContactResult contactResult)
        {
            Paybackholder = paybackHolder;
            ContactResult = contactResult;
        }
    }

    /// <summary>
    /// Holds immutable data for an Element Override.
    /// </summary>
    public struct ElementOverrideHolder
    {
        /// <summary>
        /// The type of elemental damage dealt.
        /// </summary>
        public Enumerations.Elements Element { get; private set; }

        /// <summary>
        /// How many overrides of this Element exist.
        /// </summary>
        public int OverrideCount { get; private set; }

        public static ElementOverrideHolder Default => new ElementOverrideHolder(Enumerations.Elements.Invalid, 0);

        public ElementOverrideHolder(Enumerations.Elements element, int overrideCount)
        {
            Element = element;
            OverrideCount = overrideCount;
        }
    }

    /// <summary>
    /// Holds immutable data for a StatusProperty.
    /// </summary>
    public struct StatusPropertyHolder
    {
        /// <summary>
        /// The likelihood of being afflicted by a StatusEffect-inducing move. This value cannot be lower than 0.
        /// </summary>
        public double StatusPercentage { get; private set; }

        /// <summary>
        /// The number of turns to add onto the StatusEffect's base duration. This can be negative to reduce the duration.
        /// </summary>
        public int AdditionalTurns { get; private set; }

        /// <summary>
        /// The entity's immunity to the StatusEffect.
        /// If this is greater than 0, the entity is immune to the StatusEffect.
        /// <para>This is an int because there may be many sources of adding and removing immunity.</para>
        /// </summary>
        public int Immunity { get; private set; }

        /// <summary>
        /// Whether the entity is currently immune to the StatusEffect or not.
        /// </summary>
        public bool IsImmune => (Immunity > 0);

        public static StatusPropertyHolder Default => new StatusPropertyHolder(100d, 0);

        public StatusPropertyHolder(double statusPercentage, int additionalTurns)
        {
            StatusPercentage = UtilityGlobals.Clamp(statusPercentage, 0, double.MaxValue);
            AdditionalTurns = additionalTurns;
            Immunity = 0;
        }

        public StatusPropertyHolder(double statusPercentage, int additionalTurns, int immunity) : this (statusPercentage, additionalTurns)
        {
            Immunity = immunity;
        }
    }

    /// <summary>
    /// Holds immutable data for elemental damage
    /// </summary>
    public struct ElementDamageHolder
    {
        /// <summary>
        /// The damage dealt
        /// </summary>
        public int Damage { get; private set; }

        /// <summary>
        /// The type of Elemental damage dealt
        /// </summary>
        public Enumerations.Elements Element { get; private set; }

        public static ElementDamageHolder Default => new ElementDamageHolder(0, Enumerations.Elements.Normal);

        public ElementDamageHolder(int damage, Enumerations.Elements element)
        {
            Damage = damage;
            Element = element;
        }
    }

    /// <summary>
    /// Holds immutable data regarding a chance at inflicting a particular StatusEffect.
    /// </summary>
    public struct StatusChanceHolder
    {
        /// <summary>
        /// The percentage of inflicting the StatusEffect.
        /// </summary>
        public double Percentage { get; private set; }

        /// <summary>
        /// The StatusEffect to inflict.
        /// </summary>
        public StatusEffect Status { get; private set; }

        public StatusChanceHolder(double percentage, StatusEffect status)
        {
            Percentage = percentage;
            Status = status;
        }
    }

    /// <summary>
    /// Holds the required data for initiating a damage interaction.
    /// This is passed to methods that involve calculating damage interactions.
    /// </summary>
    
    //NOTE: Consider putting a DamageData in here so new fields added there don't need to be added here as well
    public struct InteractionParamHolder
    {
        public BattleEntity Attacker { get; private set; }
        public BattleEntity Victim { get; private set; }
        public int Damage { get; private set; }
        public Enumerations.Elements DamagingElement { get; private set; }
        public bool Piercing { get; private set; }
        public Enumerations.ContactTypes ContactType { get; private set; }
        public Enumerations.ContactProperties ContactProperty { get; private set; }
        public StatusChanceHolder[] Statuses { get; private set;}
        public Enumerations.DamageEffects DamageEffect { get; private set; }
        public bool CantMiss { get; private set; }
        public Enumerations.DefensiveActionTypes DefensiveOverride { get; private set; }

        public InteractionParamHolder(BattleEntity attacker, BattleEntity victim, int damage, Enumerations.Elements element,
            bool piercing, Enumerations.ContactTypes contactType, Enumerations.ContactProperties contactProperty,
            StatusChanceHolder[] statuses, Enumerations.DamageEffects damageEffect, bool cantMiss,
            Enumerations.DefensiveActionTypes defensiveOverride)
        {
            Attacker = attacker;
            Victim = victim;
            Damage = damage;
            DamagingElement = element;
            Piercing = piercing;
            ContactType = contactType;
            ContactProperty = contactProperty;
            Statuses = statuses;
            DamageEffect = damageEffect;
            CantMiss = cantMiss;
            DefensiveOverride = defensiveOverride;
        }
    }

    /// <summary>
    /// Holds data for the result of a damage interaction.
    /// It includes the BattleEntity that got damaged, the amount and type of damage dealt, the Status Effects inflicted, and more.
    /// </summary>
    public struct InteractionHolder
    {
        /// <summary>
        /// The BattleEntity that got damaged.
        /// </summary>
        public BattleEntity Entity;
        public int TotalDamage;
        public Enumerations.Elements DamageElement;
        public ElementInteractionResult ElementResult;
        public Enumerations.ContactTypes ContactType;
        public Enumerations.ContactProperties ContactProperty;
        public bool Piercing;
        //NOTE: This had to be changed to a StatusChanceHolder since you can now specify the chance of inflicting one
        //I wanted to keep it a StatusEffect array, but I'll keep this since knowing the chance of it being inflicted is useful
        public StatusChanceHolder[] StatusesInflicted;
        public bool Hit;
        public Enumerations.DamageEffects DamageEffect;

        /// <summary>
        /// The types of DefensiveActions performed in this interaction, if any.
        /// </summary>
        public Enumerations.DefensiveActionTypes DefensiveActionsPerformed;

        /// <summary>
        /// Indicates if the damage to be dealt is from Payback.
        /// </summary>
        public bool IsPaybackDamage;

        /// <summary>
        /// Tells whether the BattleEntity in this data should take damage at the end of the interaction.
        /// </summary>
        public bool DontDamageEntity;

        public static InteractionHolder Default => new InteractionHolder(null, 0, Enumerations.Elements.Normal,
            ElementInteractionResult.Damage, Enumerations.ContactTypes.None, Enumerations.ContactProperties.None, false, null, false, Enumerations.DamageEffects.None);

        public InteractionHolder(BattleEntity entity, int totalDamage, Enumerations.Elements damageElement, ElementInteractionResult elementResult,
            Enumerations.ContactTypes contactType, Enumerations.ContactProperties contactProperty, bool piercing, StatusChanceHolder[] statusesInflicted, bool hit, Enumerations.DamageEffects damageEffect)
        {
            Entity = entity;
            TotalDamage = totalDamage;
            DamageElement = damageElement;
            ElementResult = elementResult;
            ContactType = contactType;
            ContactProperty = contactProperty;
            Piercing = piercing;
            StatusesInflicted = statusesInflicted;
            Hit = hit;
            DamageEffect = damageEffect;
            
            DefensiveActionsPerformed = Enumerations.DefensiveActionTypes.None;

            IsPaybackDamage = false;

            DontDamageEntity = false;
        }
    }

    /// <summary>
    /// Holds properties of a MoveAction.
    /// </summary>
    public struct MoveActionData
    {
        public CroppedTexture2D Icon;
        public string Description;
        public Enumerations.MoveResourceTypes ResourceType;
        public float ResourceCost;
        public Enumerations.CostDisplayTypes CostDisplayType;
        public Enumerations.MoveAffectionTypes MoveAffectionType;
        public TargetSelectionMenu.EntitySelectionType SelectionType;
        public bool UsesCharge;
        public Enumerations.HeightStates[] HeightsAffected;

        /// <summary>
        /// Adds these types of BattleEntities to the target list if MoveAffectionType has Other.
        /// The types of BattleEntities are added in the order they appear in the array.
        /// </summary>
        public Enumerations.EntityTypes[] OtherEntTypes;

        public MoveActionData(CroppedTexture2D icon, string description, Enumerations.MoveResourceTypes resourceType,
            float resourceCost, Enumerations.CostDisplayTypes costDisplayType, Enumerations.MoveAffectionTypes moveAffectionType,
            TargetSelectionMenu.EntitySelectionType selectionType, bool usesCharge, Enumerations.HeightStates[] heightsAffected,
            params Enumerations.EntityTypes[] otherEntTypes)
        {
            Icon = icon;
            Description = description;
            ResourceType = resourceType;
            ResourceCost = resourceCost;
            CostDisplayType = costDisplayType;
            MoveAffectionType = moveAffectionType;
            SelectionType = selectionType;
            HeightsAffected = heightsAffected;
            UsesCharge = usesCharge;
            OtherEntTypes = otherEntTypes;
        }
    }

    /// <summary>
    /// Holds data regarding healing, including HP, FP, and Status Effects.
    /// </summary>
    public struct HealingData
    {
        /// <summary>
        /// The amount of HP healed.
        /// </summary>
        public int HPHealed;

        /// <summary>
        /// The amount of FP healed.
        /// </summary>
        public int FPHealed;

        /// <summary>
        /// The StatusEffects healed.
        /// </summary>
        public Enumerations.StatusTypes[] StatusEffectsHealed;

        public static HealingData Default => new HealingData(0, 0, null);

        public HealingData(int hpHealed, int fpHealed, Enumerations.StatusTypes[] statusesHealed)
        {
            HPHealed = hpHealed;
            FPHealed = fpHealed;
            StatusEffectsHealed = statusesHealed;
        }
    }

    /// <summary>
    /// Holds data regarding damage.
    /// </summary>
    public struct DamageData
    {
        public int Damage;
        public Enumerations.Elements DamagingElement;
        public bool Piercing;
        public Enumerations.ContactTypes ContactType;
        public Enumerations.ContactProperties ContactProperty;
        public StatusChanceHolder[] Statuses;
        public bool CantMiss;
        public bool AllOrNothingAffected;
        public Enumerations.DefensiveActionTypes DefensiveOverride;
        public Enumerations.DamageEffects DamageEffect;

        public DamageData(int damage, Enumerations.Elements damagingElement, bool piercing, Enumerations.ContactTypes contactType,
            Enumerations.ContactProperties contactProperty, StatusChanceHolder[] statuses, bool cantMiss, bool allOrNothingAffected,
            Enumerations.DefensiveActionTypes defensiveOverride, Enumerations.DamageEffects damageEffect)
        {
            Damage = damage;
            DamagingElement = damagingElement;
            Piercing = piercing;
            ContactType = contactType;
            ContactProperty = contactProperty;
            Statuses = statuses;
            CantMiss = cantMiss;
            AllOrNothingAffected = allOrNothingAffected;
            DefensiveOverride = defensiveOverride;
            DamageEffect = damageEffect;
        }

        public DamageData(int damage, Enumerations.Elements damagingElement, bool piercing, Enumerations.ContactTypes contactType,
            Enumerations.ContactProperties contactProperty, StatusChanceHolder[] statuses, Enumerations.DamageEffects damageEffect)
            : this(damage, damagingElement, piercing, contactType, contactProperty, statuses, false, true, 
                  Enumerations.DefensiveActionTypes.None, damageEffect)
        {

        }
    }

    #endregion

    #region Classes

    /// <summary>
    /// A class containing the main stats in the game.
    /// Enemies have internal levels, likely related to Star Point gain.
    /// </summary>
    public class Stats
    {
        public int Level;

        //Max stats
        public int MaxHP;
        public int MaxFP;

        //Base stats going into battle

        /// <summary>
        /// Base Attack without any modifications.
        /// </summary>
        public int BaseAttack;

        /// <summary>
        /// Base Defense without any modifications.
        /// </summary>
        public int BaseDefense;

        public int HP;
        public int FP;

        /// <summary>
        /// The Attack amount modified.
        /// </summary>
        public int Attack;

        /// <summary>
        /// The Defense amount modified.
        /// </summary>
        public int Defense;

        /// <summary>
        /// Different from Defense - this modifies the total damage the attack itself does, making it possible to reduce damage dealt
        /// to you from Piercing attacks. This value can be negative.
        /// <para>The P-Up, D-Down and P-Down, D-Up Badges modify this value.</para>
        /// </summary>
        public int DamageReduction;

        /// <summary>
        /// Accuracy; the value is interpreted as the percentage of hitting.
        /// Higher values indicate a greater chance of hitting, and lower values indicate a smaller chance of hitting.
        /// <para>Accuracy stacks multiplicatively.</para>
        /// </summary>
        public int Accuracy = 100;

        /// <summary>
        /// The total Accuracy modifier, recalculated each time one is added or removed. A value of 1 represents the base value.
        /// </summary>
        public double AccuracyMod { get; protected set; } = 1d;

        /// <summary>
        /// The list of Accuracy modifiers.
        /// </summary>
        public readonly List<double> AccuracyModifiers = new List<double>();

        //NOTE: Evasion stacks multiplicatively
        //For example, with 3 Pretty Lucky and 1 Lucky Day badge equipped, the chance of attacks missing is: (0.9)(0.9)(0.9)(0.75) = .54675
        /// <summary>
        /// Evasion; the value is interpreted as the percentage of being hit.
        /// Lower values indicate a lower chance of getting hit, and higher values indicate a higher chance of getting hit.
        /// <para>Evasion stacks multiplicatively.</para>
        /// </summary>
        public int Evasion = 100;

        /// <summary>
        /// The total Evasion modifier, recalculated each time one is added or removed. A value of 1 represents the base value.
        /// </summary>
        public double EvasionMod { get; protected set; } = 1f;

        /// <summary>
        /// The list of Evasion modifiers.
        /// </summary>
        public readonly List<double> EvasionModifiers = new List<double>();

        /// <summary>
        /// Default stats
        /// </summary>
        public static Stats Default => new Stats(1, 10, 5, 0, 0);

        /// <summary>
        /// The BattleEntity's BaseAttack combined with any modifiers.
        /// </summary>
        public int TotalAttack => BaseAttack + Attack;

        /// <summary>
        /// The BattleEntity's BaseDefense combined with any modifiers.
        /// </summary>
        public int TotalDefense => BaseDefense + Defense;

        public Stats(int level, int maxHP, int maxFP, int attack, int defense)
        {
            Level = level;
            MaxHP = HP = maxHP;
            MaxFP = FP = maxFP;
            BaseAttack = attack;
            BaseDefense = defense;
            Attack = 0;
            Defense = 0;
        }

        public double GetTotalAccuracy() => (Accuracy * AccuracyMod);
        public double GetTotalEvasion() => (Evasion * EvasionMod);

        /// <summary>
        /// Calculates the total Accuracy modifier.
        /// </summary>
        public void CalculateTotalAccuracyMod()
        {
            double totalAccuracyMod = 1d;

            for (int i = 0; i < AccuracyModifiers.Count; i++)
            {
                totalAccuracyMod *= AccuracyModifiers[i];
            }

            AccuracyMod = totalAccuracyMod;
        }

        /// <summary>
        /// Calculates the total Evasion modifier.
        /// </summary>
        public void CalculateTotalEvasionMod()
        {
            double totalEvasionMod = 1d;

            for (int i = 0; i < EvasionModifiers.Count; i++)
            {
                totalEvasionMod *= EvasionModifiers[i];
            }

            EvasionMod = totalEvasionMod;
        }
    }

    /// <summary>
    /// Stats for Mario's Partners.
    /// </summary>
    public sealed class PartnerStats : Stats
    {
        public PartnerGlobals.PartnerRanks PartnerRank = PartnerGlobals.PartnerRanks.Normal;

        public PartnerStats(PartnerGlobals.PartnerRanks partnerRank, int maxHP, int attack, int defense)
            : base((int)partnerRank, maxHP, 0, attack, defense)
        {
            PartnerRank = partnerRank;
        }
    }

    /// <summary>
    /// Stats for Mario.
    /// </summary>
    public sealed class MarioStats : Stats
    {
        /// <summary>
        /// The level of Mario's Boots
        /// </summary>
        public EquipmentGlobals.BootLevels BootLevel = EquipmentGlobals.BootLevels.Normal;

        /// <summary>
        /// The level of Mario's Hammer
        /// </summary>
        public EquipmentGlobals.HammerLevels HammerLevel = EquipmentGlobals.HammerLevels.Normal;

        /// <summary>
        /// Mario's Star Spirit Star Power.
        /// </summary>
        public StarSpiritPower SSStarPower = new StarSpiritPower();

        /// <summary>
        /// Mario's Crystal Star Power.
        /// </summary>
        public CrystalStarPower CSStarPower = new CrystalStarPower();

        /// <summary>
        /// The number of Star Points Mario has.
        /// When it reaches 100, it resets back to 0 and Mario goes up one level.
        /// </summary>
        public int StarPoints = 0;

        public MarioStats(int level, int maxHp, int maxFP, int attack, int defense,
            EquipmentGlobals.BootLevels bootLevel, EquipmentGlobals.HammerLevels hammerLevel) : base(level, maxHp, maxFP, attack, defense)
        {
            BootLevel = bootLevel;
            HammerLevel = hammerLevel;
        }

        /// <summary>
        /// Retrieves the type of Star Power based on a given StarPowerTypes.
        /// </summary>
        /// <param name="starPowerType">The type of Star Power to get.</param>
        /// <returns>StarSpiritPower or CrystalStarPower if the respective type is passed in. Otherwise, it returns null.</returns>
        public StarPowerBase GetStarPowerFromType(StarPowerGlobals.StarPowerTypes starPowerType)
        {
            if (starPowerType == StarPowerGlobals.StarPowerTypes.StarSpirit)
            {
                return SSStarPower;
            }
            else if (starPowerType == StarPowerGlobals.StarPowerTypes.CrystalStar)
            {
                return CSStarPower;
            }

            return null;
        }
    }

    /// <summary>
    /// The final result of an interaction, containing InteractionHolders for both the attacker and victim
    /// </summary>
    public sealed class InteractionResult
    {
        public InteractionHolder AttackerResult = InteractionHolder.Default;
        public InteractionHolder VictimResult = InteractionHolder.Default;

        public InteractionResult()
        {
            
        }

        public InteractionResult(InteractionResult copy)
        {
            AttackerResult = copy.AttackerResult;
            VictimResult = copy.VictimResult;
        }

        public InteractionResult(InteractionHolder attackerResult, InteractionHolder victimResult)
        {
            AttackerResult = attackerResult;
            VictimResult = victimResult;
        }

        #region Helper Properties & Methods

        /// <summary>
        /// Tells if the Victim in the interaction was hit.
        /// </summary>
        public bool WasVictimHit => (VictimResult.DontDamageEntity == false && VictimResult.Hit == true);

        /// <summary>
        /// Tells if the Attacker in the interaction was hit.
        /// </summary>
        public bool WasAttackerHit => (AttackerResult.DontDamageEntity == false && AttackerResult.Hit == true);

        #endregion
    }

    #endregion

    /// <summary>
    /// Class for general global values and references
    /// </summary>
    public static class GeneralGlobals
    {
        /// <summary>
        /// The value that is used in random conditions. If this is less than the result, it returns true.
        /// </summary>
        public const int RandomConditionVal = 100;

        /// <summary>
        /// Random reference for generating pseudo-random numbers.
        /// </summary>
        public static readonly Random Randomizer = new Random();

        public static double GenerateRandomDouble() => (Randomizer.NextDouble() * RandomConditionVal);
        public static int GenerateRandomInt() => Randomizer.Next(RandomConditionVal);
    }

    /// <summary>
    /// Class for global values dealing with Action Commands.
    /// </summary>
    public static class ActionCommandGlobals
    {
        #region Structs

        /// <summary>
        /// A struct holding information Art Attack's Action Command sends.
        /// </summary>
        public struct ArtAttackResponse
        {
            public Rectangle BoundingBox;

            public ArtAttackResponse(Rectangle boundingBox)
            {
                BoundingBox = boundingBox;
            }
        }

        /// <summary>
        /// A struct holding information Power Lift's Action Command sends.
        /// </summary>
        public struct PowerLiftResponse
        {
            public int AttackBoosted;
            public int DefenseBoosted;

            public PowerLiftResponse(int attackBoosted, int defenseBoosted)
            {
                AttackBoosted = attackBoosted;
                DefenseBoosted = defenseBoosted;
            }

            public override string ToString()
            {
                return $"{nameof(PowerLiftResponse)} - Attack: {AttackBoosted}, Defense: {DefenseBoosted}";
            }
        }

        /// <summary>
        /// A struct holding information Sweet Treat and Sweet Feast's Action Commands send.
        /// </summary>
        public struct SweetTreatResponse
        {
            public int MarioHPRestored;
            public int PartnerHPRestored;
            public int FPRestored;

            public SweetTreatResponse(int marioHPRestored, int partnerHPRestored, int fpRestored)
            {
                MarioHPRestored = marioHPRestored;
                PartnerHPRestored = partnerHPRestored;
                FPRestored = fpRestored;
            }
        }

        /// <summary>
        /// A struct holding information Bomb Squad's Action Command sends.
        /// </summary>
        public struct BombSquadResponse
        {
            public Vector2 ThrowVelocity;
            public float Gravity;

            public BombSquadResponse(Vector2 throwVelocity, float gravity)
            {
                ThrowVelocity = throwVelocity;
                Gravity = gravity;
            }
        }

        /// <summary>
        /// A struct holding information Shell Shield's Action Command sends.
        /// </summary>
        public struct ShellShieldResponse
        {
            public int HP;
            public int MaxHP;

            public ShellShieldResponse(int hp, int maxHP)
            {
                HP = hp;
                MaxHP = maxHP;
            }
        }

        /// <summary>
        /// Defines the range in a bar and the value and CommandRank associated with that range.
        /// </summary>
        public struct BarRangeData
        {
            /// <summary>
            /// The start range on the bar. This is inclusive.
            /// </summary>
            public float StartBarVal;

            /// <summary>
            /// The end range on the bar. This is exclusive.
            /// </summary>
            public float EndBarVal;

            /// <summary>
            /// The value associated with the range.
            /// </summary>
            public int Value;

            /// <summary>
            /// The CommandRank associated with the range.
            /// </summary>
            public ActionCommand.CommandRank Rank;

            /// <summary>
            /// The Color of the segment of the bar in this range.
            /// </summary>
            public Color SegmentColor;

            public BarRangeData(float startBarVal, float endBarVal, int value, ActionCommand.CommandRank rank, Color segmentColor)
            {
                StartBarVal = startBarVal;
                EndBarVal = endBarVal;
                Value = value;
                Rank = rank;
                SegmentColor = segmentColor;
            }

            /// <summary>
            /// Tells if a value is in the bar's range.
            /// </summary>
            /// <param name="value">The value to test.</param>
            /// <returns>true if <paramref name="value"/> is greater than or equal to StartBarVal and less than EndBarVal.</returns>
            public bool IsValueInRange(float value)
            {
                return (value >= StartBarVal && value < EndBarVal);
            }
        }

        #endregion
    }

    /// <summary>
    /// Global values dealing with Sweet Treat.
    /// </summary>
    public static class SweetTreatGlobals
    {
        #region Enums

        /// <summary>
        /// The types of restoration used in Sweet Treat and Sweet Feast.
        /// </summary>
        public enum RestoreTypes
        {
            None,
            MarioHP,
            PartnerHP,
            FP,
            PoisonMushroom,
            BigMarioHP,
            BigPartnerHP,
            BigFP
        }

        /// <summary>
        /// The behaviors for the RestoreTypes.
        /// </summary>
        public enum RestoreBehavior
        {
            None,
            HealMarioHP,
            HealPartnerHP,
            HealFP,
            PreventInput
        }

        #endregion

        #region RestorationTable

        public static readonly Dictionary<RestoreTypes, RestoreBehaviorData> RestorationTable = new Dictionary<RestoreTypes, RestoreBehaviorData>()
        {
            { RestoreTypes.MarioHP, new RestoreBehaviorData(RestoreBehavior.HealMarioHP, 1) },
            { RestoreTypes.PartnerHP, new RestoreBehaviorData(RestoreBehavior.HealPartnerHP, 1) },
            { RestoreTypes.FP, new RestoreBehaviorData(RestoreBehavior.HealFP, 1) },
            { RestoreTypes.BigMarioHP, new RestoreBehaviorData(RestoreBehavior.HealMarioHP, 3) },
            { RestoreTypes.BigPartnerHP, new RestoreBehaviorData(RestoreBehavior.HealPartnerHP, 3) },
            { RestoreTypes.BigFP, new RestoreBehaviorData(RestoreBehavior.HealFP, 3) },
            { RestoreTypes.PoisonMushroom, new RestoreBehaviorData(RestoreBehavior.PreventInput, 800) }
        };

        #endregion

        #region Structs

        /// <summary>
        /// Holds immutable data regarding a RestoreBehavior and a value.
        /// </summary>
        public struct RestoreBehaviorData
        {
            public RestoreBehavior Behavior { get; private set; }
            public int Value { get; private set; }

            public static RestoreBehaviorData Default => new RestoreBehaviorData(RestoreBehavior.None, 0);

            public RestoreBehaviorData(RestoreBehavior behavior, int value)
            {
                Behavior = behavior;
                Value = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Class for global values dealing with PhysicalAttributes.
    /// </summary>
    public static class PhysAttributeGlobals
    {
        #region Classes

        /// <summary>
        /// Compares PhysicalAttributes by their values.
        /// </summary>
        public class PhysAttributeComparer : IComparer<Enumerations.PhysicalAttributes>
        {
            /// <summary>
            /// An IComparer method used to sort PhysicalAttributes by their Priorities.
            /// </summary>
            /// <param name="physAttr1">The first PhysicalAttribute to compare.</param>
            /// <param name="physAttr2">The second PhysicalAttribute to compare.</param>
            /// <returns>-1 if status1 has a higher value, 1 if physAttribute2 has a higher value, 0 if they have the same values.</returns>
            public int Compare(Enumerations.PhysicalAttributes physAttr1, Enumerations.PhysicalAttributes physAttr2)
            {
                return SortPhysicalAttributes(physAttr1, physAttr2);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// A Comparison sort method for PhysicalAttributes, putting higher valued attributes first for consistency with contact results
        /// </summary>
        /// <param name="physAttr1"></param>
        /// <param name="physAttr2"></param>
        /// <returns></returns>
        public static int SortPhysicalAttributes(Enumerations.PhysicalAttributes physAttr1, Enumerations.PhysicalAttributes physAttr2)
        {
            if (physAttr1 > physAttr2)
                return -1;
            else if (physAttr1 < physAttr2)
                return 1;

            return 0;
        }

        #endregion
    }

    /// <summary>
    /// Class for global values dealing with Elements.
    /// </summary>
    public static class ElementGlobals
    {
        #region Classes

        /// <summary>
        /// Compares Elements by their values.
        /// </summary>
        public class ElementComparer : IComparer<Enumerations.Elements>
        {
            /// <summary>
            /// An IComparer method used to sort Elements by their Priorities.
            /// </summary>
            /// <param name="element1">The first Element to compare.</param>
            /// <param name="element2">The second Element to compare.</param>
            /// <returns>-1 if status1 has a higher value, 1 if physAttribute2 has a higher value, 0 if they have the same value.</returns>
            public int Compare(Enumerations.Elements element1, Enumerations.Elements element2)
            {
                return SortElements(element1, element2);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// A Comparison sort method for Elements, putting higher valued elements first.
        /// </summary>
        /// <param name="element1"></param>
        /// <param name="element2"></param>
        /// <returns></returns>
        public static int SortElements(Enumerations.Elements element1, Enumerations.Elements element2)
        {
            if (element1 > element2)
                return -1;
            else if (element1 < element2)
                return 1;

            return 0;
        }

        #endregion
    }

    /// <summary>
    /// Class for global values dealing with Partners.
    /// </summary>
    public static class PartnerGlobals
    {
        #region Enums

        /// <summary>
        /// The Ranks for Partners.
        /// </summary>
        public enum PartnerRanks
        {
            Normal = 1, Super = 2, Ultra = 3
        }
        
        /// <summary>
        /// The colors Yoshi can be.
        /// </summary>
        public enum YoshiColors
        {
            Green, Red, Blue, Orange, Pink, Black, White
        }

        #endregion
    }

    /// <summary>
    /// Class for global values dealing with general equipment
    /// </summary>
    public static class EquipmentGlobals
    {
        #region Enums

        /// <summary>
        /// The types of Boot levels for Mario.
        /// </summary>
        public enum BootLevels
        {
            Normal = 1, Super = 2, Ultra = 3
        }

        /// <summary>
        /// The types of Hammer levels for Mario.
        /// </summary>
        public enum HammerLevels
        {
            Normal = 1, Super = 2, Ultra = 3
        }

        #endregion
    }

    public static class AudioGlobals
    {
        
    }
}
