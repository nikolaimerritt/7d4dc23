using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTFWhodunnit.Models;
using CTFWhodunnit.Utils;

namespace CTFWhodunnit.Database;

public static class DbInitializer
{

    public static readonly int DEFAULT_MAX_CTF_POINTS = 100;
    public static readonly int DEFAULT_MAX_FLAG_POINTS = 100; 
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if(!context.AppConfigs.Any()) {
            var conf = new AppConfig
            {
                Name=AppConfig.CTF_ID_KEY,
                Value="93"
            };
            context.AppConfigs.Add(conf);

            conf = new AppConfig
            {
                Name=AppConfig.TEAM_VIEW_KEY,
                Value="false"
            };
            context.AppConfigs.Add(conf);

            conf = new AppConfig
            {
                Name=AppConfig.MAX_FLAG_POINTS_KEY,
                Value=DEFAULT_MAX_FLAG_POINTS.ToString()
            };
            context.AppConfigs.Add(conf);

            conf = new AppConfig
            {
                Name=AppConfig.MAX_CTF_POINTS_KEY,
                Value=DEFAULT_MAX_CTF_POINTS.ToString()
            };
            context.AppConfigs.Add(conf);

            conf = new AppConfig
            {
                Name=AppConfig.PLAYGROUND_LEADERBOARD_URL_KEY,
                Value="https://playground.withsecure.com/api/public/ctf/$ctfEventId/leaderboard"
            };
            context.AppConfigs.Add(conf);

            conf = new AppConfig
            {
                Name=AppConfig.VIDEO_URL_KEY,
                Value="https://pgmediauk.blob.core.windows.net/ctfs/Video-v2.mp4?sp=r&st=2023-09-05T16:41:41Z&se=2023-11-10T01:41:41Z&spr=https&sv=2022-11-02&sr=b&sig=egD4oauVLJmnscbwzcaRcxrZfmagL%2BQR080V%2FQz44gA%3D"
            };
            context.AppConfigs.Add(conf);
            context.SaveChanges();
        }

        if (!context.Users.Any())
        {
            var user = new User
            {
                Username = "admin",
                Password = "MWRPassword2", // Remember to hash & salt in a real-world scenario
                IsAdmin = true
            };
            context.Users.Add(user);

            user = new User
            {
                Username = "test",
                Password = "password" // Remember to hash & salt in a real-world scenario
            };
            context.Users.Add(user);

            user = new User
            {
                Username = "test2",
                Password = "password" // Remember to hash & salt in a real-world scenario
            };
            context.Users.Add(user);

            user = new User
            {
                Username = "test3",
                Password = "password" // Remember to hash & salt in a real-world scenario
            };
            context.Users.Add(user);

            user = new User
            {
                Username = "test2",
                Password = "password" // Remember to hash & salt in a real-world scenario
            };
            context.Users.Add(user);

            user = new User
            {
                Username = "poxeyey306",
                Password = "password" // Remember to hash & salt in a real-world scenario
            };
            context.Users.Add(user);

            context.SaveChanges();
        }

        if (!context.Hints.Any())
        {
            var hint = new Hint

            {

                Name = "Hint 1",

                Text = "The hacker operates from a country in Asia."

            };

            context.Hints.Add(hint);


            hint = new Hint

            {

                Name = "Hint 2",

                Text = "The hacker uses a Unix-based operating system."

            };

            context.Hints.Add(hint);


            hint = new Hint

            {

                Name = "Hint 3",

                Text = "The hacker's skills include Social Engineering."

            };

            context.Hints.Add(hint);


            hint = new Hint

            {

                Name = "Hint 4",

                Text = "The hacker does not use Ubuntu."

            };

            context.Hints.Add(hint);


            hint = new Hint

            {

                Name = "Hint 5",

                Text = "The hacker's name has a connection to a historical or mythical warrior."

            };

            context.Hints.Add(hint);


            hint = new Hint

            {

                Name = "Hint 6",

                Text = "The hacker is not from China."

            };

            context.Hints.Add(hint);


            hint = new Hint

            {

                Name = "Hint 7",
                Text = "The hacker does not use Arch Linux."

            };

            context.Hints.Add(hint);


            hint = new Hint

            {

                Name = "Hint 8",
                Text = "The hacker's skills do not include AppSec."

            };

            context.Hints.Add(hint);


            hint = new Hint

            {

                Name = "Hint 9",
                Text = "The hacker's name does not start with 'B'."

            };

            context.Hints.Add(hint);


            hint = new Hint

            {

                Name = "Hint 10",
                Text = "The hacker is from Japan."

            };
            context.Hints.Add(hint);

            context.SaveChanges();

            Initializers.SetHintUnlockPoints(context, DEFAULT_MAX_CTF_POINTS);
        }

