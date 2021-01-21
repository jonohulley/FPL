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
		private static OleDbConnection primaryDatabaseConnection = new OleDbConnection();
		private static NpgsqlConnection con = new NpgsqlConnection();
		private static FPL.Static listfull;
		public static Dictionary<int, string> teamNames = new Dictionary<int, string>();
		public static Dictionary<int, string> positionNames = new Dictionary<int, string>();
		public static Dictionary<int, string> playerNames = new Dictionary<int, string>();
		public static List<int> PlayersToCheck = new List<int>();

		public static string PostGresql;
		public static string MSSQL;


		public static void Main(string[] args)
		{
			populateConnectionsfromXML();
			populateLeague();
			populateListful();
			setDictionaries();
			populateGWData();
			updatePlayerInfo();
			WriteElementsToDB();
			writeEventsToDB();
			Console.WriteLine();


		}

		public static void populateLeague()
		{
			Console.WriteLine("Fetching Dash for Cash data...");
			string htmlCode;
			FPL.FullLeague fullLeague;
			using (WebClient client = new WebClient())
			{
				client.Encoding = Encoding.UTF8;
				htmlCode = client.DownloadString(@"https://fantasy.premierleague.com/api/leagues-classic/319719/standings/?page_new_entries=1&page_standings=1&phase=1");
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
				WriteGWToDB(GWPlayerData);
			}
		}

		public static void WriteGWToDB(FPL.GWData GWPlayerData)
		{
			con.ConnectionString = PostGresql;
			try
			{
				con.Open();
				for (int i = 0; i < GWPlayerData.history.Length; i++)
				{
					string CommandText = getGWStatsString(i, GWPlayerData.history);
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
			Console.WriteLine("Writing Dash for Cash to DB...");
			con.ConnectionString = PostGresql;
			try
			{
				con.Open();
				for (int i = 0; i < fullLeague.standings.results.Count; i++)
				{
					string CommandText = getDFCString(i, fullLeague);
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
					string CommandText = getElementsString(i);
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
			con.ConnectionString = PostGresql;
			try
			{
				con.Open();
				for (int i = 0; i < listfull.events.Count; i++)
				{
					string CommandText = getEventsString(i);
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

		public static string getEventsString(int i)
		{
			
			string eventstring = string.Format("INSERT INTO events (id, name, deadline_time, average_entry_score, finished, data_checked, highest_scoring_entry" +
							", deadline_time_epoch, deadline_time_game_offset, highest_score, is_previous, is_current, is_next, bboost_played" +
							", trip_cap_played, wildcard_played, freehit_played, most_selected, most_transferred_in, top_player, top_player_points" +
							", transfers_made, most_captained, most_vice_captained)" +
							"VALUES ({0},'{1}', '{2}', {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, '{17}', '{18}', '{19}', {20}, {21}, '{22}', '{23}')" +
							"ON CONFLICT (id) DO UPDATE " +
							"SET name = '{1}', deadline_time = '{2}', average_entry_score = {3},finished = {4},data_checked = {5}" +
							",highest_scoring_entry = {6},deadline_time_epoch = {7},deadline_time_game_offset = {8},highest_score = {9}" +
							",is_previous = {10},is_current = {11},is_next = {12},bboost_played = {13},trip_cap_played = {14}" +
							",wildcard_played = {15},freehit_played = {16},most_selected = '{17}',most_transferred_in = '{18}'" +
							",top_player = '{19}',top_player_points = {20},transfers_made = {21},most_captained = '{22}',most_vice_captained = '{23}';"
							, listfull.events[i].id, listfull.events[i].name, listfull.events[i].deadline_time, listfull.events[i].average_entry_score, Convert.ToBoolean(listfull.events[i].finished),
							Convert.ToBoolean(listfull.events[i].data_checked), listfull.events[i].highest_scoring_entry == null ? 0 : listfull.events[i].highest_scoring_entry, listfull.events[i].deadline_time_epoch, 
							listfull.events[i].deadline_time_game_offset, listfull.events[i].highest_score == null ? 0 : listfull.events[i].highest_score, Convert.ToBoolean(listfull.events[i].is_previous), 
							Convert.ToBoolean(listfull.events[i].is_current), Convert.ToBoolean(listfull.events[i].is_next), listfull.events[i].bboost, listfull.events[i].tripCap, listfull.events[i].wildcard, 
							listfull.events[i].freehit, listfull.events[i].most_selected_name == null ? "" : listfull.events[i].most_selected_name, 
							listfull.events[i].most_transferred_in_name == null ? "" : listfull.events[i].most_transferred_in_name, listfull.events[i].top_element_name,listfull.events[i].top_element_points, 
							listfull.events[i].transfers_made, listfull.events[i].most_captained_name == null ? "" : listfull.events[i].most_captained_name, listfull.events[i].most_vice_captained_name == null ? "" : listfull.events[i].most_vice_captained_name);

			return eventstring;
		}

		public static string getElementsString(int i)
		{

			string elementstring = string.Format("INSERT INTO players (id" +
					", web_name, first_name, second_name, chance_of_playing_next_round, chance_of_playing_this_round" +
					", code, cost_change_event, cost_change_event_fall, cost_change_start, cost_change_start_fall" +
					", dreamteam_count, position, ep_next, ep_this, event_points, form, in_dreamteam, news, news_added" +
					", now_cost, points_per_game, selected_by, special, squad_number, status, team, team_code, total_points" +
					", transfers_in, transfers_in_event, transfers_out, transfers_out_event, value_form, value_season, minutes, goals_scored, assists, clean_sheets" +
					", goals_conceded, own_goals, penalties_saved, penalties_missed, yellow_cards, red_cards, saves, bonus, bps, influence, creativity, threat, ict_index, influence_rank" +
					", influence_rank_type, creativity_rank, creativity_rank_type, crners_fkicks_order, crners_fkicks_note, fkicks_order, fkicks_note, pens_order, pens_note)" +
					"VALUES({0},'{1}', '{2}', '{3}', {5}, {6}, {7}, {8}, {61}, {4}, {9}, {10}, '{11}', {12}, {13}, {14}, {15}, {16}, '{17}', '{18}', {19}, {20}, {21}, {22}, {23}, '{24}', '{25}'" +
					", {26}, {27}, {28}, {29}, {30}, {31}, {32}, {33}, {34}, {35}, {36}, {37}, {38}, {39}, {40}, {41}, {42}, {43}, {44}, {45}, {46}, {47}, {48}, {49}, {50}, {51}, {52}, {53}" +
					", {54}, {55}, '{56}', {57}, '{58}', {59}, '{60}')" +
					"ON CONFLICT (id) DO UPDATE " +
					"SET web_name = '{1}', first_name = '{2}', second_name = '{3}', chance_of_playing_next_round = {5},chance_of_playing_this_round = {6},code = {7}" +
					",cost_change_event = {8},cost_change_event_fall = {9},cost_change_start = {61},cost_change_start_fall = {4},dreamteam_count = {10},position = '{11}'" +
					",ep_next = {12},ep_this = {13},event_points = {14},form = {15},in_dreamteam = {16},news = '{17}',news_added = '{18}',now_cost = {19},points_per_game = {20}" +
					",selected_by = {21},special = {22},squad_number = {23},status = '{24}',team = '{25}',team_code = {26},total_points = {27},transfers_in = {28}" +
					",transfers_in_event = {29},transfers_out = {30},transfers_out_event = {31},value_form = {32},value_season = {33},minutes = {34},goals_scored = {35}" +
					",assists = {36},clean_sheets = {37},goals_conceded = {38},own_goals = {39},penalties_saved = {40},penalties_missed = {41},yellow_cards = {42},red_cards = {43}" +
					",saves = {44},bonus = {45},bps = {46},influence = {47},creativity = {48},threat = {49},ict_index = {50},influence_rank = {51},influence_rank_type = {52}" +
					",creativity_rank = {53},creativity_rank_type = {54},crners_fkicks_order = {55},crners_fkicks_note = '{56}',fkicks_order = {57},fkicks_note = '{58}',pens_order = {59},pens_note = '{60}'"
					, listfull.elements[i].id, listfull.elements[i].web_name.Replace("'", "''"), listfull.elements[i].first_name.Replace("'", "''"), listfull.elements[i].second_name.Replace("'", "''")
					, listfull.elements[i].cost_change_start_fall, listfull.elements[i].chance_of_playing_next_round == null ? 100 : listfull.elements[i].chance_of_playing_next_round//5
					, listfull.elements[i].chance_of_playing_this_round == null ? 100 : listfull.elements[i].chance_of_playing_this_round, listfull.elements[i].code, listfull.elements[i].cost_change_event
					, listfull.elements[i].cost_change_event_fall, listfull.elements[i].dreamteam_count, listfull.elements[i].position, listfull.elements[i].ep_next, listfull.elements[i].ep_this
					, listfull.elements[i].event_points, listfull.elements[i].form, Convert.ToBoolean(listfull.elements[i].in_dreamteam), "test", listfull.elements[i].news_added == null ? DateTime.Now : listfull.elements[i].news_added
					, listfull.elements[i].now_cost, listfull.elements[i].points_per_game, listfull.elements[i].selected_by_percent, Convert.ToBoolean(listfull.elements[i].special)
					, listfull.elements[i].squad_number == null ? 0 : listfull.elements[i].squad_number, listfull.elements[i].status, listfull.elements[i].team_name, listfull.elements[i].team_code
					, listfull.elements[i].total_points, listfull.elements[i].transfers_in, listfull.elements[i].transfers_in_event, listfull.elements[i].transfers_out , listfull.elements[i].transfers_out_event
					, listfull.elements[i].value_form, listfull.elements[i].value_season, listfull.elements[i].minutes, listfull.elements[i].goals_scored , listfull.elements[i].assists, listfull.elements[i].clean_sheets, listfull.elements[i].goals_conceded
					, listfull.elements[i].own_goals, listfull.elements[i].penalties_safed, listfull.elements[i].penalties_missed, listfull.elements[i].yellow_cards, listfull.elements[i].red_cards
					, listfull.elements[i].saves, listfull.elements[i].bonus , listfull.elements[i].bps, listfull.elements[i].influence, listfull.elements[i].creativity, listfull.elements[i].threat, listfull.elements[i].creativity_rank_type
					, listfull.elements[i].corners_and_indirect_freekicks_order == null ? 100 : listfull.elements[i].corners_and_indirect_freekicks_order, listfull.elements[i].corners_and_indirect_freekicks_text
					, listfull.elements[i].direct_freekicks_order == null ? 100 : listfull.elements[i].direct_freekicks_order, listfull.elements[i].direct_freekicks_text
					, listfull.elements[i].penalties_order == null ? 100 : listfull.elements[i].penalties_order, listfull.elements[i].penalties_text, listfull.elements[i].cost_change_start);

			return elementstring;
		}

		public static string getDFCString(int i, FPL.FullLeague fullLeague)
		{
			string DFCString = string.Format("BEGIN TRANSACTION;" +
			"INSERT INTO dashforcashrank " +
			"(id, event_total, player_name, rank, last_rank, rank_sort, total, entry, entry_name)" +
			"VALUES" +
			"({0},{1},'{2}',{3},{4},{5},{6},{7},'{8}')" +
			"ON CONFLICT (id) " +
			"DO UPDATE " +
			"SET event_total = {1} " +
			", player_name = '{2}' " +
			", rank = {3} " +
			", last_rank = {4} " +
			", rank_sort = {5} " +
			", total = {6} " +
			", entry = {7} " +
			", entry_name = '{8}'; " +
			"COMMIT TRANSACTION; ", fullLeague.standings.results[i].id, fullLeague.standings.results[i].event_total, fullLeague.standings.results[i].player_name.Replace("'", "''"),
									fullLeague.standings.results[i].rank, fullLeague.standings.results[i].last_rank, fullLeague.standings.results[i].rank_sort, fullLeague.standings.results[i].total,
									fullLeague.standings.results[i].entry, fullLeague.standings.results[i].entry_name.Replace("'", "''"));

			return DFCString;
		}

		public static string getGWStatsString(int i, FPL.History[] hist)
		{
			string playername = playerNames[hist[i].element];

			string GWString = string.Format("BEGIN;" +
			"INSERT INTO GWHistory" +
			"(element, player_name, fixture, opponent_team, total_points, was_home, minutes, goals_scored, assists, clean_sheets" +
			", goals_conceded, own_goals, penalties_saved, penalties_missed, yellow_cards, red_cards, saves, bonus" +
			", bps, influence, creativity, threat, ict_index, value, transfers_balance, selected, transfers_in, transfers_out)" +
			"VALUES" +
			"({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27})" +
			"ON CONFLICT DO NOTHING;" +
			"COMMIT; ", hist[i].element, playername.Replace("'", "''"), hist[i].fixture, hist[i].opponent_team, hist[i].total_points, Convert.ToByte(hist[i].was_home), hist[i].minutes
									, hist[i].goals_scored, hist[i].assists, hist[i].clean_sheets, hist[i].goals_conceded, hist[i].own_goals, hist[i].penalties_saved, hist[i].penalties_missed
									, hist[i].yellow_cards, hist[i].red_cards, hist[i].saves, hist[i].bonus, hist[i].bps, hist[i].influence, hist[i].creativity, hist[i].threat, hist[i].ict_index
									, hist[i].value, hist[i].transfers_balance, hist[i].selected, hist[i].transfers_in, hist[i].transfers_out);

			return GWString;
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

							case "LocalSQL":
								MSSQL = xmlReader.GetAttribute("value");
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
