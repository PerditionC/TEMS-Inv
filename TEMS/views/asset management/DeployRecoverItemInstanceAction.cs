// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    public class DeployRecoverItemInstanceAction : NotifyPropertyChanged
    {
        public static readonly string MenuDeploy = "De_ploy";
        public static readonly string MenuRecover = "_Recover";

        // cache a copy of reference status that we are updating/querying on
        public ItemStatus statusAvailable { get; private set; } = null;
        public ItemStatus statusDeployed { get; private set; } = null;

        /// <summary>
        /// maintains a reference to our DataReposity as needed to query & update items for deployment/recovery
        /// </summary>
        public DataRepository db
        {
            get { return _db; }
            set
            {
                try
                {
                    SetProperty(ref _db, value, nameof(db));
                    statusAvailable = db.db.LoadAll<ItemStatus>("WHERE name='Available'").First();
                    statusDeployed = db.db.LoadAll<ItemStatus>("WHERE name='Deployed'").First();
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Error updating DeployRecoverItemInstanceAction.db - {e.Message}");
                    throw;
                }
            }
        }
        private DataRepository _db = null;

        // anything that needs initializing
        public DeployRecoverItemInstanceAction() : base() { }


        public string DeployRecoverItemInstanceCommandText
        {
            get { return _DeployRecoverItemInstanceCommandText; }
            set { SetProperty(ref _DeployRecoverItemInstanceCommandText, value, nameof(DeployRecoverItemInstanceCommandText)); }
        }
        private string _DeployRecoverItemInstanceCommandText = MenuDeploy;

        public ICommand DeployRecoverItemInstanceCommand
        {
            get
            {
                if (_DeployRecoverItemInstanceCommand == null)
                    DeployRecoverItemInstanceCommand = new RelayCommand(param => DoDeployRecover(param), param => CanDeployRecover(param));
                return _DeployRecoverItemInstanceCommand;
            }
            private set { SetProperty(ref _DeployRecoverItemInstanceCommand, value, nameof(DeployRecoverItemInstanceCommand)); }
        }
        private ICommand _DeployRecoverItemInstanceCommand = null;


        /// <summary>
        /// Based on current status of item instance represented by parameter
        /// will invoke corresponding status action function and return true 
        /// or if item instance is not in (available, deployed) then does nothing and returns false
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="availableActionFn"></param>
        /// <param name="deployedActionFn"></param>
        /// <returns></returns>
        private bool DispatchDeployRecoverAction(object parameter, Action<object> availableActionFn, Action<object> deployedActionFn)
        {
            // Note we assume parameter already has status value so we don't have to query DB again for full item just to determine status
            // and update command text and return its status
            if (IsItemInstanceAvailable(parameter))
            {
                availableActionFn(parameter);
                return true;
            }
            if (IsItemInstanceDeployed(parameter))
            {
                deployedActionFn(parameter);
                return true;
            }

            return false;
        }

        /// <summary>
        /// returns true if object o is an ItemResult and also selected; false otherwise
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private bool IsSelected(object o)
        {
            var results = o as ItemResult; // use ItemResult instead of QueryResultsBase so only returns true if ItemResult object is selected
            return results?.IsSelected ?? false;
        }

        public bool CanDeployRecover(object parameter)
        {
            // parameter can be either a single item or a collection of selected items
            var selectedItems = parameter as ObservableCollection<object>;
            if (selectedItems != null)
            {
                // if collection, assume all are either available or deployed and use 1st as representative of which
                parameter = selectedItems.FirstOrDefault(x => IsSelected(x));
            }

            return DispatchDeployRecoverAction(parameter, x => DeployRecoverItemInstanceCommandText = MenuDeploy, x => DeployRecoverItemInstanceCommandText = MenuRecover);
        }

        /// <summary>
        /// recursively deploys/recovers item instances identified by parameter
        /// </summary>
        /// <param name="parameter">parameter must either be a IList<object> of QueryResultsBase objects or an ItemInstance</object></param>
        public void DoDeployRecover(object parameter)
        {
            logger.Debug("Deploying Recovering Item");

            // parameter can be either a single item or a collection of selected items
            var selectedItems = parameter as ObservableCollection<object>;
            if (selectedItems != null)
            {
                // if collection, assume all are either available or deployed and use 1st as representative of which
                var listItem = selectedItems.FirstOrDefault(x => IsSelected(x)) as QueryResultsBase;

                foreach (var i in selectedItems)
                {
                    DispatchDeployRecoverAction(i, x => DeployRecoverItem(x, statusDeployed), x => DeployRecoverItem(x, statusAvailable));
                }
            }
            else
            {
                // if single item, then use whatever it is
                DispatchDeployRecoverAction(parameter, x => DeployRecoverItem(x, statusDeployed), x => DeployRecoverItem(x, statusAvailable));
            }
        }

        private DeployEvent GetNewEvent(ItemInstance itemInstance)
        {
            var deployEvent = new DeployEvent
            {
                // set default/common values, specific event values set by caller
                itemInstance = itemInstance,
                deployBy = "ME", // User.id
                deployDate = DateTime.Now,
                //notes = (new UserControls.InputBox("Deployment notes:", "?", "")).ShowDialog();
            };
            return deployEvent;
        }

        private DeployEvent GetLastEvent(ItemInstance itemInstance)
        {
            var deployEvent = db.GetDeploymentEventsFor(itemInstance.id).FirstOrDefault();
            if ((deployEvent == null) || (deployEvent.recoverDate != null))
            {
                deployEvent = GetNewEvent(itemInstance);
            }
            return deployEvent;
        }

        /// <summary>
        /// Given an ItemInstance, will set it up as deployed/recovered, if ItemInstance is
        /// as bin or module then will also deploy/recover all items contained within
        /// </summary>
        /// <param name="itemInstance"></param>
        private void RecursiveDeployRecover(ItemInstance itemInstance, ItemStatus status, string notes)
        {
            try
            {
            var deployEvent = GetLastEvent(itemInstance);
            if (string.Equals(itemInstance.status.name, "Deployed", StringComparison.InvariantCultureIgnoreCase))
            {
                // if deployed then should return prior deployed event
                deployEvent.recoverBy = "ME"; // User.id
                deployEvent.recoverDate = DateTime.Now;
            }
            else
            {
                // otherwise returning a newly deployed event, we need to set any non-default values for
                deployEvent.notes = notes;
            }
            db.db.Save(deployEvent);

            itemInstance.status = status;
            db.db.Save(itemInstance);

            if (itemInstance.item.itemType.isBin || itemInstance.item.itemType.isModule)
            {
                var children = db.GetItemInstancesWithinBinOrModuleAsync(itemInstance);
                logger.Debug(itemInstance.itemNumber + " " + itemInstance.item.itemType.name);
                foreach (var child in children)
                {
                    logger.Debug("   " + child.itemNumber);
                    RecursiveDeployRecover(child, status, notes);
                }
            }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in RecursiveDeployRecover - {e.Message}");
                throw;
            }
        }

        private void DeployRecoverItem(object parameter, ItemStatus status)
        {
            try
            {
                // first try if bound to a full ItemInstance
                var itemInstance = parameter as ItemInstance;
                // if not, assume bound to an QueryResultsBase and retrieve ItemInstance from it (either cached or loading from DB)
                var listItem = parameter as QueryResultsBase;
                if (itemInstance == null)
                    itemInstance = LoadItemInstance(listItem);

                if (itemInstance != null)
                {
                    // apply same notes to all item and any contained items
                    var notes = (new TEMS_Inventory.UserControls.InputBox("Deployment notes:", "?", "")).ShowDialog();
                    RecursiveDeployRecover(itemInstance, status, notes);

                    // update list item so coherent with actual item
                    var resultItem = listItem as ItemResult;
                    if (resultItem != null) resultItem.status = itemInstance.status?.name;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in DeployRecoverItem - {e.Message}");
                throw;
            }
        }

        // attempt to use previously loaded instance, otherwise load instance from DB
        private ItemInstance LoadItemInstance(QueryResultsBase listItem)
        {
            try
            {
                ItemInstance itemInstance = listItem?.entity as ItemInstance;
                if (itemInstance == null)
                {
                    var itemResult = listItem as ItemResult;
                    if (itemResult != null)
                    {
                        itemInstance = db.db.Load<ItemInstance>(itemResult.instancePk);
                        listItem.entity = itemInstance;
                    }
                }

                return itemInstance;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in LoadItemInstance - {e.Message}");
                throw;
            }
        }

        private bool IsItemInstanceAvailable(object parameter)
        {
            try
            {
                var itemResult = parameter as ItemResult;
                if (itemResult != null)
                    return string.Equals(itemResult.status, "Available", StringComparison.InvariantCultureIgnoreCase);

                var itemInstance = parameter as ItemInstance;
                if (itemInstance != null)
                    return itemInstance.statusId.Equals(statusAvailable.id);

                // either a bad parameter or status is other than Available
                return false;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in IsItemInstanceAvailable - {e.Message}");
                throw;
            }
        }

        private bool IsItemInstanceDeployed(object parameter)
        {
            try
            {
                var itemResult = parameter as ItemResult;
                if (itemResult != null)
                    return string.Equals(itemResult.status, "Deployed", StringComparison.InvariantCultureIgnoreCase);

                var itemInstance = parameter as ItemInstance;
                if (itemInstance != null)
                    return itemInstance.statusId.Equals(statusDeployed.id);

                // either a bad parameter or status is other than Deployed
                return false;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in IsItemInstanceDeployed - {e.Message}");
                throw;
            }
        }
    }
}