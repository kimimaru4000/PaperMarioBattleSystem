using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sweet Treat action. Throw stars at the icons to restore HP and FP, and avoid the Poison Mushrooms.
    /// </summary>
    public class SweetTreatAction : SpecialMoveAction
    {
        public SweetTreatAction(BattleEntity user) : base(user)
        {
            Name = "Sweet Treat";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(26, 989, 40, 31)),
                "", MoveResourceTypes.SSSP, 100, CostDisplayTypes.Shown, MoveAffectionTypes.Self | MoveAffectionTypes.Ally, 
                Enumerations.EntitySelectionType.All, false, null);

            HealingInfo = new HealingData(0, 0, GetStatusesCured());

            SetMoveSequence(new SweetTreatSequence(this));
            actionCommand = new SweetTreatCommand(MoveSequence, User.BManager.battleUIManager);
        }

        protected StatusTypes[] GetStatusesCured()
        {
            return new StatusTypes[]
            {
                StatusTypes.Poison, StatusTypes.DEFDown, StatusTypes.Dizzy,
                StatusTypes.Confused, StatusTypes.Frozen, StatusTypes.Burn,
                StatusTypes.Slow, StatusTypes.Sleep, StatusTypes.Tiny,
                StatusTypes.Stop, StatusTypes.POWDown, StatusTypes.NoSkills
            };
        }
    }
}
