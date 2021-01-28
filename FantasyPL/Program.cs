using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using Npgsql;

namespace FantasyPL
{
	class Program
	{
		private static NpgsqlConnection con = new NpgsqlConnection();
		private static FPL.Static listfull;
		public static Dictionary<int, string> teamNames = new Dictionary<int, string>();
		public static Dictionary<int, string> positionNames = new Dictionary<int, string>();
		public static Dictionary<int, string> playerNames = new Dictionary<int, string>();
		public static List<int> PlayersToCheck = new List<int>();

		public static string PostGresql;
		public static string LeagueCode;


		public static void Main(string[] args)
		{
			populateConnectionsfromXML();
			//populateLeague();
			populateListful();
			setDictionaries();
			//populateGWData();
			updatePlayerInfo();
			WriteElementsToDB();
			writeEventsToDB();
			Console.WriteLine();

		}

		public static void populateLeague()
		{
			Console.WriteLine("Fetching League data...");
			string htmlCode;
			FPL.FullLeague fullLeague;
			using (WebClient client = new WebClient())
			{
				client.Encoding = Encoding.UTF8;
				htmlCode = client.DownloadString(@"https://fantasy.premierleague.com/api/leagues-classic/"+LeagueCode+"/standings/?page_new_entries=1&page_standings=1&phase=1");
			}
			fullLeague = JsonConvert.DeserializeObject<FPL.FullLeague>(htmlCode);
			WriteDFCToDB(fullLeague);
		}

		public static void populateGWData()
		{
			Console.WriteLine("Fetching Game Week data...");
			foreach (var player in PlayersToCheck)
			{
				FPL.GWData GWPlayerData = populateStats(player);
				Console.WriteLine("Writing Game Week data to DB for " + playerNames[player] + "...");
				WriteGWToDB(GWPlayerData, playerNames[player]);
			}
		}

		public static void WriteGWToDB(FPL.GWData GWPlayerData, string player)
		{
			con.ConnectionString = PostGresql;
			try
			{
				con.Open();
				for (int i = 0; i < GWPlayerData.history.Length; i++)
				{
					string CommandText = Script.getGWStatsString(GWPlayerData.history[i], player);
					NpgsqlCommand updatePlayers = new NpgsqlCommand(CommandText, con);
					updatePlayers.ExecuteNonQuery();
				}
				con.Close();
			}
			catch (Exception E)
			{
				Console.WriteLine("Exception " + E.Message + " Stack trace " + E.StackTrace);
			}
		}

		public static void WriteDFCToDB(FPL.FullLeague fullLeague)
		{
			Console.WriteLine("Writing League to DB...");
			con.ConnectionString = PostGresql;
			try
			{
				con.Open();
				for (int i = 0; i < fullLeague.standings.results.Count; i++)
				{
					string CommandText = Script.getLeagueString(fullLeague.standings.results[i]);
					NpgsqlCommand updatePlayers = new NpgsqlCommand(CommandText, con);
					updatePlayers.ExecuteNonQuery();
				}
				con.Close();
			}
			catch (Exception E)
			{
				Console.WriteLine("Exception " + E.Message + " Stack trace " + E.StackTrace);
			}
		}

		public static FPL.GWData populateStats(int playernum)
		{
			string htmlCode;
			using (WebClient client = new WebClient())
			{
				client.Encoding = Encoding.UTF8;
				string StatString = "https://fantasy.premierleague.com/api/element-summary/" + playernum.ToString() + "/";
				htmlCode = client.DownloadString(@StatString);
			}
			FPL.GWData GWPlayerStats = JsonConvert.DeserializeObject<FPL.GWData>(htmlCode);

			return GWPlayerStats;
		}

		public static void populateListful()
		{
			Console.WriteLine("Fetching Static data...");
			string htmlCode;
			using (WebClient client = new WebClient())
			{
				client.Encoding = Encoding.UTF8;
				//needs to be broken down into events, game_settings, phases, teams, total_players, elements, element_stats, element_types
				htmlCode = client.DownloadString(@"https://fantasy.premierleague.com/api/bootstrap-static/");
			}
			listfull = JsonConvert.DeserializeObject<FPL.Static>(htmlCode);

		}

