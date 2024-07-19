# Database:
team = { id, name, colour }
sea = { id, name }
purchase = { id, team id, points count, ships count, creation time }
round = { id, start time, end time }
move = { id, round id, team id, ships count, sea id, creation time }
outcome = { id, round id, sea id, team id, ship count }

# Computed from database:
number of ships belonging to (team) = 
    sum of ships in latest outcomes(team) 
    + sum of ships purchased by team in purchases later than the latest outcome

winner in (round, sea) = 
    team with highest ship count in round and sea 
    | null

winner = 
    team which has been winner in most rounds, seas
    | null

# API:
// API returns JSON objects. 
// JSON objects have their ID references resolved: e.g. outcome objects contain round, sea and team objects.

GET /api/outcomes(/id) // outcomes in latest round are shown on map
GET /api/teams(/id)
GET /api/seas(/id) -> { ...sea, isVisible: bool } // sea is visible to team. seas that are more than 1 sea away from team are invisible
GET /api/moves(/id) -> moves that are in seas visible to team
GET /api/scoreboard -> { team id, [sea id, number of rounds spent in control of sea] } 

PUT /api/purchase?teamId=&points= // check whether coins can be spent, have concurrency check against number of coins before spending
PUT /api/move?teamId=&seaId=&ships= // check whether move is in sea visible to team, check whether team has enough ships
