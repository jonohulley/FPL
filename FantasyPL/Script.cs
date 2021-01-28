using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyPL
{
	class Script
	{
		public static string getEventsString(FPL.Event events)
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
							, events.id, events.name, events.deadline_time, events.average_entry_score, Convert.ToBoolean(events.finished),
							Convert.ToBoolean(events.data_checked), events.highest_scoring_entry == null ? 0 : events.highest_scoring_entry, events.deadline_time_epoch,
							events.deadline_time_game_offset, events.highest_score == null ? 0 : events.highest_score, Convert.ToBoolean(events.is_previous),
							Convert.ToBoolean(events.is_current), Convert.ToBoolean(events.is_next), events.bboost, events.tripCap, events.wildcard,
							events.freehit, events.most_selected_name == null ? "" : events.most_selected_name,
							events.most_transferred_in_name == null ? "" : events.most_transferred_in_name, events.top_element_name, events.top_element_points,
							events.transfers_made, events.most_captained_name == null ? "" : events.most_captained_name, events.most_vice_captained_name == null ? "" : events.most_vice_captained_name);

			return eventstring;
		}

		public static string getElementsString(FPL.FootballPlayer player)
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
					, player.id, player.web_name.Replace("'", "''"), player.first_name.Replace("'", "''"), player.second_name.Replace("'", "''")
					, player.cost_change_start_fall, player.chance_of_playing_next_round == null ? 100 : player.chance_of_playing_next_round//5
					, player.chance_of_playing_this_round == null ? 100 : player.chance_of_playing_this_round, player.code, player.cost_change_event
					, player.cost_change_event_fall, player.dreamteam_count, player.position, player.ep_next, player.ep_this
					, player.event_points, player.form, Convert.ToBoolean(player.in_dreamteam), "test", player.news_added == null ? DateTime.Now : player.news_added
					, player.now_cost, player.points_per_game, player.selected_by_percent, Convert.ToBoolean(player.special)
					, player.squad_number == null ? 0 : player.squad_number, player.status, player.team_name, player.team_code
					, player.total_points, player.transfers_in, player.transfers_in_event, player.transfers_out, player.transfers_out_event
					, player.value_form, player.value_season, player.minutes, player.goals_scored, player.assists, player.clean_sheets, player.goals_conceded
					, player.own_goals, player.penalties_safed, player.penalties_missed, player.yellow_cards, player.red_cards
					, player.saves, player.bonus, player.bps, player.influence, player.creativity, player.threat,player.ict_index, player.influence_rank, player.influence_rank_type
					, player.creativity_rank, player.creativity_rank_type
					, player.corners_and_indirect_freekicks_order == null ? 100 : player.corners_and_indirect_freekicks_order, player.corners_and_indirect_freekicks_text
					, player.direct_freekicks_order == null ? 100 : player.direct_freekicks_order, player.direct_freekicks_text
					, player.penalties_order == null ? 100 : player.penalties_order, player.penalties_text, player.cost_change_start);

			return elementstring;
		}

		public static string getLeagueString(FPL.Result player)
		{
			string DFCString = string.Format("BEGIN TRANSACTION;" +
			"INSERT INTO league " +
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
			"COMMIT TRANSACTION; ", player.id, player.event_total, player.player_name.Replace("'", "''"),
									player.rank, player.last_rank, player.rank_sort, player.total,
									player.entry, player.entry_name.Replace("'", "''"));

			return DFCString;
		}

		public static string getGWStatsString(FPL.History playergw, string playername)
		{

			string GWString = string.Format("BEGIN;" +
			"INSERT INTO GWHistory" +
			"(element, player_name, fixture, opponent_team, total_points, was_home, minutes, goals_scored, assists, clean_sheets" +
			", goals_conceded, own_goals, penalties_saved, penalties_missed, yellow_cards, red_cards, saves, bonus" +
			", bps, influence, creativity, threat, ict_index, value, transfers_balance, selected, transfers_in, transfers_out)" +
			"VALUES" +
			"({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27})" +
			"ON CONFLICT DO NOTHING;" +
			"COMMIT; ", playergw.element, playername.Replace("'", "''"), playergw.fixture, playergw.opponent_team, playergw.total_points, Convert.ToByte(playergw.was_home), playergw.minutes
									, playergw.goals_scored, playergw.assists, playergw.clean_sheets, playergw.goals_conceded, playergw.own_goals, playergw.penalties_saved, playergw.penalties_missed
									, playergw.yellow_cards, playergw.red_cards, playergw.saves, playergw.bonus, playergw.bps, playergw.influence, playergw.creativity, playergw.threat, playergw.ict_index
									, playergw.value, playergw.transfers_balance, playergw.selected, playergw.transfers_in, playergw.transfers_out);

			return GWString;
		}
	}
}