		public static void setDictionaries()
		{
			Console.WriteLine("Setting Dictionaries...");
			//set teams
			foreach (var team in listfull.teams)
			{
				teamNames.Add(team.id, team.name);
			}

			//set positions
			foreach (var position in listfull.element_types)
			{
				positionNames.Add(position.id, position.singular_name);
			}

			//set players
			foreach (var player in listfull.elements)
			{
				playerNames.Add(player.id, player.web_name);

				//add some to PlayersToCheck
				if (player.minutes > 500)
				{
					PlayersToCheck.Add(player.id);
				}
			}

		}

		public static void updatePlayerInfo()
		{
			Console.WriteLine("Updating Player info...");
			//Update player fields
			foreach (var player in listfull.elements)
			{
				player.position = positionNames[player.element_type];
				player.team_name = teamNames[player.team];
			}

			//update events with player names instead of IDs
			foreach (var evento in listfull.events)
			{
				if (evento.most_selected != null)
				{
					int mostsel = evento.most_selected ?? default(int);
					evento.most_selected_name = playerNames[mostsel];
				}
				if (evento.most_transferred_in != null)
				{
					int mosttrans = evento.most_transferred_in ?? default(int);
					evento.most_transferred_in_name = playerNames[mosttrans];
				}
				if (evento.most_captained != null)
				{
					int mostcap = evento.most_captained ?? default(int);
					evento.most_captained_name = playerNames[mostcap];
				}
				if (evento.top_element != null)
				{
					int topPl = evento.top_element ?? default(int);
					evento.top_element_name = playerNames[topPl];
					evento.top_element_points = evento.top_element_info.points;
				}
				if (evento.most_vice_captained != null)
				{
					int mostvc = evento.most_vice_captained ?? default(int);
					evento.most_vice_captained_name = playerNames[mostvc];
				}
				foreach (var chips in evento.chip_plays)
				{
					switch (chips.chip_name)
					{
						case "bboost":
							evento.bboost = chips.num_played;
							break;
						case "freehit":
							evento.freehit = chips.num_played;
							break;
						case "3xc":
							evento.tripCap = chips.num_played;
							break;
						case "wildcard":
							evento.wildcard = chips.num_played;
							break;
					}
				}
			}
		}

		public static void WriteElementsToDB()
		{
			Console.WriteLine("Writing Players to DB...");
			con.ConnectionString = PostGresql;
			try
			{
				con.Open();
				for (int i = 0; i < listfull.elements.Count; i++)
				{
					string CommandText = Script.getElementsString(listfull.elements[i]) ;
					NpgsqlCommand updatePlayers = new NpgsqlCommand(CommandText, con);
					updatePlayers.ExecuteNonQuery();
				}
				con.Close();
			}
			catch (Exception E)
			{
				Console.WriteLine("Exception " + E.Message + " Stack trace " + E.StackTrace);
			}
		}

		public static void writeEventsToDB()
		{
			Console.WriteLine("Writing GameWeeks to DB...");
			con.ConnectionString = PostGresql;
			try
			{
				con.Open();
				for (int i = 0; i < listfull.events.Count; i++)
				{
					string CommandText = Script.getEventsString(listfull.events[i]);
					NpgsqlCommand updatePlayers = new NpgsqlCommand(CommandText, con);
					updatePlayers.ExecuteNonQuery();
				}
				con.Close();
			}
			catch (Exception E)
			{
				Console.WriteLine("Exception " + E.Message + " Stack trace " + E.StackTrace);
			}
		}

		public static void populateConnectionsfromXML()
		{
			Console.WriteLine("Getting connection strings...");
			XmlReader xmlReader = XmlReader.Create(@"Config\config.xml", new XmlReaderSettings() { CloseInput = true });

			try
			{
				while (xmlReader.Read())
				{
					if ((xmlReader.NodeType == XmlNodeType.Element))
					{

						switch (xmlReader.Name)
						{
							case "PostGres":
								PostGresql = xmlReader.GetAttribute("value");
								break;

							case "LeagueCode":
								LeagueCode = xmlReader.GetAttribute("value");
								break;
						}
					}
				}
			}
			catch (Exception E)
			{
				Console.WriteLine("Exception " + E.Message + " Stack trace " + E.StackTrace);
			}
		}
	}
}
