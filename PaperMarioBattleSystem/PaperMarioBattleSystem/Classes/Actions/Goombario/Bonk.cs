using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Goombario's Bonk action
    /// </summary>
    public sealed class Bonk : BattleAction
    {
        public Bonk()
        {
            Name = "Bonk";
        }

        public override void OnMenuSelected()
        {
            BattleUIManager.Instance.StartTargetSelection((targets) =>
            {
                for (int i = 0; i < targets.Length; i++)
                    targets[i].LoseHP(1);
                BattleManager.Instance.EntityTurn.UsedTurn = true;
                BattleManager.Instance.EntityTurn.EndTurn();
            }, TargetSelectionMenu.EntitySelectionType.Single, BattleManager.Instance.GetAliveEnemies());
        }
    }
}
