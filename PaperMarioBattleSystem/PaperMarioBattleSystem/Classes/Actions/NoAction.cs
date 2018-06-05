using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleAction that causes the BattleEntity to do nothing.
    /// <para>This is used when Mario or his Partner take the "Do Nothing" action and when entities are unable to attack an ally
    /// because it has no allies remaining or the move can't reach the ally.</para>
    /// <para>This is also the recommended method of skipping turns in many circumstances (Ex. a flipped Koopa).
    /// Instead of ending the turn directly, have the BattleEntity perform a NoAction.</para>
    /// </summary>
    public sealed class NoAction : MoveAction
    {
        public NoAction(BattleEntity user) : base(user)
        {
            Name = "Do Nothing";
            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(182, 844, 24, 24)),
                "Do nothing this turn.", Enumerations.MoveResourceTypes.FP, 0,
                Enumerations.CostDisplayTypes.Hidden, Enumerations.MoveAffectionTypes.None, Enumerations.EntitySelectionType.Single,
                false, null);

            SetMoveSequence(new NoSequence(this));
        }
    }
}
