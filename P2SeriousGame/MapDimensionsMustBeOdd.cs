﻿using System;
using System.Runtime.Serialization;

namespace P2SeriousGame
{
    public class MapDimensionsMustBeOdd : Exception
    {
        private int value;

        public MapDimensionsMustBeOdd()
        {
        }

        public MapDimensionsMustBeOdd(string message) : base(message)
        {
        }

        public MapDimensionsMustBeOdd(int value, string message) : base(message)
        {
            this.value = value;
        }

        public MapDimensionsMustBeOdd(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}