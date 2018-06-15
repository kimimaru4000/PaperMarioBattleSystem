using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for enemy AI behavior.
    /// </summary>
    public abstract class EnemyAIBehavior
    {
        /// <summary>
        /// The enemy choosing the action to perform.
        /// </summary>
        public BattleEnemy Enemy { get; private set; } = null;

        protected EnemyAIBehavior()
        {

        }

        protected EnemyAIBehavior(BattleEnemy enemy)
        {
            Enemy = enemy;
        }

        /// <summary>
        /// Tells the enemy to perform an action on its turn.
        /// </summary>
        public abstract void PerformAction();

        /// <summary>
        /// Tells whether the enemy has a held Item.
        /// </summary>
        /// <returns>true if the enemy has a HeldCollectible and the HeldCollectible is an Item, otherwise false.</returns>
        protected bool HasItem()
        {
            return (Enemy.HeldCollectible?.CollectibleType == Enumerations.CollectibleTypes.Item);
        }

        /// <summary>
        /// Base logic for making an Enemy use a held Item on the appropriate targets.
        /// <para>If it's a healing item, it'll use it on hurt allies.
        /// If a damaging item, it'll use it on opponents.</para>
        /// </summary>
        /// <returns>true if the item was used, otherwise false.</returns>
        protected virtual bool TryUseItem()
        {
            //If the enemy doesn't have an item, we can't use it
            if (HasItem() == false) return false;

            //Say there's a 33% chance of using the item for now
            //NOTE: Find the actual games' chance
            int randVal = RandomGlobals.Randomizer.Next(0, 3);

            //The enemy didn't decide to use the item, so return
            if (randVal != 0)
                return false;

            BattleItem bItem = (BattleItem)Enemy.HeldCollectible;
            ItemAction itemMove = new ItemAction(Enemy, bItem);

            //Get the affected list; target all at the start
            List<BattleEntity> affectedEntities = new List<BattleEntity>();
            itemMove.GetEntitiesMoveAffects(affectedEntities);

            BattleEntity[] finalArray = null;

            //If it targets anyone, see who to target
            if (affectedEntities.Count > 0)
            {
                //If the move targets the first BattleEntity, remove all but it
                if (itemMove.MoveProperties.SelectionType == Enumerations.EntitySelectionType.First)
                {
                    if (affectedEntities.Count > 1)
                        affectedEntities.RemoveRange(1, affectedEntities.Count - 1);
                }
                //If it affects a single one, choose the one to target
                else if (itemMove.MoveProperties.SelectionType == Enumerations.EntitySelectionType.Single)
                {
                    //If it damages, simply choose a random target
                    if (itemMove.DealsDamage == true)
                    {
                        //Remove all but the randomly chosen one
                        BattleEntity randChoice = affectedEntities[RandomGlobals.Randomizer.Next(0, affectedEntities.Count)];
                        affectedEntities.Clear();
                        affectedEntities.Add(randChoice);
                    }
                    //If it heals, see who is hurt
                    else if (itemMove.Heals == true)
                    {
                        for (int i = affectedEntities.Count - 1; i >= 0; i--)
                        {
                            //If it's not hurt, remove it
                            if (affectedEntities[i].CurHP >= affectedEntities[i].BattleStats.MaxHP)
                            {
                                affectedEntities.RemoveAt(i);
                            }
                        }

                        //No one is hurt, so don't use the item
                        if (affectedEntities.Count == 0) return false;

                        //Choose a random BattleEntity to heal
                        BattleEntity randChoice = affectedEntities[RandomGlobals.Randomizer.Next(0, affectedEntities.Count)];
                        affectedEntities.Clear();
                        affectedEntities.Add(randChoice);
                    }
                }
                else
                {
                    //If it heals and affects all, see if anyone is actually damaged
                    if (itemMove.Heals == true)
                    {
                        bool anyHurt = false;

                        for (int i = 0; i < affectedEntities.Count; i++)
                        {
                            if (affectedEntities[i].CurHP < affectedEntities[i].BattleStats.MaxHP)
                            {
                                anyHurt = true;
                                break;
                            }
                        }

                        //Don't use the item if no one the item affects is hurt
                        if (anyHurt == false) return false;
                    }
                }

                //Put all the BattleEntities affected into the final array
                finalArray = affectedEntities.ToArray();
            }

            //Use the item
            Enemy.StartAction(itemMove, false, finalArray);

            return true;
        }
    }
}
