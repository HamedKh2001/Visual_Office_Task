﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace SharedKernel.Conventions
{
    public class DateOnlyComparer : ValueComparer<DateOnly>
    {
        public DateOnlyComparer() : base((d1, d2) => d1 == d2 && d1.DayNumber == d2.DayNumber, d => d.GetHashCode())
        {
        }
    }
}