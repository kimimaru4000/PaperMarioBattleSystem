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
    public class MoveAction : BattleAction, IActionCommand
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
        /// What happens when the BattleAction is selected on the menu.
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
                    SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font,
                    $"Command: {Name} performed by {User.Name}",
                    new Vector2(SpriteRenderer.Instance.WindowCenter.X, 50f), Color.Black, 0f, new Vector2(.5f, .5f), 1.1f, .9f, true);

                    actionCommand?.Draw();
                }
            }
        }
    }
}
