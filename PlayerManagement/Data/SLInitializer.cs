using Microsoft.EntityFrameworkCore;
using PlayerManagement.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;

namespace PlayerManagement.Data
{
    public class SLInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            PlayerManagementContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<PlayerManagementContext>();
            try
            {
                //Delete the database if you need to apply a new Migration
                context.Database.EnsureDeleted();
                //Create the database if it does not exist and apply the Migration
                context.Database.Migrate();

                //This approach to seeding data uses int and string arrays with loops to
                //create the data using random values
                Random random = new Random();

                //Prepare some string arrays for building objects
                string[] firstNames = new string[] { "Lyric", "Antoinette", "Kendal", "Vivian", "Ruth", "Jamison", "Emilia", "Natalee", "Yadiel", "Jakayla", "Lukas", "Moses", "Kyler", "Karla", "Chanel", "Tyler", "Camilla", "Quintin", "Braden", "Clarence", "Dave", "Tim", "Elton", "Paul", "Shania", "Bruce" };
                string[] lastNames = new string[] { "Stovell", "Jones", "Bloggs", "Watts", "Randall", "Arias", "Weber", "Stone", "Carlson", "Robles", "Frederick", "Parker", "Morris", "Soto", "Bruce", "Orozco", "Boyer", "Burns", "Cobb", "Houston", "Rubble", "Brown", "John", "McCartney", "Twain", "Cockburn" };
                string[] playerPositions = new string[] { "Goalkeeper", "Right Fullback", "Left Fullback", "Center Back", "Holding Midfielder", "Right Midfielder", "Left Midfielder", "Box-to-Box Midfielder", "Striker", "Attacking Midfielder", "Winger" };

                #region PlayerPositions
                //PlayerPositions
                if (!context.PlayerPositions.Any())
                {
                    //loop through the array of player positions names
                    foreach (string pname in playerPositions)
                    {
                        PlayerPosition position = new PlayerPosition()
                        {
                            PlayerPos = pname
                        };
                        context.PlayerPositions.Add(position);
                    }
                    context.SaveChanges();
                }
                #endregion

                #region League
                //League
                if (!context.Leagues.Any())
                {

                    context.Leagues.AddRange(
                        new League
                        {
                            Name = "Recreational",
                            LeagueFoundation = "2015",
                            NumberOfTeams = 0,
                            LeagueBudget = 0
                        });
                    context.SaveChanges();
                }
                #endregion

                //Unique Date for Team, because is just seed data we are not using different dates
                DateTime registrationDate = new DateTime(2023, 01, 01);

                //Team names
                //string[] teamNamesIntermediate = new string[] { "Monstars", "PFC", "PDHC", "Past Our Prime", "Squad Goals", "Monarch FC", "Vas Defenses", "Sorry In Advance", "Niacon FC", "Chelsea Farm Team", "Strikers", "Proactive", "Bunny Rabbits FC", "Summer FC", "Niagara FC", "Willow FC", "Blue Jays", "TBD" };
                string[] teamNamesRecreational = new string[] { "Niagara 55ers", "Dolls and Balls", "Back That Pass Up", "Multiple Score", "Your keepers", "Threat Level Midnight", "Pink Slips", "The Tigers", "FISA FC", "InterCorsica", "Cleats Up", "Niagara Munchën", "Blue Balls FC", "Goon Squad", "Balotelli-tubbies", "BallStars FC", "Individuals 2", "For Kicks and Giggles", "Dom Pérignon", "Fake Madrid", "Beers and Balls", "Ball Busters", "Shooting Blanks", "Rum Runners" };

                #region Teams
                // Look for any Teams.
                if (!context.Teams.Any())
                {

                    //Create teams for Recreational League
                    foreach (string team in teamNamesRecreational)
                    {
                        context.Teams.Add(
                        new Team
                        {
                            Name = team,
                            RegistrationDate = registrationDate,
                            League = context.Leagues.FirstOrDefault(l => l.Name == "Recreational")
                        });
                    }

                    context.SaveChanges();

                    // get league
                    var recreationalLeague = context.Leagues.FirstOrDefault(l => l.Name == "Recreational");

                    int countTeamsRec = context.Teams.Count(t => t.League.Name == "Recreational");

                    // update number of teams and league budget for each league
                    recreationalLeague.NumberOfTeams = countTeamsRec;
                    recreationalLeague.LeagueBudget = recreationalLeague.NumberOfTeams * 1500.00;

                    context.SaveChanges();
                }
                #endregion


                //Create a collection of the primary keys of the Positions
                int[] positionIDs = context.PlayerPositions.Select(p => p.Id).ToArray();
                int positionIDCount = positionIDs.Length;

                //Create a collection of the primary keys of the Teams
                int[] teamIDs = context.Teams.Select(p => p.Id).ToArray();
                int teamIDCount = teamIDs.Length;


                #region Players
                if (!context.Players.Any())
                {
                    DateTime startDOB = DateTime.Today;// 

                    List<string> usedNames = new List<string>();

                    foreach (int teamID in teamIDs)
                    {
                        for (int i = 0; i < 13; i++)
                        {
                            string firstName = string.Empty;
                            string lastName = string.Empty;
                            bool isUniqueName = false;

                            // Generate a unique player name
                            while (!isUniqueName)
                            {
                                firstName = firstNames[random.Next(firstNames.Length)];
                                lastName = lastNames[random.Next(lastNames.Length)];
                                string fullName = firstName + lastName;

                                if (!usedNames.Contains(fullName))
                                {
                                    usedNames.Add(fullName);
                                    isUniqueName = true;
                                }
                            }

                            Player p = new Player()
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                DOB = startDOB.AddDays(-random.Next(7400, 25000)),
                                Phone = random.Next(2, 10).ToString() + random.Next(213214131, 989898989).ToString(),
                                PlayerPositionId = positionIDs[random.Next(positionIDCount)],
                                Email = $"{firstName}{lastName}@outlook.com",
                                TeamId = teamID
                            };

                            context.Players.Add(p);
                        }
                    }

                    context.SaveChanges();
                }
                #endregion

