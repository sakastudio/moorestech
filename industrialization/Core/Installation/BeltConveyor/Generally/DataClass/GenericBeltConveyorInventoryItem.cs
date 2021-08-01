﻿using System;

namespace industrialization.Core.Installation.BeltConveyor.Generally.DataClass
{
    public class GenericBeltConveyorInventoryItem
    {
        public GenericBeltConveyorInventoryItem(int itemId,double removalAvailableTime)
        {
            ItemID = itemId;
            InsertTime = DateTime.Now;
            RemovalAvailableTime = DateTime.Now.AddMilliseconds(removalAvailableTime);
        }

        public DateTime InsertTime { get; }
        public DateTime RemovalAvailableTime { get; }
        public int ItemID { get; }
    }
}