using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Runs away from battle, ending the battle immediately.
    /// <para>Unless Runaway Pay is equipped, all Star Points accumulated during battle are lost when running.</para>
    /// </summary>
    public sealed class RunAwayAction : MoveAction
    {
        public RunAwayAction()
        {
            Name = "Run Away";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(743, 10, 39, 37)),
                "Run from battle.", MoveResourceTypes.FP, 0, CostDisplayTypes.Hidden, MoveAffectionTypes.None,
                TargetSelectionMenu.EntitySelectionType.First, false, null, null);

            SetMoveSequence(new RunAwaySequence(this));
            actionCommand = new RunAwayCommand(MoveSequence, 100d, 8d, .4d, 400d, 3500d, Keys.Z);
        }

        public override void OnMenuSelected()
        {
            //In TTYD, all allies run and play the same animations at the same time, so pass them in
            ActionStart(BattleManager.Instance.GetEntities(User.EntityType, null));
        }

        public override void Initialize()
        {
            if (BattleManager.Instance.Properties.Runnable == false)
            {
                Disabled = true;
                DisabledString = "Can't select that!";
            }
        }
    }
}