                //Create a collection of the primary keys of the Players
                int[] playerIDs = context.Players.Select(a => a.Id).ToArray();
                int playerIDCount = playerIDs.Length;

                #region Play
                    //Play
                    //Add a few positions to each player
                    if (!context.Plays.Any())
                    {
                        //i loops through the primary keys of the players
                        //j is just a counter so we add a few positions to a player
                        //k lets us step through all positions so we can make sure each gets used
                        int k = 0;//Start with the first positions
                        foreach (int i in playerIDs)
                        {
                            int howMany = random.Next(1, positionIDCount);//add a few positions to a player
                            for (int j = 1; j <= howMany; j++)
                            {
                                k = (k >= positionIDCount) ? 0 : k;//Resets counter k to 0 if we have run out of positions
                                PlayPosition p = new PlayPosition()
                                {
                                    PlayerId = i,
                                    PlayerPositionId = positionIDs[k]
                                };
                                context.Plays.Add(p);
                                k++;
                            }
                        }
                        context.SaveChanges();
                    }
                    #endregion

                #region Fields
                    if (!context.Fields.Any())
                    {
                        context.Fields.AddRange(
                            new Field
                            {
                                Name = "West Park 1",
                                Address = "78 Louth St, St Catharines, ON",
                                Comments = "Parking off of Louth Street for Field 1 & Powerview Ave for field 2",
                                GoogleMapsLink = "https://maps.google.ca/maps?oe=utf-8&client=firefox-a&ie=UTF-8&q=west+park+st.+catharines&fb=1&gl=ca&hq=west+park&hnear=0x89d35054bb6a5a4b:0x37563636c082837,St+Catharines,+ON&cid=0,0,3582645329997063709&ei=FegtUuejAsr4qAGB1IC4Cw&ved=0CIABEPwSMAo"
                            },
                            new Field
                            {
                                Name = "West Park 2",
                                Address = "78 Louth St, St Catharines, ON",
                                Comments = "Parking off of Louth Street for Field 1 & Powerview Ave for field 2",
                                GoogleMapsLink = "https://maps.google.ca/maps?oe=utf-8&client=firefox-a&ie=UTF-8&q=west+park+st.+catharines&fb=1&gl=ca&hq=west+park&hnear=0x89d35054bb6a5a4b:0x37563636c082837,St+Catharines,+ON&cid=0,0,3582645329997063709&ei=FegtUuejAsr4qAGB1IC4Cw&ved=0CIABEPwSMAo"
                            },
                            new Field
                            {
                                Name = "Berkley Park",
                                Address = "44 Ridgeview Avenue (Just off of Bunting), St. Catharines, ON",
                                Comments = "",
                                GoogleMapsLink = "http://maps.google.ca/maps?sugexp=chrome,mod%3D15&um=1&ie=UTF-8&q=berkley+park+st.+catharines+ontario&fb=1&gl=ca&hq=berkley+park+st.+catharines+ontario&hnear=berkley+park+st.+catharines+ontario&cid=0,0,1601090874100366313&ei=Tp27T8GyNcXCgAfV_JysCg&sa=X&oi=local_result&ct=image&resnum=1&ved=0CAkQ_BIwAA"
                            },
                            new Field
                            {
                                Name = "Grantham Lions Park",
                                Address = "732 Niagara Street, St Catharines, ON",
                                Comments = "",
                                GoogleMapsLink = "https://maps.google.ca/maps?oe=utf-8&client=firefox-a&channel=rcs&ie=UTF-8&q=Grantham+Lions+Park&fb=1&gl=ca&hq=Grantham+Lions+Park&cid=8286619720609870366&ei=ZL4YU9ftCIqJrAH_p4HADg&ved=0CHkQ_BIwCg"
                            },
                            new Field
                            {
                                Name = "Pearson Park West",
                                Address = "352 Niagara St, St Catharines, ON",
                                Comments = "",
                                GoogleMapsLink = "https://www.google.ca/maps/place/Lester+B.+Pearson+Park/@43.183219,-79.2246726,15z/data=!4m2!3m1!1s0x0:0x5843290380bf4151"
                            },
                            new Field
                            {
                                Name = "Pearson Park East",
                                Address = "352 Niagara St, St Catharines, ON",
                                Comments = "",
                                GoogleMapsLink = "https://www.google.ca/maps/place/Lester+B.+Pearson+Park/@43.183219,-79.2246726,15z/data=!4m2!3m1!1s0x0:0x5843290380bf4151"
                            },
                            new Field
                            {
                                Name = "Lancaster Park ",
                                Address = "31 Wood St, St Catharines, ON",
                                Comments = "",
                                GoogleMapsLink = "https://maps.google.ca/maps?oe=utf-8&rls=org.mozilla:en-GB:official&client=firefox-a&gfe_rd=cr&um=1&ie=UTF-8&fb=1&gl=ca&q=Lancaster+Park&cid=11421773781498883178&sa=X&ei=P9uRU4PqB5eNqAbVz4HICg&ved=0CJUBEPwSMA4"
                            },
                            new Field
                            {
                                Name = "Bermuda Park ",
                                Address = "16 Bermuda Drive St, St Catharines, ON",
                                Comments = "",
                                GoogleMapsLink = "https://www.google.com/maps/place/Bermuda+Dr,+St.+Catharines,+ON/@43.1911051,-79.2083083,331m/data=!3m1!1e3!4m5!3m4!1s0x89d350a4f9061ced:0x622dd0779088811f!8m2!3d43.1914735!4d-79.2087968"
                            });
                        context.SaveChanges();
                    }
                    #endregion

