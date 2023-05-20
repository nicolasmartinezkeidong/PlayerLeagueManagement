using Microsoft.EntityFrameworkCore;
using PlayerManagement.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
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
                string[] lastsNames = new string[] { "Stovell", "Jones", "Bloggs", "Watts", "Randall", "Arias", "Weber", "Stone", "Carlson", "Robles", "Frederick", "Parker", "Morris", "Soto", "Bruce", "Orozco", "Boyer", "Burns", "Cobb", "Houston", "Rubble", "Brown", "John", "McCartney", "Twain", "Cockburn" };
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
                            Name = "Intermediate",
                            LeagueFoundation = "2020",
                            NumberOfTeams = 0,
                            LeagueBudget = 0
                        },
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
                string[] teamNamesIntermediate = new string[] { "Monstars", "PFC", "PDHC", "Past Our Prime", "Squad Goals", "Monarch FC", "Vas Defenses", "Sorry In Advance", "Niacon FC", "Chelsea Farm Team", "Strikers", "Proactive", "Bunny Rabbits FC", "Summer FC", "Niagara FC", "Willow FC", "Blue Jays", "TBD" };
                string[] teamNamesRecreational = new string[] { "Niagara 55ers", "Dolls and Balls", "Back That Pass Up", "Multiple Score", "Your keepers", "Threat Level Midnight", "Pink Slips", "The Tigers", "FISA FC", "InterCorsica", "Cleats Up", "Niagara Munchën", "Blue Balls FC", "Goon Squad", "Balotelli-tubbies", "BallStars FC", "Individuals 2", "For Kicks and Giggles", "Dom Pérignon", "Fake Madrid", "Beers and Balls", "Ball Busters", "Shooting Blanks", "Rum Runners" };

                #region Teams
                // Look for any Teams.
                if (!context.Teams.Any())
                {
                    //Create teams for Intermediate League
                    foreach (string team in teamNamesIntermediate)
                    {
                        context.Teams.Add(
                        new Team
                        {
                            Name = team,
                            RegistrationDate = registrationDate,
                            League = context.Leagues.FirstOrDefault(l => l.Name == "Intermediate")
                        });
                    }
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

                    // get leagues
                    var intermediateLeague = context.Leagues.FirstOrDefault(l => l.Name == "Intermediate");
                    var recreationalLeague = context.Leagues.FirstOrDefault(l => l.Name == "Recreational");

                    int countTeamsInt = context.Teams.Count(t => t.League.Name == "Intermediate");
                    int countTeamsRec = context.Teams.Count(t => t.League.Name == "Recreational");

                    // update number of teams and league budget for each league
                    intermediateLeague.NumberOfTeams = countTeamsInt;
                    intermediateLeague.LeagueBudget = intermediateLeague.NumberOfTeams * 1500.00;

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
                //Look for any Player
                if (!context.Players.Any())
                {
                    // Start birthdate for randomly produced players 
                    // We will subtract a random number of days from today
                    DateTime startDOB = DateTime.Today;


                    //Double loop through the arrays of names 
                    //to build Players as the loops is running
                    int playerCount = 0;

                    //Double loop through the arrays of names 
                    //and build the Player as we go
                    foreach (string f in firstNames)
                    {
                        foreach (string l in lastsNames)
                        {
                            if (playerCount % 13 == 0)// if multiple of 13 we change team
                            {
                                teamIDCount = (teamIDCount + 1) % teamIDs.Length;
                            }
                            Player p = new Player()
                            {
                                FirstName = f,
                                LastName = l,
                                DOB = startDOB.AddDays(-random.Next(7400, 25000)),
                                Phone = random.Next(2, 10).ToString() + random.Next(213214131, 989898989).ToString(),
                                PlayerPositionId = positionIDs[random.Next(positionIDCount)],
                                Email = $"{f}{l}@outlook.com",
                                TeamId = teamIDs[random.Next(teamIDCount)]
                            };
                            context.Players.Add(p);

                            playerCount++;
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
                    //Times when the games are played
                    string[] matchTime = { "3:50", "5:10", "6:30", "7:50", "9:00" };
                    //Unique date to sample the seed data for match 1 date
                    DateTime matchDate = new DateTime(2022, 05, 29);

                    List<string> availableTeams = teamNamesIntermediate.ToList();
                    List<string> scheduledTeams = new List<string>();

                    //name of the last home team scheduled
                    string lastHomeTeam = null;
                    //name of the last away team scheduled
                    string lastAwayTeam = null;


                    //hold scheduled match times for each field
                    Dictionary<int, List<string>> scheduledTimes = new Dictionary<int, List<string>>();
                    foreach (int fieldId in fieldIDs)
                    {
                        scheduledTimes.Add(fieldId, new List<string>());
                    }

                    //Create teams for Intermediate League
                    foreach (string homeTeam in teamNamesIntermediate)
                    {
                        foreach (string times in matchTime)
                        {
                            foreach (int fields in fieldIDs)
                            {
                                //Check if this time has already been scheduled for this field
                                if (scheduledTimes[fields].Contains(times))
                                    continue;

                                //Check if this home team has already played in the last match or is the same as the last away team
                                if (homeTeam == lastHomeTeam || homeTeam == lastAwayTeam)
                                    continue;


                                // Remove the home team from the available teams list
                                availableTeams.Remove(homeTeam);
                                // Get the first available away team that is not the same as the home team, the last home team, or the last away team
                                string awayTeam = availableTeams.FirstOrDefault(t => t != homeTeam && t != lastHomeTeam && t != lastAwayTeam);

                                if (awayTeam != null)
                                {
                                    scheduledTeams.Add(homeTeam);
                                    scheduledTeams.Add(awayTeam);
                                    availableTeams.Remove(awayTeam);

                                    context.MatchSchedules.Add(
                                    new MatchSchedule
                                    {
                                        Date = matchDate,
                                        Time = times,
                                        FieldId = fields,
                                        HomeTeamId = context.Teams.FirstOrDefault(l => l.Name == homeTeam).Id,
                                        AwayTeamId = context.Teams.FirstOrDefault(l => l.Name == awayTeam).Id,
                                        HomeTeamScore = random.Next(0, 7),
                                        AwayTeamScore = random.Next(0, 7)
                                    });

                                    //Add the scheduled time to the list for this field
                                    scheduledTimes[fields].Add(times);

                                    lastHomeTeam = homeTeam;
                                    lastAwayTeam = awayTeam;
                                }
                                else
                                {
                                    // All available teams have been scheduled, so break the loop
                                    break;
                                }
                            }
                            context.SaveChanges();
                        }

                    }

                    List<string> availableTeamsRec = teamNamesRecreational.ToList();

                    string lastHomeTeamRec = null;
                    string lastAwayTeamRec = null;

                    //Create teams for Recreational League
                    foreach (string homeTeam in teamNamesRecreational)
                    {
                        foreach(string times in matchTime)
                        {
                            foreach(int fields in fieldIDs)
                            {
                                if (scheduledTimes[fields].Contains(times))
                                    continue;

                                if (homeTeam == lastHomeTeamRec || homeTeam == lastAwayTeamRec)
                                    continue;
                                availableTeamsRec.Remove(homeTeam);
                                string awayTeam = availableTeamsRec.FirstOrDefault(t => t != homeTeam && t != lastHomeTeamRec && t != lastAwayTeamRec);


                                if (awayTeam != null)
                                {
                                    scheduledTeams.Add(homeTeam);
                                    scheduledTeams.Add(awayTeam);
                                    availableTeamsRec.Remove(awayTeam);
                                    context.MatchSchedules.Add(
                                    new MatchSchedule
                                    {
                                        Date = matchDate,
                                        Time = matchTime[random.Next(matchTime.Length)],
                                        FieldId = fieldIDs[random.Next(fieldIDCount)],
                                        HomeTeamId = context.Teams.FirstOrDefault(l => l.Name == homeTeam).Id,
                                        AwayTeamId = context.Teams.FirstOrDefault(l => l.Name == awayTeam).Id,
                                        HomeTeamScore = random.Next(0, 7),
                                        AwayTeamScore = random.Next(0, 7)
                                    });

                                    lastHomeTeamRec = homeTeam;
                                    lastAwayTeamRec = awayTeam;
                                }
                                else
                                {
                                    // All available teams have been scheduled, so break the loop
                                    break;
                                }

                            }
                            context.SaveChanges();
                        }
                        
                    }
                    
                }
                #endregion

                

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
}

    }
}
