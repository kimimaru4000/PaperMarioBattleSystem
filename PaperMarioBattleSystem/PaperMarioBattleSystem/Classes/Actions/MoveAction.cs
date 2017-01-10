using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.TargetSelectionMenu;
using static PaperMarioBattleSystem.Enumerations;

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
        protected MoveActionData MoveInfo = MoveActionData.Default;

        /// <summary>
        /// The Sequence this MoveAction has the BattleEntity perform.
        /// </summary>
        public Sequence MoveSequence { get; private set; } = null;

        /// <summary>
        /// The damage information of this MoveAction.
        /// </summary>
        public InteractionParamHolder? DamageInfo { get; protected set; } = null;

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
        /// Action commands are always disabled for enemies
        /// </summary>
        public bool CommandEnabled => (HasActionCommand == true && User.EntityType != EntityTypes.Enemy && DisableActionCommand == false);

        /// <summary>
        /// Whether Action Commands are disabled on this action.
        /// This value automatically resets to false after the action is completed.
        /// </summary>
        public bool DisableActionCommand { get; set; }

        /// <summary>
        /// Tells if the MoveAction has an Action Command.
        /// </summary>
        public bool HasActionCommand => (actionCommand != null);

        /// <summary>
        /// Tells if the MoveAction deals damage.
        /// </summary>
        public bool DealsDamage => (DamageInfo != null);

        /// <summary>
        /// Tells if the MoveAction costs FP or not.
        /// </summary>
        public bool CostsFP => (MoveProperties.FPCost > 0);

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

        /// <summary>
        /// The color used for displaying when a MoveAction is disabled.
        /// </summary>
        public static readonly Color DisabledColor = Color.LightSlateGray;

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

        public MoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, InteractionParamHolder damageInfo) : this(name, moveProperties, moveSequence)
        {
            DamageInfo = damageInfo;
        }

        public MoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, ActionCommand actioncommand, InteractionParamHolder damageInfo) : this(name, moveProperties, moveSequence, actioncommand)
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

        //Virtual to account for all types of MoveActions (Ex. Special Moves check for SP instead of FP)
        public virtual void Initialize()
        {
            /*Check if the MoveAction should be disabled or not
               1. Check the FP cost, if it costs FP
               2. Check if the move can hit any BattleEntities it targets
             */

            if (CostsFP == true)
            {
                //Check for the number of Flower Saver Badges on the entity and reduce the FP cost by that amount; minimum of 1
                int flowerSaverCount = User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.FlowerSaver);
                MoveInfo.FPCost = UtilityGlobals.Clamp(MoveInfo.FPCost - flowerSaverCount, 1, 99);

                //If there is at least one Flower Saver Badge equipped, display the FP count in a bluish-gray color
                if (flowerSaverCount > 0)
                    LoweredFPCost = true;

                if (MoveProperties.FPCost > User.CurFP)
                {
                    Disabled = true;
                    DisabledString = "Not enough FP.";
                    return;
                }
            }

            //If the move targets entities, check if any entities can be targeted
            if (MoveProperties.TargetsEntity == true)
            {
                BattleEntity[] entities = BattleManager.Instance.GetEntities(MoveProperties.EntityType, MoveProperties.HeightsAffected);

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
            if (MoveProperties.TargetsEntity == true)
                BattleUIManager.Instance.StartTargetSelection(ActionStart, MoveProperties.SelectionType, BattleManager.Instance.GetEntities(MoveProperties.EntityType, MoveProperties.HeightsAffected));
            //Otherwise, simply start the action
            else ActionStart(null);
        }

        /// <summary>
        /// Clears the menu stack and makes the entity whose turn it is start performing this action
        /// </summary>
        /// <param name="targets"></param>
        protected void ActionStart(BattleEntity[] targets)
        {
            BattleUIManager.Instance.ClearMenuStack();
            User.StartAction(this, targets);
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
                User.LoseFP(MoveProperties.FPCost);
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
            //We check for both the Charged Status and the ChargedDamage property because the Status could be suspended
            if (MoveProperties.UsesCharge == true && User.EntityProperties.HasStatus(StatusTypes.Charged) == true
                && User.EntityProperties.HasAdditionalProperty(AdditionalProperty.ChargedDamage) == true)
            {
                User.EntityProperties.RemoveStatus(StatusTypes.Charged);
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
                if (CommandEnabled == true)
                {
                    SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont,
                    $"Command: {Name} performed by {User.Name}",
                    new Vector2(SpriteRenderer.Instance.WindowCenter.X, 50f), Color.Black, 0f, new Vector2(.5f, .5f), 1.1f, .9f, true);

                    actionCommand?.Draw();
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
        /// <param name="alphaMod">The alpha value of the color. This is less than 1 if this MoveAction isn't currently selected on the menu.</param>
        public virtual void DrawMenuInfo(Vector2 position, Color color, float alphaMod)
        {
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, Name, position, color * alphaMod, 0f, Vector2.Zero, 1f, .4f);

            //Show FP count if the move costs FP
            if (CostsFP == true && MoveProperties.HideCost == false)
            {
                Color fpColor = color;

                //If the FP cost was lowered, show it a bluish-gray color (This feature is from PM)
                //Keep it gray if the move is disabled for any reason
                if (Disabled == false && LoweredFPCost)
                {
                    Color blueGray = SpecialCaseColor;
                    fpColor = blueGray;
                }

                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, GetCostString(), position + new Vector2(200, 0), fpColor * alphaMod, 0f, Vector2.Zero, 1f, .4f);
            }
        }

        /// <summary>
        /// Gets the cost of the MoveAction. This is the FP cost for most moves and SP cost for Special Moves.
        /// </summary>
        /// <returns>A string of the cost to use the action.</returns>
        public virtual string GetCostString()
        {
            return $"{MoveProperties.FPCost} FP";
        }
    }
}
