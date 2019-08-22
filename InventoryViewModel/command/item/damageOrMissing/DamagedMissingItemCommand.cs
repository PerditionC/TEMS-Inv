// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.userManager;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// An item(including any contained items if item is a container)
    /// in inventory have been deployed (taken to be used)
    /// </summary>
    public sealed class DamagedMissingItemCommand : UpdateItemStatusCommand
    {
        public DamagedMissingItemCommand(DamageMissingEventType eventType) : base()
        {
            _execute = DamagedOrMissingItem;
            _canExecute = IsItemInstanceNotDamagedOrMissing;

            this.eventType = eventType;
            _reportedBy = UserManager.GetUserManager.CurrentUser().fullName;
            _discoveryDate = DateTime.Now.Date;
        }

        /// <summary>
        /// Does this ICommand represent a damage or a missing event action
        /// </summary>
        private DamageMissingEventType eventType;

        // who (name, possibly contact info - may be used for auto-complete values) reported event
        public string ReportedBy { get { return _reportedBy; } set { SetProperty(ref _reportedBy, value, nameof(ReportedBy)); } }

        private string _reportedBy;

        // when issue was identified/reported, default to today
        public DateTime DiscoveryDate { get { return _discoveryDate; } set { SetProperty(ref _discoveryDate, value, nameof(DiscoveryDate)); } }

        private DateTime _discoveryDate;

        /// <summary>
        /// if parameter represents an ItemInstance and has status other than Damaged/Missing
        /// Note: a damaged item can still be marked missing, but a missing item
        /// can not be marked damaged i.e. for damage returns true if not damaged or missing
        /// but for missing just returns not already missing (okay to be damaged)
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool IsItemInstanceNotDamagedOrMissing(object parameter)
        {
            // we can still mark a damaged item as missing, but can't mark a missing item as damaged
            var notAllMissing = !IsAllItemInstanceStatus(parameter, GetItemStatus("Missing"));
            if (eventType == DamageMissingEventType.Missing)
            {
                return notAllMissing;
            }
            else
            {
                var notAllDamaged = !IsAllItemInstanceStatus(parameter, GetItemStatus("Damaged"));
                return notAllDamaged && notAllMissing;
            }
        }

        /// <summary>
        /// updates item(s) to damaged and creates new damaged/missing event
        /// </summary>
        /// <param name="parameters"></param>
        private void DamagedOrMissingItem(object parameter)
        {
            logger.Trace(nameof(DamagedOrMissingItem));
            ItemStatus status;
            if (eventType == DamageMissingEventType.Damage)
                status = GetItemStatus("Damaged");
            else // == DamageMissingEventType.Missing
                status = GetItemStatus("Missing");

            try
            {
                if (parameter is ItemInstance itemInstance)
                {
                    UpdateStatus(itemInstance, status, CheckEvent(itemInstance, status, GetNewEvent(itemInstance)));
                    return;
                }

                var db = DataRepository.GetDataRepository;

                if (parameter is GenericItemResult itemResult)
                {
                    var item = db.Load<ItemInstance>(itemResult.id);
                    UpdateStatus(item, status, GetNewEvent(item));
                    return;
                }

                // user selected a list of damaged items
                if (parameter is IList<GenericItemResult> items)
                {
                    foreach (var i in items)
                    {
                        var item = db.Load<ItemInstance>(i.id);
                        UpdateStatus(item, status, GetNewEvent(item));
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in {nameof(DamagedOrMissingItem)} - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// creates a new Event to store information about this action
        /// </summary>
        /// <param name="itemInstance"></param>
        /// <returns></returns>
        private DamageMissingEvent GetNewEvent(ItemInstance itemInstance)
        {
            var damageMissingEvent = new DamageMissingEvent
            {
                // set default/common values, specific event values set by caller
                itemInstance = itemInstance,
                inputBy = UserManager.GetUserManager.CurrentUser().userId,
                reportedBy = ReportedBy,
                discoveryDate = DiscoveryDate,
                eventType = eventType,
                notes = Notes
            };
            return damageMissingEvent;
        }
    }
}