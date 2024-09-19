## Docker

The easiest way to run this is to use docker. Start by creating the container:

    docker build -t whudunnit -f Dockerfile .

Then run the container:

    docker run -p 80:80 --restart always --name whudunnit-app whudunnit

You can access the app at http://localhost:80.

# Documentation
## Entities
The entities are:
- Teams
- Seas
- Rounds
- Moves
- Purchases
- Outcomes
- Messages

## Gameplay
Teams compete for the ownership of seas, by deploying ships purchased with points gained from CTFs. The winner is the team that has held the most amount of seas, taking into account all the rounds.

Each team starts off with ten ships in their allocated starting sea.

The gameplay is split into multiple consecutive rounds. Each round has a planning phase, followed by a cooldown phase. 

During the planning phase, teams can move ships across different seas in their sphere of influence, and use points gained from solving flags on Playground to purchase ships in their sphere of influence. A team's sphere of influence is seas that the team had ships in at the end of the last round, along with the seas next to those seas.

During the cooldown phase, if a sea has ships from more than one team, these teams fight in that sea. During the fight calculations, each team receives a small semi-random increase in ships, to make gameplay interesting and to ensure that there are no ties. At the end of the round, whichever team had the most ships in that sea wins that sea. The winning team will lose ships equal to the total of all the other ships in the sea, but it will never have less than one ship. All other teams in that sea lose all their ships.

For example, if the Southern ocean contains 4 ships from Team Drake, 6 ships from Team Blackbeard, and 8 ships from Team Read, then Team Read wins. Team Read would have ended up with `8 - (4 + 6) = ` `-2` ships, but since this is less than 1 ship, Team Read ends up with 1 ship. As they are not the winner, Team Drake and Team Blackbeard lose all their ships in that sea. 

Teams can message each other during the game. A notification icon appears when a team has an unread message.

## UI
Teams can view:
- A live map of the seven seas,
- A live leaderboard,
- A live record of the actions teams have taken in the past, plus the rounds schedule.

## Architecture
The server is written in C# following the MVC pattern, and is backed by a SQLite database. The frontend is written in Vue, attached to server-side-rendered pages. All live pages are implemented with polling.

When the server starts, it waits until all the configuration entries are present in the database, which include configurations for round timings. When this is present, the server declares all rounds in the database in advance. The server then schedules the OutcomeService to write each round's outcomes (see below) just before the end of the round. Only then does the server start serving pages. **If the game isn't starting, this is probably why!**

During each round's planning phase, the server accepts requests from teams to purchase ships with points gained from CTFs, and requests to move ships.

Just before the end of each round, the server takes into account the actions players made in each sea, and calculates the result of team fights. These are recorded as Outcomes. Outcomes record, within a round, how many ships a team has in a given sea. This is how gameplay is recorded. Previous outcomes are used to calculate the Outcomes for subsequent rounds.

The frontend used to request outcomes to display the live map, but outcomes turned out to be too granular and things got messy. Instead, the server exposes an endpoint for "Sea States". This endpoint collates the latest outcomes in a given sea. The server presents this as a "Sea state", which is a list of `(team, ship count)` pairs within that sea -- much easier to work with! The frontend still uses outcomes to display the "Captain's Log" page though, as this is basically a list of outcomes.

## If things go wrong
The server returns the stack trace in error 500 responses, so this should help find any issue.

If the game isn't starting at all, make sure that all the configuration entries are present in the database. The server waits for all of these to be present before serving any pages.

If things go wrong during gameplay, the first thing to do is to check the "Captain's Log" page for anything strange. I'd also check the current round, and see if the bug could be to do with the current round being in its cooldown phase. 

I have tested this game thoroughly (I promise!)