        if (!context.Suspects.Any())
        {
            var suspect = new Suspect

            {

                Name = "Byte Crusader",

                Location = "USA",

                OperatingSystem = "Arch Linux",

                Skills = "AppSec, NetSec, Crypto, Reversing",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Data Phantom",

                Location = "Russia",

                OperatingSystem = "Ubuntu",

                Skills = "Crypto, Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Code Ninja",

                Location = "Japan",

                OperatingSystem = "FreeBSD",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Ghost Protocol",

                Location = "UK",

                OperatingSystem = "MacOS 10",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Quantum Cipher",

                Location = "Germany",

                OperatingSystem = "Windows 11",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Binary Shadow",

                Location = "China",

                OperatingSystem = "Arch Linux",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Silicon Spectre",

                Location = "India",

                OperatingSystem = "Ubuntu",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Digital Samurai",

                Location = "Japan",

                OperatingSystem = "FreeBSD",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = true,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Pixel Phantom",

                Location = "Russia",

                OperatingSystem = "MacOS 10",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Quantum Quark",

                Location = "USA",

                OperatingSystem = "Windows 11",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Bit Bandit",

                Location = "UK",

                OperatingSystem = "Arch Linux",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Cyber Centurion",

                Location = "Germany",

                OperatingSystem = "Ubuntu",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Trojan Templar",

                Location = "China",

                OperatingSystem = "FreeBSD",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Firewall Phantom",

                Location = "India",

                OperatingSystem = "MacOS 10",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Kernel Knight",

                Location = "Japan",

                OperatingSystem = "Windows 11",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Packet Paladin",

                Location = "Russia",

                OperatingSystem = "Arch Linux",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Script Sorcerer",

                Location = "USA",

                OperatingSystem = "Ubuntu",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Byte Barbarian",

                Location = "UK",

                OperatingSystem = "FreeBSD",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Data Druid",

                Location = "Germany",

                OperatingSystem = "MacOS 10",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Crypto Crusader",

                Location = "China",

                OperatingSystem = "Windows 11",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Network Nomad",

                Location = "India",

                OperatingSystem = "Arch Linux",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Silicon Shaman",

                Location = "Japan",

                OperatingSystem = "Ubuntu",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Digital Druid",

                Location = "Russia",

                OperatingSystem = "FreeBSD",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Quantum Quester",

                Location = "USA",

                OperatingSystem = "MacOS 10",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Bit Buccaneer",

                Location = "UK",

                OperatingSystem = "Windows 11",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Cyber Corsair",

                Location = "Germany",

                OperatingSystem = "Arch Linux",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Trojan Trickster",

                Location = "China",

                OperatingSystem = "Ubuntu",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Firewall Fencer",

                Location = "India",

                OperatingSystem = "FreeBSD",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Kernel King",

                Location = "Japan",

                OperatingSystem = "MacOS 10",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Packet Prince",

                Location = "Russia",

                OperatingSystem = "Windows 11",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Script Scribe",

                Location = "USA",

                OperatingSystem = "Arch Linux",

                Skills = "AppSec, NetSec, Crypto",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);


            suspect = new Suspect

            {

                Name = "Byte Brawler",

                Location = "UK",

                OperatingSystem = "Ubuntu",

                Skills = "Reversing, Malware, Social Engineering",

                IsCulprit = false,

            };

            context.Suspects.Add(suspect);
            context.SaveChanges();
        }

        if (!context.Flags.Any())
        {
            var flag = new Flag
            {
                Name = "Flag 1",
                Value = "coffee-razor54",
            };
            context.Flags.Add(flag);

            flag = new Flag
            {
                Name = "Flag 2",
                Value = "market-tape12",
            };
            context.Flags.Add(flag);

            flag = new Flag
            {
                Name = "Flag 3",
                Value = "fan-microwave75",
            };
            context.Flags.Add(flag);

            flag = new Flag
            {
                Name = "Flag 4",
                Value = "socket-bedroom89",
            };
            context.Flags.Add(flag);

            flag = new Flag
            {
                Name = "Flag 5",
                Value = "casket-pillow21",
            };
            context.Flags.Add(flag);


            context.SaveChanges();
            Initializers.SetFlagValues(context, DEFAULT_MAX_FLAG_POINTS);
        }

        if (!context.UnlockedIntels.Any()){
            Initializers.UnlockSuspectsByTeam(context);
        }
        
    }

    
}