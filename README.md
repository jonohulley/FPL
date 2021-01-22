# FPL
 
I created this to allow myself to scrape Fantasy Premier League data into a Postgresql database.
It scrapes the following data:
- Player Overview Data
- Gameweek data
- Player history data
- Personal League data

Use the following steps below:
1. Create a Postgresql Database.
2. Edit the config.xml file with the correct connection string to connect to your db
3. Run setup.exe to choose what tables you would like in your database. This will generate a script called CreateTables.sql
4. Open your Postgres Database and run the script called CreateTables.sql to create the SQL tables
5. Lastly, run FantasyPL.exe and the data will be populated in your database. 

