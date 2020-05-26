Developers:
Derek Dixon (U0726538)
Matt Pham (U0952138)

TODO pulled from PS9 page:

-----As always, the README file should be updated to include your current status. Create a section at the top of your README called "Database Description".

The Database Description should describe your database architecture. This section should include (1) descriptions of the table(s) contained in your database, and (2) example SQL code corresponding to each query (i.e., inserts, updates, and removes) performed on the database by your server.

Software Engineering Tip: It is very important to document the DB relationship to a program as this information can easily be lost and is hard to "infer" from the code.-----

Log:

Added SpaceWars File to this repo the client and server are in here also

11/9

Pulled code from fancy chat client, got server talking with client

11/10

Figured out how to pull data from server. With this data we inititally create a ship class, after each pull, we reassign the values to the specific ship by 
calling a 4(?)-argument constructor.

TODO: Get one ship fully working

Update 11/16 15:00

World is drawn, ship is updating with values appropriately. 

TODO 11/16

Pass action requests to the server (move/shoot)

'R' - turn right = Right arrow key pressed
'L' - turn left = Left arrow key pressed
'T' - engine thrust = Up arrow key pressed
'F' - fire projectile = Spacebar pressed

Update:11/16 17:22

Now passing input to server and server is updated accordingly.

TODO:

Have holding left CONSTANTLY send the turn left input, stop turning left on key_up for left

Update:11/17 16:16

Controls now work by holding down arrow keys and spacebar, removed dialog boxes that were networking also increased the framerate of our client drastically.

TODO:

Projectiles are not displaying as compared to Kopta's code.
Implement Scoreboard
Implement Name before connecting
Error handling when connecting to server

11/17 17:33

Seperate code logic in order to follow proper MVC policy

Implement scoreboard

Implement death animation

Try/Catch statement on redraw

Require name on connect

Non-default world size

Polish -> sort scoreboard by score 
Health color

11/18

made recieve start up just like in network Diagram

made it so that you had to have a name to actually connect to server

Made it when a ship dies it stops being drawn

small error handling in redraw

11/19 

Made star animated (Polish)

made death animation(Polish)

Screen resizes depending on server

Last thing we have to do is scoreboard:

11/20

Scoreboard now works

All text boxes and the connect button dissappear when properly connecting to localhost(Polish)

Cleaned up code via removing unused objects

Added comments

------------------------------------------------------------------------------------------------------------------------------------------------
Server Side
------------------------------------------------------------------------------------------------------------------------------------------------
Set up inital event loop
Need to get TCPListener working.

12/1

Event loop working with TCP Listener

TODO 12/1

Setup a way to check for input requests, believe that using the socketstates stored in our clients list is the key to this

UPDATE 12/6

Able to parse through individual player command requests, need to now find a way to attach a socketstate to its 
respective ship

TODO

Take client command requests and apply them to the ship

UPDATE 12/7

Server now accepting and applying command requests to their respective ships.

TODO

Find a way to stop thrusting when 'F' is not passed in
Remove ability to rotate death animation
Implement projectiles, collission detection, score updating and new game mode for poilish (ships collide with eachother/
star fires projectiles/non-stop thursting) this needs to be set from the .xml file in order to decide the game mode.


Final:

got multiple command input working for one ship still unsure why AI client doesn't work.
better respawn mechanic
implemented fire rate
stopped bug where ships dont respanw and if they do they are unresposnvie. 
cleaned up projectiles so they"die"

Got the mySQL command working just hit ctrl + c to add that to the database
made three different tables one for unique game ID and time and date of wehn game is palyed
another one wiht game ID , player ID and playername
and a final table with player ID and stats

got the scores working for the http server
nothing else is jsut psedo code 
couldn't figure out how to parse everything

