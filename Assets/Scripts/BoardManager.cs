using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public static class BoardManager
{
    public static List<GameObject> sword = new List<GameObject>();
    public static List<GameObject> arc = new List<GameObject>();
    public static List<GameObject> catapult = new List<GameObject>();
    public static List<GameObject> swordEnemy = new List<GameObject>();
    public static List<GameObject> arcEnemy = new List<GameObject>();
    public static List<GameObject> catapultEnemy = new List<GameObject>();
    
    public static List<int> cardsAlreadyPicked = new List<int>();
    
    public static List<List<GameObject>> wholeBoard = new List<List<GameObject>>() {sword, arc, catapult, swordEnemy, arcEnemy, catapultEnemy};

    
    public static bool playerHasPassed = false;
    public static bool enemyHasPassed = false;
  

    
    public static List<CardObj> defausse = new List<CardObj>()/* {c11,c22,c33,c44,c55}*/;
    public static List<GameObject> objDefausse = new List<GameObject>();
    public static List<GameObject> board = new List<GameObject>();

    public static List<(string, int, int, int, bool,int, int, int)> deck = new List<(string, int, int, int, bool,int, int, int)>()
    {
            ("geralt_of_rivia_card", 8, 15, 7,true, 0, 0, 4),
            ("cirilla_fiona_elen_riannon_card", 8, 15, 7, true, 0, 1, 4),
            ("vesemir_card", 8, 6, 7, false, 0, 2, 4),
            ("yennefer_of_vengerberg_card", 3, 7, 8, true, 0, 3, 4),
            ("triss_merigold_card", 8, 7, 7, true, 0, 5, 4),
            ("dandelion_card", 4, 2, 7, false, 0, 6, 4),
            ("zoltan_chivay_card", 8, 5, 7, false, 0, 7, 4),
            ("emiel_regis_rohellec_terzieff_card", 8, 5, 7, false, 0, 8, 4),
            ("villentretenmerth_card", 13, 7, 7, false, 0,9, 4), //hmmm
            ("avallach_card", 6, 0, 10, true, 0, 10, 4),
            ("decoy_card", 12, -1, -1, false, 0, 11, 4),
            ("decoy_card", 12, -1, -1, false, 0, 12, 4),
            ("decoy_card", 12, -1, -1, false, 0, 13, 4),
            ("commanders_horn_card", 12, -1, 14, false, 0, 14, 4), 
            ("commanders_horn_card", 12, -1, 14, false, 0, 15, 4), 
            ("commanders_horn_card", 12, -1, 14, false, 0, 16, 4), 
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
            ("clear_weather_card", 13, -1, 20, false, 0, 29, 4),
            ("clear_weather_card", 13, -1, 20, false, 0, 30, 4),
            ("clear_weather_card", 13, -1, 20, false, 0, 31, 4),
            ("vernon_roche_card", 8, 10, 7, true, 0, 32, 0),
            ("john_natalis_card", 8, 10, 7, true, 0, 33, 0),
            ("esterad_thyssen_card", 8, 10, 7, true, 0, 34, 0),
            ("philippa_eilhart_card", 8, 10, 8,true, 0, 35, 0),
            ("thaler_card", 6, 1, 12, false, 0, 36, 0),
            ("ves_card", 8, 5, 7, false, 0, 37, 0),
            ("siegfried_of_denesle_card", 8,5,7,false, 0, 38, 0),
            ("yarpen_zigrin_card", 8, 2, 7, false, 0, 39, 0),
            ("sigismund_dijkstra_card", 6, 4, 10, false, 0, 40, 0),
            ("keira_metz_card", 8, 5, 8, false, 0, 41, 0),
            ("sile_de_tansarville_card", 8, 5, 8, false, 0, 42, 0),
            ("sabrina_glevissig_card", 8, 4, 8, false, 0, 43, 0),
            ("sheldon_skaggs_card", 8, 4, 8, false, 0, 44, 0),
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
            ("shilard_fitz_oesternlen_card", 6, 7, 10, false, 0, 81, 1),
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
            ("siege_technician_card", 3, 0, 9, false, 0, 93, 1),
            ("heavy_zerrikanian_fire_scorpion_card", 8, 10, 9, false, 0, 94, 1),
            ("impera_brigade_guard_card", 7, 3, 7, false, 0, 95, 1),
            ("impera_brigade_guard_card", 7, 3, 7, false, 0, 96, 1),
            ("impera_brigade_guard_card", 7, 3, 7, false, 0, 97, 1),
            ("impera_brigade_guard_card", 7, 3, 7, false, 0, 98, 1),
            ("nausicaa_cavalry_brigade_card", 7, 2, 7, false, 0, 99, 1),
            ("nausicaa_cavalry_brigade_card", 7, 2, 7, false, 0, 100, 1),
            ("nausicaa_cavalry_brigade_card", 7, 2, 7, false, 0, 101, 1),
            ("siege_engineer_card", 8, 6, 9, false, 0, 102, 1),
            ("young_emissary_card", 7, 5, 7, false, 0, 103, 1),
            ("young_emissary_card", 7, 5, 7, false, 0, 104, 1),
            ("eithne_card",8, 10, 8, true, 0, 105, 2),
            ("saesenthessis_card", 8, 10, 8, true, 0, 106, 2),
            ("isengrim_faolitarna_card", 4, 10,7, false, 0, 107, 2),
            ("iorveth_card", 8, 10, 8, true, 0, 108, 2),
            ("dennis_cranmer_card", 8, 6, 7, false, 0, 109, 2),
            ("milva_card", 4, 10, 8, false, 0, 110, 2), // <3
            ("ida_emean_card", 8, 6, 8, false, 0, 111, 2),
            ("filavandrel_card", 0, 6, 7, false, 0, 112, 2), // hmmm son ranK ??
            ("yaevinn_card", 0, 6, 7, false, 0, 113, 2), //hmm son rank??
            ("toruviel_card", 8, 2, 8, false, 0, 114, 2),
            ("riordain_card", 8, 1 , 8, false, 0, 115, 2),
            ("ciaran_aep_easnillien_card", 0, 3, 7, false,0, 116, 2),//hmm son rank??
            ("barclay_els_card", 0, 6, 7, false, 0, 117, 2), //hmm son rank??
            ("havekar_healer_card", 3, 0, 8, false, 0, 118, 2),
            ("havekar_healer_card", 3, 0, 8, false, 0, 119, 2),
            ("havekar_healer_card", 3, 0, 8, false, 0, 120, 2),
            ("vrihedd_brigade_recruit_card", 0, 4, 8, false, 0, 121, 2), 
            ("vrihedd_brigade_veteran_card", 0, 5, 7, false, 0, 122, 2), //hmm son rank ??
            ("dol_blathanna_archer_card", 0, 6, 8, false, 0, 123, 2), 
            ("dol_blathanna_scout_card", 0, 6, 7, false, 0, 124, 2), //hmm son rank ??
            ("dol_blathanna_scout_card", 0, 6, 7, false, 0, 125, 2), //hmm son rank ??
            ("dwarven_skirmisher_card", 5, 3, 7, false, 0, 126, 2),
            ("dwarven_skirmisher_card", 5, 3, 7, false, 0, 127, 2),
            ("dwarven_skirmisher_card", 5, 3, 7, false, 0, 128, 2),
            ("mahakaman_defender_card", 8, 5, 7, false, 0, 129, 2),
            ("mahakaman_defender_card", 8, 5, 7, false, 0, 130, 2),
            ("mahakaman_defender_card", 8, 5, 7, false, 0, 131, 2),
            ("mahakaman_defender_card", 8, 5, 7, false, 0, 132, 2),
            ("mahakaman_defender_card", 8, 5, 7, false, 0, 133, 2),
            ("elven_skirmisher_card", 5, 2, 8, false, 0, 134, 2),
            ("elven_skirmisher_card", 5, 2, 8, false, 0, 135, 2),
            ("elven_skirmisher_card", 5, 2, 8, false, 0, 136, 2),
            //("vrihedd_cadet_card", 0, 4, 7, false, 0, 137, 2), //hmm son rank ??
            ("havekar_smuggler_card", 5, 5, 7, false, 0, 138, 2),
            ("havekar_smuggler_card", 5, 5, 7, false, 0, 139, 2),
            ("havekar_smuggler_card", 5, 5, 7, false, 0, 140, 2),
            ("draug_card", 8, 10, 7, true, 0, 141, 3),
            ("kayran_card", 4, 8, 7, true, 0, 142, 3), //hmm son rank??
            ("imlerith_card", 8, 10, 7, true, 0, 143, 3),
            ("leshen_card", 8, 10, 8, true, 0, 144, 3),
            ("forktail_card", 8,5, 7, false, 0, 145, 3),
            ("earth_elemental_card",  8, 6, 9, false, 0, 146, 3),
            ("fiend_card", 8, 6, 7, false, 0, 147, 3),
            ("plague_maiden_card", 8, 5, 7, false, 0, 148, 3),
            ("griffin_card", 8, 5, 7, false, 0, 149, 3),
            ("werewolf_card", 8, 5, 7, false, 0, 150, 3),
            ("botchling_card", 8, 4, 7, false, 0, 151, 3),
            ("frightener_card", 8, 5, 7, false, 0, 152, 3),
            ("ice_giant_card", 8, 5, 9, false, 0, 153, 3),
            ("endrega_card", 8, 2, 8, false, 0, 154, 3),
            ("harpy_card", 0, 2, 7, false, 0, 155, 3), //hmm le rank??
            ("cockatrice_card", 8, 2, 8, false, 0, 156, 3),
            ("gargoyle_card", 8, 2, 8, false, 0, 157, 3),
            ("celaeno_harpy_card", 0, 2, 7, false, 0, 158, 3), //hmm le rank??
            ("grave_hag_card", 8, 5, 8, false, 0, 159, 3),
            ("fire_elemental_card", 8, 6, 9, false, 0, 160, 3),
            ("foglet_card", 8, 2, 7, false, 0, 161, 3),
            ("wyvern_card", 8, 2, 8, false, 0, 162, 3),
            ("arachas_behemoth_card", 5, 6, 9, false, 0, 163, 3),
            ("arachas_card", 5, 4, 7, false, 0, 164, 3),
            ("arachas_card", 5, 4, 7, false, 0, 165, 3),
            ("arachas_card", 5, 4, 7, false, 0, 166, 3),
            ("nekker_card", 5, 2, 7, false, 0, 167, 3),
            ("nekker_card", 5, 2, 7, false, 0, 168, 3),
            ("nekker_card", 5, 2, 7, false, 0, 169, 3),
            ("vampire_ekimmara_card", 5, 4, 7, false, 0, 170, 3),
            ("vampire_fleder_card", 5, 4, 7, false, 0, 171, 3),
            ("vampire_garkain_card", 5, 4, 7, false, 0, 172, 3),
            ("vampire_bruxa_card", 5, 4, 7, false, 0, 173, 3),
            ("vampire_katakan_card", 5, 4, 7, false, 0, 174, 3),
            ("ghoul_card", 5, 1, 7, false, 0, 175, 3),
            ("ghoul_card", 5, 1, 7, false, 0, 176, 3),
            ("ghoul_card", 5, 1, 7, false, 0, 177, 3),
            ("crone_brewess_card", 5, 6, 7, false, 0, 178, 3),
            ("crone_weavess_card", 5, 6, 7, false, 0, 179, 3),
            ("crone_whispess_card", 5, 6, 7, false, 0, 180, 3),
            
            
        };/*{c1,c2,c3,c4,c5,c6,c7,c8,c9,c0}*/
    
    public static List<GameObject> deckEnemy = new List<GameObject>();

    public static List<GameObject> weatherCards = new List<GameObject>();


    public static List<CardBehavior> cardsInHand = new List<CardBehavior>();
    public static List<CardBehavior> cardsInHandEnemy = new List<CardBehavior>();

    public static int nbCardsInHand = 10;
    public static int nbCardsInHandEnemy = 10;
    private static int offset = 0;
    

    
    


}
