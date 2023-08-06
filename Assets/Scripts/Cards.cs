using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public static class Cards
{

    [SerializeField] private static GameObject card;
    public static List<(string, int, int, int, bool,int, int, int)> cardss =
        new List<(string, int, int, int, bool,int, int, int)>()
        {
            ("geralt_of_rivia_card", 8, 15, 7,true, 0, 0, 4),
            ("cirilla_fiona_elen_riannon_card", 8, 15, 7, true, 0, 1, 4),
            ("vesemir_card", 8, 6, 7, false, 0, 2, 4),
            ("yennefer_of_vengerberg_card", 3, 7, 8, true, 0, 3, 4),
            ("triss_merigold_card", 8, 7, 7, true, 0, 5, 4),
            ("dandelion", 4, 2, 7, false, 0, 6, 4),
            ("zoltan_chivay_card", 8, 5, 7, false, 0, 7, 4),
            ("emiel_regis_rohellec_terzieff_card", 8, 5, 7, false, 0, 8, 4),
            ("villentretenmerth_card", 13, 7, 7, false, 0,9, 4), //hmmm
            ("avallach_card", 6, 0, 10, true, 0, 10, 4),
            ("decoy_card", 12, -1, 7, false, 0, 11, 4),
            ("decoy_card", 12, -1, 7, false, 0, 12, 4),
            ("decoy_card", 12, -1, 7, false, 0, 13, 4),
            ("commanders_horn_card", 14, -1, 14, false, 0, 14, 4), 
            ("commanders_horn_card", 14, -1, 14, false, 0, 15, 4), 
            ("commanders_horn_card", 14, -1, 14, false, 0, 16, 4), 
            ("scorch_card", 13, -1, -1, false, 0, 17, 4),
            ("scorch_card", 13, -1, -1, false, 0, 18, 4),
            ("scorch_card", 13, -1, -1, false, 0, 19, 4),
            ("biting_frost_card", 9, -1, 20, false, 0, 20, 4),
            ("biting_frost_card", 9, -1, 20, false, 0, 21, 4),
            ("biting_frost_card", 9, -1, 20, false, 0, 22, 4),
            ("impenetrable_fog_card", 10, -1, 20, false, 0, 23, 4),
            ("impenetrable_fog_card", 10, -1, 20, false, 0, 24, 4),
            ("impenetrable_fog_card", 10, -1, 20, false, 0, 25, 4),
            ("torrential_rain_card", 11, -1, 20, false, 0, 26, 4),
            ("torrential_rain_card", 11, -1, 20, false, 0, 27, 4),
            ("torrential_rain_card", 11, -1, 20, false, 0, 28, 4),
            ("clear_weather_card", 15, -1,-1, false, 0, 29, 4),
            ("clear_weather_card", 15, -1,-1, false, 0, 30, 4),
            ("clear_weather_card", 15, -1,-1, false, 0, 31, 4),
            ("vernon_roche_card", 8, 10, 7, true, 0, 32, 0),
            ("john_natalis_card", 8, 10, 7, true, 0, 33, 0),
            ("esterad_thyssen_card", 8, 10, 7, true, 0, 34, 0),
            ("philippa_eilhart_card", 8, 10, 8,true, 0, 35, 0),
            ("thaler_card", 6, 1, 12, false, 0, 36, 0),
            ("ves_card", 8, 5, 7, false, 0, 37, 0),
            ("siegfried_of_denesle_card", 8,5,7,false, 0, 38, 0),
            ("yarpen_zigrin_card", 8, 2, 7, false, 0, 39, 0),
            ("sigismund_dijkstra_card", 6, 4, 10, false, 0, 40, 0),
            ("kiera_metz_card", 8, 5, 8, false, 0, 41, 0),
            ("sile_de_tansarville_card", 8, 5, 8, false, 0, 42, 0),
            ("sabrina_glevissing_card", 8, 4, 8, false, 0, 43, 0),
            ("sheldon_skaggs", 8, 4, 8, false, 0, 44, 0),
            ("dethmold_card", 8, 6, 8, false, 0, 45, 0),
            ("prince_stennis_card", 6, 5, 7, false, 0, 46, 0),
            ("trebuchet_card", 8, 6, 9, false, 0, 47, 0),
            ("trebuchet_card", 8, 6, 9, false, 0, 48, 0),
            ("poor_fcking_infantry_card", 7, 1, 7, false, 0, 49, 0),
            ("poor_fcking_infantry_card", 7, 1, 7, false, 0, 50, 0),
            ("poor_fcking_infantry_card", 7, 1, 7, false, 0, 51, 0),
            ("crinfrid_reavers_dragon_hunter_card", 7, 5, 8, false, 0, 52, 0),
            ("crinfrid_reavers_dragon_hunter_card", 7, 5, 8, false, 0, 53, 0),
            ("crinfrid_reavers_dragon_hunter_card", 7, 5, 8, false, 0, 54, 0),
            ("redanian_foot_soldier_card", 8, 1, 7, false, 0, 55, 0),
            ("redanian_foot_soldier_card", 8, 1, 7, false, 0, 56, 0),
            ("catapult_card", 7, 8, 9, false, 0, 57, 0),
            ("catapult_card", 7, 8, 9, false, 0, 58, 0),
            ("ballista_card", 8, 6, 9, false, 0, 59, 0),
            ("ballista_card", 8, 6, 9, false, 0, 60, 0),
            ("kaedweni_siege_expert_card", 4, 1, 9, false, 0, 61, 0),
            ("kaedweni_siege_expert_card", 4, 1, 9, false, 0, 62, 0),
            ("kaedweni_siege_expert_card", 4, 1, 9, false, 0, 63, 0),
            ("blue_stripes_commando_card", 7, 4, 7, false, 0, 64, 0),
            ("blue_stripes_commando_card", 7, 4, 7, false, 0, 65, 0),
            ("blue_stripes_commando_card", 7, 4, 7, false, 0, 66, 0),
            ("siege_tower_card", 8, 6, 9, false, 0, 67, 0),
            ("dun_banner_medic_card", 3, 5, 9, false, 0, 68, 0),
            ("letho_of_gulet_card", 8, 10 , 7, true, 0, 69, 1),
            ("menno_coehoorn_card", 3, 10, 7, true, 0, 70, 1),
            ("morvran_voorhis_card", 8, 10, 9, true, 0, 71, 1),
            ("tibor_eggebracht_card", 8, 10, 8, true, 0, 72, 1),
            ("albrich_card", 8, 2, 8, false, 0, 73, 1),
            ("assire_var_anahid_card", 8, 6, 8, false, 0, 74, 1),
            ("cynthia_card", 8, 4, 8, false, 0, 75, 1),
            ("fringilla_vigo_card", 8, 6, 8, false, 0, 76, 1),
            ("morteisen_card", 8,3, 7, false, 0, 77, 1),
            ("rainfarn_card", 8, 4, 7, false, 0, 78, 1),
            ("renuald_aep_matsen_card", 8,5, 8, false, 0, 79, 1),
            ("rotten_mangonel_card", 8, 5, 9, false, 0, 80, 1),
            ("shilard_fitz_oesterlen_card", 6, 7, 10, false, 0, 81, 1),
            ("stefan_skellen_card", 6, 9, 10, false, 0, 82, 1),
            ("sweers_card", 8, 2, 8, false, 0, 83, 1),
            ("vanhemar_card", 8, 4, 8, false, 0, 84, 1),
            ("vattier_de_rideaux_card",6, 4, 10, false, 0, 85, 1),
            ("vreemde_card", 8, 2, 7, false, 0, 86, 1),
            ("cahir_mawr_dyffryn_aep_ceallach_card", 8, 6, 7, false, 0, 87, 1),
            ("puttkammer_card", 8, 3, 8, false, 0, 88, 1),
            ("etolian_auxiliary_archers_card", 3, 1, 8, false, 0, 89, 1),
            ("etolian_auxiliary_archers_card", 3, 1, 8, false, 0, 90, 1),
            ("black_infantry_archer_card", 8, 10, 8, false, 0, 91, 1),
            ("black_infantry_archer_card", 8, 10, 8, false, 0, 92, 1), 
            ("siege_technican_card", 3, 0, 9, false, 0, 93, 1),
            ("heavy_zerrikanian_fire_scorpion_card", 8, 10, 9, false, 0, 94, 1),
            ("impera_brigade_card", 7, 3, 7, false, 0, 95, 1),
            ("impera_brigade_card", 7, 3, 7, false, 0, 96, 1),
            ("impera_brigade_card", 7, 3, 7, false, 0, 97, 1),
            ("impera_brigade_card", 7, 3, 7, false, 0, 98, 1),
            ("nausicaa_cavalry_brigade_card", 7, 2, 7, false, 0, 99, 1),
            ("nausicaa_cavalry_brigade_card", 7, 2, 7, false, 0, 100, 1),
            ("nausicaa_cavalry_brigade_card", 7, 2, 7, false, 0, 101, 1),
            ("siege_engineer_card", 8, 6, 9, false, 0, 102, 1),
            ("young_emissary_card", 7, 5, 7, false, 0, 103, 1),
            ("young_emissary_card", 7, 5, 7, false, 0, 104, 1),
            
            
            
            
            
        };

    private static (string, int, int, int, bool, int, int) c1 = ("geralt_of_rivia_card", 8, 15, 7,true, 0, 4);

    public static List<GameObject> cards = new List<GameObject>();

    private static void Test()
    {
        for (int i = 0; i < cardss.Count; i++)
        {
            card.GetComponent<CardBehavior>().Constructeur(cardss[i].Item1, cardss[i].Item2, cardss[i].Item3, cardss[i].Item4, cardss[i].Item5, cardss[i].Item6, cardss[i].Item7,  cardss[i].Item8);
            cards.Add(card);
        }
        
        
        
    }



}
