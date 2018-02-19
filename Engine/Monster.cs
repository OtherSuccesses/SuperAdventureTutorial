using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class Monster : LivingCreature
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int RewardGold { get; set; }
        public int RewardExperiencePoints { get; set; }
        public int MaximumDamage { get; set; }
        public List<LootItem> LootTable { get; set; }

        public Monster (int id, string name, int rewardGold, int rewardExperiencePoints, int maximumDamage, int currentHitPoints, 
            int maximumHitPoints) : base (currentHitPoints, maximumHitPoints)
        {
            ID = id;
            Name = name;
            RewardGold = rewardGold;
            RewardExperiencePoints = rewardExperiencePoints;
            MaximumDamage = maximumDamage;
            LootTable = new List<LootItem>();
        }
    }
}