                //Create a collection of the primary keys of the Fields
                int[] fieldIDs = context.Fields.Select(a => a.Id).ToArray();
                int fieldIDCount = fieldIDs.Length;

                #region MatchSchedules
                if (!context.MatchSchedules.Any())
                {
                    // Times when the games are played
                    string[] matchTime = { "3:50", "5:10", "6:30", "7:50", "9:00" };

                    int matchDay = 1;

                    // Retrieve existing teams from the database
                    List<Team> teams = context.Teams.ToList();

                    // Create and add the matches to the database
                    for (int i = 0; i < teams.Count - 1; i++)
                    {
                        for (int j = i + 1; j < teams.Count; j++)
                        {
                            DateTime startDate = new DateTime(2023, 5, 28);
                            DateTime endDate = new DateTime(2023, 8, 27);
                            TimeSpan span = endDate - startDate;
                            int totalDays = span.Days;

                            // Distribute matches evenly throughout the available days
                            DateTime matchDate = startDate.AddDays((matchDay - 1) % totalDays);

                            MatchSchedule match = new MatchSchedule
                            {
                                HomeTeam = teams[i],
                                AwayTeam = teams[j],
                                Date = matchDate,
                                Time = matchTime[matchDay % matchTime.Length],
                                FieldId = (matchDay % 8) + 1, // Assign fields based on the match day
                                HomeTeamScore = 0,
                                AwayTeamScore = 0,
                                MatchDay = matchDay
                            };

                            context.MatchSchedules.Add(match);

                            matchDay++;
                        }
                    }

                    context.SaveChanges();
                }

                #endregion

                #region PlayerStats
                //Create 5 notes from Bacon ipsum
                string[] baconNotes = new string[] { "Bacon ipsum dolor amet meatball corned beef kevin, alcatra kielbasa biltong drumstick strip steak spare ribs swine." };
                    //Create collections of the primary keys of the two Parents
                    int[] matchIDs = context.MatchSchedules.Select(s => s.Id).ToArray();
                    int matchIDCount = matchIDs.Length;

                    //Appointments - the Intersection
                    //Add a few appointments to each patient
                    if (!context.PlayerMatchs.Any())
                    {
                        foreach (int i in playerIDs)
                        {
                            //i loops through the primary keys of the Patients
                            //j is just a counter so we add some Appointments to a Patient
                            //k lets us step through all AppointmentReasons so we can make sure each gets used
                            int k = 0;//Start with the first AppointmentReason
                            int howMany = random.Next(1, matchIDCount);
                            for (int j = 1; j <= howMany; j++)
                            {
                                PlayerMatch p = new PlayerMatch()
                                {
                                    PlayerId = i,
                                    MatchId = matchIDs[k],
                                    RedCards = 0,
                                    YellowCards = 0,
                                    Goals = 0,
                                    Notes = baconNotes[random.Next(1)]
                                };

                                context.PlayerMatchs.Add(p);
                                k++; // Increment k to move to the next matchID
                                if (k >= matchIDCount)
                                {
                                    k = 0; // Reset k if it exceeds the number of matchIDs
                                }
                            }
                            context.SaveChanges();
                        }
                        #endregion
                    }

                }
            
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
