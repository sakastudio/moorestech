﻿using System;
using industrialization.Core.Item;
using NUnit.Framework;

namespace industrialization.Test
{
    public class ItemStackTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(0,1,0,1,2,0,0,-1)]
        [TestCase(0,5,0,1,6,0,0,-1)]
        [TestCase(-1,0,1,3,3,0,1,-1)]
        [TestCase(-1,0,2,9,9,0,2,-1)]
        [TestCase(-1,5,0,1,1,0,0,-1)]
        [TestCase(0,1,-1,0,1,0,0,-1)]
        [TestCase(0,1,-1,0,1,0,0,-1)]
        [TestCase(0,5,-1,0,5,0,0,-1)]
        [TestCase(1,1,0,8,1,8,1,0)]
        [TestCase(1,1,0,1,1,1,1,0)]
        [TestCase(2,5,5,3,5,3,2,5)]
        public void AddTest(int mid,int mamo,int rid,int ramo,int ansMAmo,int ansRAmo,int ansMid,int ansRID)
        {
            IItemStack mineItemStack;
            if (mid == -1)
            {
                mineItemStack = new NullItemStack();
            }
            else
            {
                mineItemStack = new ItemStack(mid,mamo);
            }
            IItemStack receivedItemStack;
            if (rid == -1)
            {
                receivedItemStack = new NullItemStack();
            }
            else
            {
                receivedItemStack = new ItemStack(rid,ramo);
            }
            var result = mineItemStack.AddItem(receivedItemStack);
            Assert.AreEqual(result.MineItemStack.Amount, ansMAmo);
            Assert.AreEqual(result.ReceiveItemStack.Amount, ansRAmo);
            Assert.AreEqual(result.MineItemStack.Id, ansMid);
            Assert.AreEqual(result.ReceiveItemStack.Id, ansRID);
        }

        [TestCase(0,5,1,4,0)]
        [TestCase(-1,5,1,0,-1)]
        [TestCase(0,5,10,5,0)]
        [TestCase(0,8,8,0,-1)]
        public void SubTest(int mid, int mamo, int subamo, int ansamo, int ansID)
        {
            IItemStack mineItemStack;
            if (mid == -1)
            {
                mineItemStack = new NullItemStack();
            }
            else
            {
                mineItemStack = new ItemStack(mid,mamo);
            }

            var result = mineItemStack.SubItem(subamo);            
            Assert.AreEqual(ansamo,result.Amount);
            Assert.AreEqual(ansID,result.Id);

        }

        
        [TestCase(0,50,50,0)]
        [TestCase(0,49,51,0)]
        [TestCase(0,49,52,1)]
        [TestCase(0,1,100,1)]
        [TestCase(0,60,50,10)]
        [TestCase(0,100,1,1)]
        [TestCase(0,100,100,100)]
        [TestCase(2,300,1,1)]
        [TestCase(2,1,300,1)]
        [TestCase(2,300,300,300)]
        public void ItemAddToOverFlowTest(int id,int baseAmo,int addAmo,int overflowAmo)
        {
            var baseItem = ItemStackFactory.NewItemStack(id, baseAmo);
            
            
            var result = baseItem.AddItem(ItemStackFactory.NewItemStack(id, addAmo));
            Assert.True(ItemStackFactory.NewItemStack(id,overflowAmo).Equals(result.ReceiveItemStack));
            
        }
        
        
        [TestCase(0,100,false)]
        [TestCase(0,99,false)]
        [TestCase(0,101,true)]
        [TestCase(0,110,true)]
        [TestCase(0,200,true)]
        [TestCase(1,50,false)]
        [TestCase(1,51,true)]
        [TestCase(1,100,true)]
        public void ItemAddToOverFlowThrowTest(int id,int baseAmo,bool isthrow)
        {
            try
            {
                ItemStackFactory.NewItemStack(id, baseAmo);
                Assert.False(isthrow);
            }
            catch (Exception e)
            {
                Assert.True(isthrow);
            }
        }
    }
}