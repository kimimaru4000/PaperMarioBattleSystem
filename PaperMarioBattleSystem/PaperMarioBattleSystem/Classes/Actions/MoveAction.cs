using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.TargetSelectionMenu;
using static PaperMarioBattleSystem.Enumerations;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A type of BattleAction selectable from a BattleMenu.
    /// They have sequences that the BattleEntity performs.
    /// </summary>
    public class MoveAction : BattleAction, IActionCommand, IDisableable
    {
        #region Fields/Properties

        public MoveActionData MoveProperties => MoveInfo;

        /// <summary>
        /// The properties of the MoveAction. Includes FP cost and more.
        /// </summary>
        protected MoveActionData MoveInfo;

        /// <summary>
        /// The Sequence this MoveAction has the BattleEntity perform.
        /// </summary>
        public Sequence MoveSequence { get; private set; } = null;

        /// <summary>
        /// The damage information of this MoveAction.
        /// </summary>
        protected DamageData DamageInfo;

        public DamageData DamageProperties => DamageInfo;

        /// <summary>
        /// The healing information of this MoveAction.
        /// </summary>
        public HealingData? HealingInfo { get; protected set; } = null;

        /// <summary>
        /// The category of this action. It is automatically set by the menus.
        /// </summary>
        public MoveCategories MoveCategory { get; private set; } = MoveCategories.Enemy;

        /// <summary>
        /// The ActionCommand associated with the BattleAction
        /// </summary>
        public ActionCommand actionCommand { get; set; }

        /// <summary>
        /// Tells whether the action command is enabled or not.
        /// Action commands are by default disabled for enemies
        /// </summary>
        public bool CommandEnabled => (HasActionCommand == true && EnableActionCommand == true);

        /// <summary>
        /// Whether Action Commands are enabled on this action.
        /// </summary>
        public bool EnableActionCommand { get; set; } = false;

        /// <summary>
        /// Whether to draw Action Command information or not.
        /// </summary>
        public bool DrawActionCommandInfo { get; set; } = true;

        /// <summary>
        /// Tells if the MoveAction has an Action Command.
        /// </summary>
        public bool HasActionCommand => (actionCommand != null);

        /// <summary>
        /// Tells if the MoveAction deals damage.
        /// </summary>
        public bool DealsDamage => (DamageInfo.DamagingElement != Elements.Invalid);

        /// <summary>
        /// Tells if the MoveAction costs FP or not.
        /// </summary>
        public bool CostsFP => (MoveProperties.ResourceType == MoveResourceTypes.FP && MoveProperties.ResourceCost > 0);

        /// <summary>
        /// Tells if the MoveActionCosts SP or not.
        /// </summary>
        public bool CostsSP => ((MoveProperties.ResourceType == MoveResourceTypes.SSSP 
            || MoveProperties.ResourceType == MoveResourceTypes.CSSP) && MoveProperties.ResourceCost > 0);

        /// <summary>
        /// Tells if the MoveAction heals in some capacity or not.
        /// </summary>
        public bool Heals => (HealingInfo != null);

        /// <summary>
        /// Whether the MoveAction is disabled in the menu or not.
        /// <para>This is often true if the user doesn't have enough FP to perform the move.
        /// It's also true if the move can't hit any BattleEntities it targets.</para>
        /// </summary>
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// The text displayed when selecting a MoveAction when it's disabled.
        /// </summary>
        public string DisabledString { get; set; } = string.Empty;

        /// <summary>
        /// Tells if the original FP cost was lowered by Flower Saver or Flower Saver P.
        /// </summary>
        public bool LoweredFPCost { get; protected set; } = false;

        #endregion

        #region Color Fields

        /// <summary>
        /// The color used for displaying the text for MoveActions in special cases.
        /// <para>Examples include indicating the FP cost was lowered with Flower Saver, or Quick Change being active.</para>
        /// </summary>
        //NOTE: Change back to blue gray later, this is just so it's visible now
        public static readonly Color SpecialCaseColor = Color.Blue;//new Color(102, 153, 204);

        /// <summary>
        /// The color used for displaying when a MoveAction is enabled.
        /// </summary>
        public static readonly Color EnabledColor = Color.White;

        public static readonly Color TextEnabledColor = Color.Black;

        /// <summary>
        /// The color used for displaying when a MoveAction is disabled.
        /// </summary>
        public static readonly Color DisabledColor = Color.LightSlateGray;

        public static readonly Color TextDisabledColor = Color.DarkSlateGray;

        /// <summary>
        /// The alpha of the MoveAction's entry in the menu when it's not selected.
        /// </summary>
        public const float UnselectedAlpha = .7f;

        #endregion

        protected MoveAction()
        {
            
        }

        public MoveAction(string name, MoveActionData moveProperties, Sequence moveSequence)
        {
            Name = name;
            MoveInfo = moveProperties;
            SetMoveSequence(moveSequence);
        }

        public MoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, ActionCommand actioncommand) : this(name, moveProperties, moveSequence)
        {
            actionCommand = actioncommand;
            actionCommand.SetHandler(MoveSequence);
        }

        public MoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, DamageData damageInfo) : this(name, moveProperties, moveSequence)
        {
            DamageInfo = damageInfo;
        }

        public MoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, ActionCommand actioncommand, DamageData damageInfo) : this(name, moveProperties, moveSequence, actioncommand)
        {
            DamageInfo = damageInfo;
        }

        public MoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, HealingData healingInfo) : this(name, moveProperties, moveSequence)
        {
            HealingInfo = healingInfo;
        }

        public MoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, ActionCommand actionCommand, HealingData healingInfo) : this(name, moveProperties, moveSequence, actionCommand)
        {
            HealingInfo = HealingInfo;
        }

        protected void SetMoveSequence(Sequence moveSequence)
        {
            MoveSequence = moveSequence;
            MoveSequence.SetAction(this);

            if (HasActionCommand == true)
            {
                actionCommand.SetHandler(MoveSequence);
            }
        }

        /// <summary>
        /// Initializes settings for the action's Action Command and MoveSequence.
        /// Settings include whether they should auto-complete or not based on the action user's properties.
        /// </summary>
        protected void InitActionCommandSequenceSettings()
        {
            //Set auto completing Action Commands and Stylish Moves if the BattleEntity has the pre-requisites
            if (HasActionCommand == true)
                actionCommand.AutoComplete = User.EntityProperties.HasAdditionalProperty(AdditionalProperty.AutoActionCommands);
            
            //This can be null at this point for non-standard MoveActions, such as MenuActions
            if (MoveSequence != null)
                MoveSequence.AutoCompleteStylish = User.EntityProperties.HasAdditionalProperty(AdditionalProperty.AutoStylishMoves);
        }

        /// <summary>
        /// Initializes the MoveAction, checking if it should be disabled or not based on certain conditions.
        /// <para>Common conditions include not having enough FP to perform the move and not being able to reach any BattleEntities with this move.</para>
        /// </summary>
        public virtual void Initialize()
        {
            InitActionCommandSequenceSettings();

            /*Check if the MoveAction should be disabled or not
               1. Check the FP cost, if it costs FP
               2. Check if the move can hit any BattleEntities it targets
             */

            if (CostsFP == true)
            {
                //Check for the number of Flower Saver Badges on the entity and reduce the FP cost by that amount; minimum of 1
                int flowerSaverCount = User.GetEquippedNPBadgeCount(BadgeGlobals.BadgeTypes.FlowerSaver);
                MoveInfo.ResourceCost = UtilityGlobals.Clamp(MoveInfo.ResourceCost - flowerSaverCount, 1, 99);

                //If there is at least one Flower Saver Badge equipped, display the FP count in a bluish-gray color
                if (flowerSaverCount > 0 && MoveInfo.CostDisplayType != CostDisplayTypes.Hidden)
                    MoveInfo.CostDisplayType = CostDisplayTypes.Special;

                //Disable the move if you don't have enough FP to use it
                if (MoveProperties.ResourceCost > User.CurFP)
                {
                    Disabled = true;
                    DisabledString = "Not enough FP.";
                    return;
                }
            }
            else if (CostsSP == true)
            {
                MarioStats mStats = User.BattleStats as MarioStats;
                if (mStats == null)
                {
                    Disabled = true;
                    DisabledString = "No Star Power available to use!";
                    return;
                }

                StarPowerBase starPower = null;

                if (MoveProperties.ResourceType == MoveResourceTypes.SSSP)
                    starPower = mStats.GetStarPowerFromType(StarPowerGlobals.StarPowerTypes.StarSpirit);
                else if (MoveProperties.ResourceType == MoveResourceTypes.CSSP)
                    starPower = mStats.GetStarPowerFromType(StarPowerGlobals.StarPowerTypes.CrystalStar);

                if (starPower == null)
                {
                    Disabled = true;
                    DisabledString = "No Star Power available to use!";
                    return;
                }

                //Disable the move if you don't have enough SP to use it
                if (starPower.CanUseStarPower(MoveProperties.ResourceCost) == false)
                {
                    Disabled = true;
                    DisabledString = "Not enough SP.";
                    return;
                }
            }

            //If the move targets entities, check if any entities can be targeted
            if (MoveProperties.MoveAffectionType != MoveAffectionTypes.None)
            {
                BattleEntity[] entities = GetEntitiesMoveAffects();

                //There are no entities this move can target
                if (entities.Length == 0)
                {
                    Disabled = true;
                    DisabledString = "There's no one this move can target!";
                    return;
                }
            }
        }

        public virtual void SetMoveCategory(MoveCategories moveCategory)
        {
            MoveCategory = moveCategory;
        }

        /// <summary>
        /// What happens when the MoveAction is selected on the menu.
        /// The default behavior is to start target selection with the ActionStart method
        /// </summary>
        public virtual void OnMenuSelected()
        {
            //If this action targets an entity, bring up the target selection menu
            if (MoveProperties.MoveAffectionType != MoveAffectionTypes.None)
            {
                BattleUIManager.Instance.StartTargetSelection(ActionStart, MoveProperties.SelectionType, GetEntitiesMoveAffects());
            }
            //Otherwise, simply start the action
            else
            {
                ActionStart(null);
            }
        }

        /// <summary>
        /// Clears the menu stack and makes the entity whose turn it is start performing this action
        /// </summary>
        /// <param name="targets"></param>
        protected void ActionStart(BattleEntity[] targets)
        {
            BattleUIManager.Instance.ClearMenuStack();
            User.StartAction(this, false, targets);
        }

        /// <summary>
        /// What happens when the MoveAction starts.
        /// This is called immediately before starting the Sequence.
        /// </summary>
        public virtual void OnActionStarted()
        {
            //Subtract FP if the move costs FP. The BattleEntity must have enough FP
            //at this point, as the action is not selectable from the menu if it doesn't have enough
            if (CostsFP == true)
            {
                User.LoseFP((int)MoveProperties.ResourceCost);
            }

            //Subtract SP if the move costs SP. The BattleEntity must have enough SP at this point since the move can't be selected otherwise
            if (CostsSP == true)
            {
                MarioStats mStats = User.BattleStats as MarioStats;
                if (mStats != null)
                {
                    StarPowerBase starPower = null;

                    if (MoveProperties.ResourceType == MoveResourceTypes.SSSP)
                        starPower = mStats.GetStarPowerFromType(StarPowerGlobals.StarPowerTypes.StarSpirit);
                    else if (MoveProperties.ResourceType == MoveResourceTypes.CSSP)
                        starPower = mStats.GetStarPowerFromType(StarPowerGlobals.StarPowerTypes.CrystalStar);

                    //Decrease SP
                    if (starPower != null)
                    {
                        starPower.LoseStarPower(MoveProperties.ResourceCost);
                    }
                }
            }

            //If it's not an item move, remove the dip item turns property
            //This ensures that no item turns remain if the entity was using Double/Triple Dip but did something else via Confusion
            if (User.EntityProperties.HasAdditionalProperty(AdditionalProperty.DipItemTurns) == true)
            {
                User.EntityProperties.RemoveAdditionalProperty(AdditionalProperty.DipItemTurns);
            }
        }

        /// <summary>
        /// What happens when the MoveAction ends.
        /// This is called immediately after ending the Sequence.
        /// </summary>
        public virtual void OnActionEnded()
        {
            //If the last action can expend a charge, check to see if the entity has a charge and remove it
            if (MoveProperties.UsesCharge == true && User.HasCharge() == true)
            {
                User.RemoveStatus(StatusTypes.Charged, true, false);
            }
        }

        public void StartSequence(params BattleEntity[] targets)
        {
            MoveSequence.StartSequence(targets);

            //Catch a reference error early on
            if (HasActionCommand == true)
            {
                if (actionCommand.Handler != MoveSequence)
                {
                    Debug.LogError($"The Action Command's Handler for {Name} is NOT the MoveSequence reference! This WILL cause problems, so fix ASAP");
                    Debug.LogError($"Type name for Handler: {actionCommand.Handler.GetType().Name} and for MoveSequence: {MoveSequence.GetType().Name}");
                }
            }
        }

        public override void Update()
        {
            base.Update();

            //Perform sequence
            MoveSequence.Update();
        }

        public override void Draw()
        {
            base.Draw();

            if (MoveSequence.InSequence == true)
            {
                //Don't show Action Command information if we shouldn't
                if (CommandEnabled == true && DrawActionCommandInfo == true)
                {
                    SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont,
                    $"Command: {Name} performed by {User.Name}",
                    new Vector2(RenderingGlobals.BaseResolutionWidth / 2, 50f), Color.Black, 0f, new Vector2(.5f, .5f), 1.1f, .9f);
                }

                MoveSequence.Draw();
            }
        }

        /// <summary>
        /// Draws the information about this action in the menu.
        /// This can include the name and FP cost.
        /// </summary>
        /// <param name="position">The position to draw the information at.</param>
        /// <param name="color">The color to draw the information.</param>
        /// <param name="textColor">The color to draw the text for the information.</param>
        /// <param name="alphaMod">The alpha value of the color. This is less than 1 if this MoveAction isn't currently selected on the menu.</param>
        /// <param name="iconXOffset">The X offset to display the move's icon.</param>
        /// <param name="resourceCostXOffset">The X offset to display the move's resource cost.</param>
        //public virtual void DrawMenuInfo(Vector2 position, Color color, Color textColor, float alphaMod, float iconXOffset, float resourceCostXOffset)
        //{
        //    //Draw icon
        //    if (MoveInfo.Icon != null && MoveInfo.Icon.Tex != null)
        //    {
        //        SpriteRenderer.Instance.DrawUI(MoveInfo.Icon.Tex, position - new Vector2(iconXOffset, 0), MoveInfo.Icon.SourceRect, color * alphaMod, false, false, .39f);
        //    }
        //
        //    //Draw name
        //    SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, Name, position, textColor * alphaMod, 0f, Vector2.Zero, 1f, .4f);
        //
        //    //Show cost if the move costs anything
        //    if ((CostsFP == true || CostsSP == true) && MoveProperties.CostDisplayType != CostDisplayTypes.Hidden)
        //    {
        //        Color fpColor = textColor;
        //
        //        //If the resource cost was lowered, show it a bluish-gray color (This feature is from PM)
        //        //Keep it gray if the move is disabled for any reason
        //        if (Disabled == false && MoveProperties.CostDisplayType == CostDisplayTypes.Special)
        //        {
        //            Color blueGray = SpecialCaseColor;
        //            fpColor = blueGray;
        //        }
        //
        //        SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, GetCostString(), position + new Vector2(resourceCostXOffset, 0), fpColor * alphaMod, 0f, Vector2.Zero, 1f, .4f);
        //    }
        //}

        /// <summary>
        /// Gets the cost of the MoveAction. This is the FP cost for most moves and SP cost for Special Moves.
        /// </summary>
        /// <returns>A string of the cost to use the action.</returns>
        //public string GetCostString()
        //{
        //    if (MoveProperties.ResourceType == MoveResourceTypes.FP)
        //    {
        //        return $"{MoveProperties.ResourceCost} FP";
        //    }
        //    else
        //    {
        //        return $"{MoveProperties.ResourceCost / StarPowerGlobals.SPUPerStarPower} SP";
        //    }
        //}

        /// <summary>
        /// Gets the set of BattleEntities that this move affects.
        /// <para>The default behavior is to get the entities based on the MoveAction's <see cref="MoveAffectionTypes"/>.</para>
        /// </summary>
        /// <returns>The BattleEntities the move affects based on its MoveAffectionType and the HeightStates it can target.
        /// If None, an empty array is returned.</returns>
        /* NOTE: We eventually may want this to be static and make the instance version call the static one.
         * This would allow filtering just from data without requiring an instance.
         */
        public BattleEntity[] GetEntitiesMoveAffects()
        {
            List<BattleEntity> entities = new List<BattleEntity>();

            bool addedAllies = false;

            //Check for adding allies
            if (UtilityGlobals.MoveAffectionTypesHasFlag(MoveProperties.MoveAffectionType, MoveAffectionTypes.Ally) == true)
            {
                entities.AddRange(BattleManager.Instance.GetEntities(User.EntityType, MoveProperties.HeightsAffected));
                addedAllies = true;
            }
            
            //Check if the user of the move should be added
            if (UtilityGlobals.MoveAffectionTypesHasFlag(MoveProperties.MoveAffectionType, MoveAffectionTypes.Self) == true)
            {
                //If we didn't add allies, add the user of the move
                if (addedAllies == false)
                {
                    entities.Add(User);
                }
            }
            else
            {
                //Otherwise if we're not adding the user of the move and we did add allies, remove the user so we end up with only allies
                if (addedAllies == true)
                {
                    entities.Remove(User);
                }
            }

            //If this move targets other types of BattleEntities, add all of the ones it targets in order
            if (UtilityGlobals.MoveAffectionTypesHasFlag(MoveProperties.MoveAffectionType, MoveAffectionTypes.Other) == true)
            {
                if (MoveProperties.OtherEntTypes != null && MoveProperties.OtherEntTypes.Length > 0)
                {
                    for (int i = 0; i < MoveProperties.OtherEntTypes.Length; i++)
                    {
                        EntityTypes otherType = MoveProperties.OtherEntTypes[i];
                        entities.AddRange(BattleManager.Instance.GetEntities(otherType, MoveProperties.HeightsAffected));
                    }
                }
                else
                {
                    Debug.LogWarning($"{Name} targets {nameof(MoveAffectionTypes.Other)}, but {nameof(MoveProperties.OtherEntTypes)} is null or empty.");
                }
            }

            //If this move has custom entity targeting logic, add the entities to the list
            if (UtilityGlobals.MoveAffectionTypesHasFlag(MoveProperties.MoveAffectionType, MoveAffectionTypes.Custom) == true)
            {
                BattleEntity[] customAffected = GetCustomAffectedEntities();

                if (customAffected != null && customAffected.Length > 0)
                {
                    entities.AddRange(customAffected);
                }
            }

            //Filter out untargetable BattleEntities
            BattleManagerUtils.FilterEntitiesByTargetable(entities);

            //If the BattleEntity has a custom targeting method and shouldn't be targeted, remove it
            //Otherwise, set to the true target in the event something is defending it
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                //Check for a custom targeting method
                bool? targetVal = entities[i].EntityProperties.CustomTargeting?.Invoke(this);

                //If it returns false, remove the BattleEntity from the list
                if (targetVal == false)
                {
                    entities.RemoveAt(i);
                    continue;
                }
                else
                {
                    entities[i] = entities[i].GetTrueTarget();
                }
            }

            return entities.ToArray();
        }

        protected virtual BattleEntity[] GetCustomAffectedEntities()
        {
            return null;
        }
    }
}
