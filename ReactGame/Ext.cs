using System;
using System.Collections.Generic;

namespace ReactGame
{
    public static class ListExtension
    {
        public static List<T> UnSort<T>(this List<T> Items)
        {
            var rand = new Random(new Random().Next());
            for(int it = 0; it < 60; it++)
            {
                int i = rand.Next(0, Items.Count);
                int j = rand.Next(0, Items.Count);
                var temp = Items[i];
                Items[i] = Items[j];
                Items[j] = temp;
            }
            return Items;
        }
    }
}
