﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace AdventureRoller.DatabaseContext
{
    public partial class CharacterAttributes
    {
        public Guid CharacterId { get; set; }
        public int CharacterLevel { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Dice { get; set; }

        public virtual Characters Character { get; set; }
    }
}