using Microsoft.EntityFrameworkCore;
using PlayerManagement.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;

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

                DateTime registrationDate = new DateTime(2023,01,01);

                string[] teamNamesIntermediate = new string[]{"Monstars","PFC","PDHC", "Past Our Prime", "Squad Goals", "Monarch FC", "Vas Defenses", "Sorry In Advance", "Niacon FC", "Chelsea Farm Team", "Strikers", "Proactive", "Bunny Rabbits FC", "Summer FC", "Niagara FC", "Willow FC", "Blue Jays", "TBD"};
                string[] teamNamesRecreational = new string[] {"Niagara 55ers", "Dolls and Balls", "Back That Pass Up", "Multiple Scoregasms", "Your moms A keeper", "Threat Level Midnight", "Pink Slips", "The Tigers", "FISA FC", "InterCorsica", "Cleats Up", "Niagara Munchën", "Blue Balls FC", "Goon Squad", "Balotelli-tubbies", "BallStars FC", "Individuals 2", "For Kicks and Giggles", "Dom Pérignon", "Fake Madrid", "Beers and Balls", "Ball Busters", "Shooting Blanks", "Rum Runners"};


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

                //Create a collection of the primary keys of the Positions
                int[] positionIDs = context.PlayerPositions.Select(p => p.Id).ToArray();
                int positionIDCount = positionIDs.Length;

                //Create a collection of the primary keys of the Teams
                int[] teamIDs = context.Teams.Select(p => p.Id).ToArray();
                int teamIDCount = teamIDs.Length;

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
                            if(playerCount % 13 == 0)// if multiple of 13 we change team
                            {
                                teamIDCount = (teamIDCount + 1)% teamIDs.Length;
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


                //Create a collection of the primary keys of the Players
                int[] playerIDs = context.Players.Select(a => a.Id).ToArray();
                int playerIDCount = playerIDs.Length;

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
                            Play p = new Play()
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

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }

    }
}
