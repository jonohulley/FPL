using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyPL
{
	class FPL
	{
		public class Static
		{
			public List<Event> events { get; set; }
			public GameSettings game_settings { get; set; }
			public List<Phase> phases { get; set; }
			public List<Team> teams { get; set; }
			public long total_players { get; set; }
			public List<FootballPlayer> elements { get; set; }
			public List<ElementStat> element_stats { get; set; }
			public List<ElementType> element_types { get; set; }
		}

		public class ElementStat
		{
			public string label { get; set; }
			public string name { get; set; }
		}

		public class GameSettings
		{
			public int league_join_private_max { get; set; }
			public int league_join_public_max { get; set; }
			public int league_max_size_public_classic { get; set; }
			public int league_max_size_public_h2h { get; set; }
			public int league_max_size_private_h2h { get; set; }
			public int league_max_ko_rounds_private_h2h { get; set; }
			public string league_prefix_public { get; set; }
			public int league_points_h2h_win { get; set; }
			public int league_points_h2h_lose { get; set; }
			public int league_points_h2h_draw { get; set; }
			public bool league_ko_first_instead_of_random { get; set; }
			public int cup_start_event_id { get; set; }
			public int cup_stop_event_id { get; set; }
			public string cup_qualifying_method { get; set; }
			public string cup_type { get; set; }
			public int squad_squadplay { get; set; }
			public int squad_squadsize { get; set; }
			public int squad_team_limit { get; set; }
			public int squad_total_spend { get; set; }
			public int ui_currency_multiplier { get; set; }
			public bool ui_use_special_shirts { get; set; }
			public int stats_form_days { get; set; }
			public bool sys_vice_captain_enabled { get; set; }
			public int transfers_cap { get; set; }
			public double transfers_sell_on_fee { get; set; }
			public string[] league_h2h_tiebreak_stats { get; set; }
			public string timezone { get; set; }
		}

		public class Team
		{
			public int code { get; set; }
			public int draw { get; set; }
			public int? form { get; set; }
			public int id { get; set; }
			public int loss { get; set; }
			public string name { get; set; }
			public int played { get; set; }
			public int points { get; set; }
			public int position { get; set; }
			public string short_name { get; set; }
			public int strength { get; set; }
			public int? team_division { get; set; }
			public bool unavailable { get; set; }
			public int win { get; set; }
			public int strength_overall_home { get; set; }
			public int strength_overall_away { get; set; }
			public int strength_attack_home { get; set; }
			public int strength_attack_away { get; set; }
			public int strength_defence_home { get; set; }
			public int strength_defence_away { get; set; }
			public int pulse_id { get; set; }
		}

		public class Phase
		{
			public int id { get; set; }
			public string name { get; set; }
			public int start_event { get; set; }
			public int stop_event { get; set; }
		}

		public class ChipGroup
		{
			public string chip_name { get; set; }
			public long num_played { get; set; }
		}

		public class Nullable<ElementInfo>
		{
			public int id { get; set; }
			public int points { get; set; }
		}

		public class ElementInfo
		{
			public int id { get; set; }
			public int points { get; set; }
		}

		public class Event
		{
			public int id { get; set; }
			public string name { get; set; }
			public DateTime deadline_time { get; set; }
			public int average_entry_score { get; set; }
			public bool finished { get; set; }
			public bool data_checked { get; set; }
			public long? highest_scoring_entry { get; set; }
			public long deadline_time_epoch { get; set; }
			public int deadline_time_game_offset { get; set; }
			public int? highest_score { get; set; }
			public bool is_previous { get; set; }
			public bool is_current { get; set; }
			public bool is_next { get; set; }
			public List<ChipGroup> chip_plays { get; set; }
			public long bboost { get; set; }
			public long tripCap { get; set; }
			public long freehit { get; set; }
			public long wildcard { get; set; }
			public int? most_selected { get; set; }
			public string most_selected_name { get; set; }
			public int? most_transferred_in { get; set; }
			public string most_transferred_in_name { get; set; }
			public int? top_element { get; set; }
			public string top_element_name { get; set; }
			public int top_element_points { get; set; }
			public Nullable<ElementInfo> top_element_info { get; set; }
			public long transfers_made { get; set; }
			public int? most_captained { get; set; }
			public string most_captained_name { get; set; }
			public int? most_vice_captained { get; set; }
			public string most_vice_captained_name { get; set; }
		}

		public class ElementType
		{
			public int id { get; set; }
			public string plural_name { get; set; }
			public string plural_name_short { get; set; }
			public string singular_name { get; set; }
			public string singular_name_short { get; set; }
			public int squad_select { get; set; }
			public int squad_min_play { get; set; }
			public int squad_max_play { get; set; }
			public bool ui_shirt_specific { get; set; }
			public int[] sub_positions_locked { get; set; }
			public int element_count { get; set; }
		}

		public class FullLeague
		{
			public League league { get; set; }
			public NewEntries newEntries { get; set; }
			public Standings standings { get; set; }
		}

		public class League
		{
			public long id { get; set; }
			public string name { get; set; }
			public DateTime created { get; set; }
			public bool closed { get; set; }
			public int? max_entries { get; set; }
			public string league_type { get; set; }
			public string scoring { get; set; }
			public long admin_entry { get; set; }
			public int start_event { get; set; }
			public string code_privacy { get; set; }
			public long? rank { get; set; }
		}

		public class NewEntries
		{
			public bool has_next { get; set; }
			public int page { get; set; }
			public int[] results { get; set; }
		}

		public class Standings
		{
			public bool has_next { get; set; }
			public int page { get; set; }
			public List<Result> results { get; set; }
		}

		public class Result
		{
			public long id { get; set; }
			public int event_total { get; set; }
			public string player_name { get; set; }
			public int rank { get; set; }
			public int last_rank { get; set; }
			public int rank_sort { get; set; }
			public int total { get; set; }
			public long entry { get; set; }
			public string entry_name { get; set; }
		}

		public class FootballPlayer
		{
			public int? chance_of_playing_next_round { get; set; }
			public int? chance_of_playing_this_round { get; set; }
			public int code { get; set; }
			public int cost_change_event { get; set; }
			public int cost_change_event_fall { get; set; }
			public int cost_change_start { get; set; }
			public int cost_change_start_fall { get; set; }
			public int dreamteam_count { get; set; }
			public int element_type { get; set; } //position
			public string position { get; set; }
			public double ep_next { get; set; }
			public double ep_this { get; set; }
			public double event_points { get; set; }
			public string first_name { get; set; }
			public double form { get; set; }
			public int id { get; set; }
			public bool in_dreamteam { get; set; }
			public string news { get; set; }
			public DateTime? news_added { get; set; }
			public int now_cost { get; set; }
			public string photo { get; set; }
			public double points_per_game { get; set; }
			public string second_name { get; set; }
			public double selected_by_percent { get; set; }
			public bool special { get; set; }
			public int? squad_number { get; set; }
			public string status { get; set; }
			public int team { get; set; }
			public string team_name { get; set; }
			public int team_code { get; set; }
			public int total_points { get; set; }
			public int transfers_in { get; set; }
			public int transfers_in_event { get; set; }
			public int transfers_out { get; set; }
			public int transfers_out_event { get; set; }
			public double value_form { get; set; }
			public double value_season { get; set; }
			public string web_name { get; set; }
			public int minutes { get; set; }
			public int goals_scored { get; set; }
			public int assists { get; set; }
			public int clean_sheets { get; set; }
			public int goals_conceded { get; set; }
			public int own_goals { get; set; }
			public int penalties_safed { get; set; }
			public int penalties_missed { get; set; }
			public int yellow_cards { get; set; }
			public int red_cards { get; set; }
			public int saves { get; set; }
			public int bonus { get; set; }
			public int bps { get; set; }
			public double influence { get; set; }
			public double creativity { get; set; }
			public double threat { get; set; }
			public double ict_index { get; set; }
			public int influence_rank { get; set; }
			public int influence_rank_type { get; set; }
			public int creativity_rank { get; set; }
			public int creativity_rank_type { get; set; }
			public int threat_rank { get; set; }
			public int threat_rank_type { get; set; }
			public int ict_index_rank { get; set; }
			public int ict_index_rank_type { get; set; }
			public int? corners_and_indirect_freekicks_order { get; set; }
			public string corners_and_indirect_freekicks_text { get; set; }
			public int? direct_freekicks_order { get; set; }
			public string direct_freekicks_text { get; set; }
			public int? penalties_order { get; set; }
			public string penalties_text { get; set; }
		}

		public class WeeklyStats
		{
			public string web_name { get; set; }
			public string team { get; set; }
			public string opposition { get; set; }
			public int round { get; set; }
			public string understat_position { get; set; }
			public string fpl_position { get; set; }
			public int goals_scored { get; set; }
			public double? xG { get; set; }
			public int? npg { get; set; }
			public double? npxG { get; set; }
			public int? key_passes { get; set; }
			public int? shots { get; set; }
			public int assists { get; set; }
			public double? xA { get; set; }
			public double? xGBuildup { get; set; }
			public double? xGChain { get; set; }
			public int minutes { get; set; }
			public int? team_h_score { get; set; }
			public int? team_a_score { get; set; }
			public int? was_home { get; set; }
			public int? total_points { get; set; }
			public int value { get; set; }
			public long transfers_balance { get; set; }
			public long selected { get; set; }
			public long transfers_in { get; set; }
			public long transfers_out { get; set; }
			public int clean_sheets { get; set; }
			public int goals_conceded { get; set; }
			public int own_goals { get; set; }
			public int penalties_saved { get; set; }
			public int penalties_missed { get; set; }
			public int yellow_cards { get; set; }
			public int red_cards { get; set; }
			public int saves { get; set; }
			public int bonus { get; set; }
			public int bps { get; set; }
			public double influence { get; set; }
			public double creativity { get; set; }
			public double threat { get; set; }
			public double ict_index { get; set; }
			public int player_id { get; set; }
		}

		public enum LoadType
		{
			EVENTS = 0,
			PLAYERS = 1,
			STATS = 2,
			DFC = 3,
			GWSTATS = 4
		};

		public class GWData
		{
			public Fixture[] fixtures { get; set; }
			public History[] history { get; set; }
			public History_Past[] history_past { get; set; }
		}

		public class Fixture
		{
			public int id { get; set; }
			public int code { get; set; }
			public int team_h { get; set; }
			public object team_h_score { get; set; }
			public int team_a { get; set; }
			public object team_a_score { get; set; }
			public int _event { get; set; }
			public bool finished { get; set; }
			public int minutes { get; set; }
			public bool provisional_start_time { get; set; }
			public DateTime? kickoff_time { get; set; }
			public string event_name { get; set; }
			public bool is_home { get; set; }
			public int difficulty { get; set; }
		}

		public class History
		{
			public int element { get; set; }
			public int fixture { get; set; }
			public int opponent_team { get; set; }
			public int total_points { get; set; }
			public bool? was_home { get; set; }
			public DateTime? kickoff_time { get; set; }
			public int? team_h_score { get; set; }
			public int? team_a_score { get; set; }
			public int round { get; set; }
			public int minutes { get; set; }
			public int goals_scored { get; set; }
			public int assists { get; set; }
			public int clean_sheets { get; set; }
			public int goals_conceded { get; set; }
			public int own_goals { get; set; }
			public int penalties_saved { get; set; }
			public int penalties_missed { get; set; }
			public int yellow_cards { get; set; }
			public int red_cards { get; set; }
			public int saves { get; set; }
			public int bonus { get; set; }
			public int bps { get; set; }
			public double influence { get; set; }
			public double creativity { get; set; }
			public double threat { get; set; }
			public double ict_index { get; set; }
			public int value { get; set; }
			public int transfers_balance { get; set; }
			public int selected { get; set; }
			public int transfers_in { get; set; }
			public int transfers_out { get; set; }
		}

		public class History_Past
		{
			public string season_name { get; set; }
			public int element_code { get; set; }
			public int start_cost { get; set; }
			public int end_cost { get; set; }
			public int total_points { get; set; }
			public int minutes { get; set; }
			public int goals_scored { get; set; }
			public int assists { get; set; }
			public int clean_sheets { get; set; }
			public int goals_conceded { get; set; }
			public int own_goals { get; set; }
			public int penalties_saved { get; set; }
			public int penalties_missed { get; set; }
			public int yellow_cards { get; set; }
			public int red_cards { get; set; }
			public int saves { get; set; }
			public int bonus { get; set; }
			public int bps { get; set; }
			public string influence { get; set; }
			public string creativity { get; set; }
			public string threat { get; set; }
			public string ict_index { get; set; }
		}
	}
}
