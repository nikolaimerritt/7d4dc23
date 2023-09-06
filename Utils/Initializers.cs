using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CTFWhodunnit.Database;
using CTFWhodunnit.Models;

namespace CTFWhodunnit.Utils
{
    public static class Initializers
    {
        public static void UnlockSuspectsByTeam(AppDbContext context)
        {
            var users = context.Users.Where(u => !u.IsAdmin).ToList();
            var suspects = context.Suspects.ToList();

            int suspectsPerUser = suspects.Count / users.Count;
            int suspectIndex = 0;

            foreach (User user in users)
            {
                for (int i = 0; i < suspectsPerUser && suspectIndex < suspects.Count; i++)
                {
                    var unlockedIntel = new UnlockedIntel
                    {
                        UserId = user.UserId,
                        Suspect = suspects[suspectIndex],
                        TimeUnlocked = DateTime.Now
                    };
                    context.UnlockedIntels.Add(unlockedIntel);
                    suspectIndex++;
                }
            }
            context.SaveChanges();
        }

        public static void UnlockSuspectsForAll(AppDbContext context)
        {
            var users = context.Users.Where(u => !u.IsAdmin).ToList();
            var suspects = context.Suspects.ToList();

            foreach (User user in users)
            {
                for (int i = 0; i < suspects.Count; i++)
                {
                    var unlockedIntel = new UnlockedIntel
                    {
                        UserId = user.UserId,
                        Suspect = suspects[i],
                        TimeUnlocked = DateTime.Now
                    };
                    context.UnlockedIntels.Add(unlockedIntel);
                }
            }
            context.SaveChanges();
        }

        public static void SetFlagValues(AppDbContext context, int maxPoints)
        {
            var flags = context.Flags.OrderBy(f => f.Name).ToList();
            var currentPoints = maxPoints/2;
            for (int i = 0; i < flags.Count; i++) {
                flags[i].Points = currentPoints;
                currentPoints /= 2;
                context.Flags.Update(flags[i]);
            }
            context.SaveChanges();
        }

        public static void SetHintUnlockPoints(AppDbContext context, int maxCtfPoints)
        {
            var hints = context.Hints.ToList().OrderBy(f => Regex.Replace(f.Name, @"\d+", m => m.Value.PadLeft(10, '0'))).ToList();
            var step = maxCtfPoints/context.Hints.Count();
            for (int i = 0; i < hints.Count; i++) {
                hints[i].RequiredPoints = step*(i+1);
                context.Hints.Update(hints[i]);
            }
            context.SaveChanges();
        }
    }
}