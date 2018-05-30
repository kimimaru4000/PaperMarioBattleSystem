using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Koopatrol enemy AI.
    /// </summary>
    public sealed class KoopatrolAI : KoopaTroopaAI
    {
        public KoopatrolAI(Koopatrol koopatrol) : base(koopatrol)
        {

        }

        protected override MoveAction ActionPerformed
        {
            get
            {
                //Check if any other Koopatrols are available
                //If there are less than a certain number, summon one if a random check succeeds
                bool shouldSummon = UtilityGlobals.TestRandomCondition(35d);
                if (shouldSummon == true)
                {
                    //Since the Koopatrol excludes itself, this number should be 1 less than the minimum number required for this action
                    const int minCountSummon = 2;

                    List<BattleEntity> koopatrolsList = new List<BattleEntity>();
                    BattleManager.Instance.GetEntityAllies(koopatrolsList, Enemy, Enemy.HeightState);

                    Type koopatrolType = Enemy.GetType();
                    for (int i = 0; i < koopatrolsList.Count; i++)
                    {
                        //Check exact types. Remove BattleEntities that aren't Koopatrols
                        Type enemyType = koopatrolsList[i].GetType();

                        if (koopatrolType != enemyType)
                        {
                            koopatrolsList.RemoveAt(i);
                            i--;
                        }
                    }

                    //Summon another Koopatrol
                    if (koopatrolsList.Count < minCountSummon)
                    {
                        return new SummonKoopatrolAction(Enemy);
                    }
                }

                return base.ActionPerformed;
            }
        }
    }
}
