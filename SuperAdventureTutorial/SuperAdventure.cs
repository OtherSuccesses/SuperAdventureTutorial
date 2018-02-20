using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace SuperAdventureTutorial
{
    public partial class SuperAdventure : Form
    {
        private Player _player;
        private Monster _currentMonster;

        public SuperAdventure()
        {
            InitializeComponent();

            Location location = new Location(1, "Home", "This is where you live. Feels cozy.");

            _player = new Player(20, 0, 1, 10, 10);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        //Function to control arbitrary movement
        private void MoveTo(Location newLocation)
        {
            if (newLocation.ItemRequiredToEnter != null)
            {
                bool playerHasRequiredItems = false;

                forEach(InventoryItem ii in _player.Inventory)
                {
                    if (ii.Details.ID == newLocation.ItemRequiredToEnter.ID)
                    {
                        playerHasRequiredItems = true;
                        break;
                    }
                }

                if (!playerHasRequiredItems)
                {
                    rtbMessage.Text = "You must have a " + newLocation.ItemRequiredToEnter + " to enter this area. Please " +
                        "return when you've found one." + Environment.NewLine;
                    return;
                }
            }

            //If the required item is in Inventory, that is sufficient for travel 
            _player.CurrentLocation = newLocation;

            //Controlling button visibility
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);

            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text = newLocation.Description + Environment.NewLine;

            _player.CurrentHitPoints = _player.MaximumHitPoints;

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            if (newLocation.QuestAvailableHere != null)
            {
                bool playerAlreadyHasQuest = false;
                bool playerCompletedQuest = false;

                foreach (PlayerQuest playerQuest in _player.Quests)
                {
                    if(playerQuest.Details.ID == newLocation.QuestAvailableHere.ID)
                    {
                        playerAlreadyHasQuest = true;

                        if (playerQuest.IsCompleted)
                        {
                            playerCompletedQuest = true;
                        }
                    }
                }

                //If the player has the quest in their log...
                if (playerAlreadyHasQuest)
                {
                    if (!playerCompletedQuest)
                    {
                        bool playerHasAllItemsToCompleteQuest = true;

                        foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                        {
                            bool foundItemInPlayersInventory = false;

                            foreach (InventoryItem ii in _player.Inventory)
                            {
                                if (ii.Details.ID == qci.Details.ID)
                                {
                                    foundItemInPlayersInventory = true;
                                    if (ii.Quantity < qci.Quantity)
                                    {
                                        playerHasAllItemsToCompleteQuest = false;
                                        break;
                                    }
                                    //This break is to stop the look in inventory once item is found
                                    break;
                                }
                            }
                            if (!foundItemInPlayersInventory)
                            {
                                playerHasAllItemsToCompleteQuest = false;
                                //Plenty o' breaks to prevent excess loopage
                                break;
                            }
                        }
                        //If player has sufficient items, run all this business
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            //Message of completion
                            rtbMessage.Text += Environment.NewLine;
                            rtbMessage.Text += "You have completed the " + newLocation.QuestAvailableHere.Name +
                                " quest!" + Environment.NewLine;

                            foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                            {
                                foreach(InventoryItem ii in _player.Inventory)
                                {
                                    if(ii.Details.ID == qci.Details.ID)
                                    {
                                        ii.Quantity -= qci.Quantity;
                                        break;
                                    }
                                }
                            }

                            rtbMessage.Text += "You receive: " + Environment.NewLine;
                            rtbMessage.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() +
                                " Experience Points" + Environment.NewLine;
                            rtbMessage.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold." +
                                Environment.NewLine;
                            rtbMessage.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessage.Text += Environment.NewLine;

                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            bool addedItemToPlayerInventory = false;

                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                //If the item already exists in the players inventory it adds one, woohoo.
                                if (ii.Details.ID = newLocation.QuestAvailableHere.RewardItem.ID)
                                {
                                    ii.Quantity++;
                                    addedItemToPlayerInventory = true;
                                    break;
                                }
                            }

                            //If item doesn't exist in player inventory it is added
                            if (!addedItemToPlayerInventory)
                            {
                                _player.Inventory.Add(new InventoryItem(newLocation.QuestAvailableHere.RewardItem, 1));
                            }

                            //This sets the quest to complete
                            foreach(PlayerQuest pq in _player.Quests)
                            {
                                if (pq.Details.ID == newLocation.QuestAvailableHere.ID)
                                {
                                    pq.IsCompleted = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                //This is if the quest is not located in the players quest log
                else
                {
                    rtbMessage.Text += "You receive the " + newLocation.QuestAvailableHere.Name +
                        " quest." + Environment.NewLine;
                    rtbMessage.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessage.Text += "To complete it, please return with: " + Environment.NewLine;
                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (qci.Quantity == 1)
                        {
                            rtbMessage.Text += qci.Quantity + " " + qci.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessage.Text += qci.Quantity + " " + qci.Details.NamePlural + Environment.NewLine;
                        }
                    }
                    rtbMessage.Text += Environment.NewLine;

                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            //Does the location have a monster?
            if (newLocation.MonsterLivingHere != null)
            {
                rtbMessage.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                //Set's variable to monster type for location
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);
                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.RewardGold,
                    standardMonster.RewardExperiencePoints, standardMonster.MaximumDamage, standardMonster.CurrentHitPoints,
                    standardMonster.MaximumHitPoints);

                foreach(LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }
            //No monster found!
            else
            {
                _currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }

            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            foreach(InventoryItem inventoryItem in _player.Inventory)
            {
                if(inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[]
                    {
                        inventoryItem.Details.Name, inventoryItem.Quantity.ToString()
                    });
                }
            }

            //Refresh player quest list
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.RowCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Done?";

            dgvQuests.Rows.Clear();

            foreach(PlayerQuest playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[]
                {
                    playerQuest.Details.Name, playerQuest.IsCompleted.ToString()
                });
            }

            List<Weapon> weapons = new List<Weapon>();

            foreach(InventoryItem inventoryItem in _player.Inventory)
            {
                if(inventoryItem.Details is Weapon)
                {
                    if(inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Details);
                    }
                }
            }
            //If no weapons are possessed    
            if (weapons.Count == 0)
            {
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";

                cboWeapons.SelectedIndex = 0;
            }

            List<HealingPotion> healingPotion = new List<HealingPotion>();

            foreach(InventoryItem inventoryItem in _player.Inventory)
            {
                if(inventoryItem.Details is HealingPotion)
                {
                    if(inventoryItem.Quantity > 0)
                    {
                        healingPotion.Add((HealingPotion)inventoryItem.Details);
                    }
                }
            }

            if(healingPotion.Count == 0)
            {
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotion;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {

        }
    }
}
