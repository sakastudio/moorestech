﻿using System.Runtime.Serialization;
using industrialization.Item;

namespace industrialization.Config
{
    [DataContract] 
    public class MachineRecipes
    {
        
        [DataMember(Name = "recipes")]
        private MachineRecipe[] recipes;

        public MachineRecipe[] Recipes => recipes;
    }

    [DataContract] 
    public class MachineRecipe
    {
        [DataMember(Name = "installationID")]
        private int installationID;
        [DataMember(Name = "time")]
        private double time;
        [DataMember(Name = "input")]
        private MacineRecipeInput[] itemInputs;
        [DataMember(Name = "output")]
        private MacineRecipeOutput[] itemOutputs;

        public MacineRecipeOutput[] ItemOutputs => itemOutputs;

        public MacineRecipeInput[] ItemInputs => itemInputs;

        public double Time => time;

        public int InstallationId => installationID;

        //TODO ここ実装する
        public bool RecipeConfirmation(IItemStack[] InputSlot)
        {
            return false;
        }
    }

    [DataContract] 
    public class MacineRecipeInput
    {
        [DataMember(Name = "id")]
        private int itemID;
        [DataMember(Name = "amount")]
        private int amount;


        public ItemStack ItemStack
        {
            get
            {
                return new ItemStack(itemID, amount);
            }
        }
    }

    [DataContract] 
    public class MacineRecipeOutput
    {
        [DataMember(Name = "id")]
        private int itemID;
        [DataMember(Name = "amount")]
        private int amount;
        [DataMember(Name = "percent")]
        private double percent;

        public double Percent => percent;

        public ItemStack ItemStack
        {
            get
            {
                return new ItemStack(itemID, amount);
            }
        }
    }
}