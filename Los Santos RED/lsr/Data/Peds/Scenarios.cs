﻿using LosSantosRED.lsr.Interface;
using System.Collections.Generic;

public class Scenarios : IScenarios
{
    public List<Scenario> ScenarioList { get; private set; }
    public Scenarios()
    {
        ScenarioList = new List<Scenario>() { new Scenario("WORLD_HUMAN_AA_COFFEE","Drink Coffee")
,new Scenario("WORLD_HUMAN_AA_SMOKE","Smoke")
,new Scenario("WORLD_HUMAN_BINOCULARS","Use Binoculars")
,new Scenario("WORLD_HUMAN_BUM_FREEWAY","Freeway Bum")
,new Scenario("WORLD_HUMAN_BUM_SLUMPED","Bum Slump")
,new Scenario("WORLD_HUMAN_BUM_STANDING","Bum Stand")
,new Scenario("WORLD_HUMAN_BUM_WASH","Bum Wash")
,new Scenario("WORLD_HUMAN_CAR_PARK_ATTENDANT","Part Car")
,new Scenario("WORLD_HUMAN_CHEERING","Cheer")
,new Scenario("WORLD_HUMAN_CLIPBOARD","Use Clipboard")
,new Scenario("WORLD_HUMAN_CONST_DRILL","Dill")
,new Scenario("WORLD_HUMAN_COP_IDLES","Idle")
,new Scenario("WORLD_HUMAN_DRINKING","Drink Alcohol")
,new Scenario("WORLD_HUMAN_DRUG_DEALER","Deal Drugs")
,new Scenario("WORLD_HUMAN_DRUG_DEALER_HARD","Deal Drugs")
,new Scenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING","Watch Mobile Phone")
,new Scenario("WORLD_HUMAN_GARDENER_LEAF_BLOWER","Use Leaf Blower")
,new Scenario("WORLD_HUMAN_GARDENER_PLANT","Plant")
,new Scenario("WORLD_HUMAN_GOLF_PLAYER","Swing Golf Club")
,new Scenario("WORLD_HUMAN_GUARD_PATROL","Patrol")
,new Scenario("WORLD_HUMAN_GUARD_STAND","Stand Guard")
,new Scenario("WORLD_HUMAN_GUARD_STAND_ARMY","Stand Guard")
,new Scenario("WORLD_HUMAN_HAMMERING","Use Hammer")
,new Scenario("WORLD_HUMAN_HANG_OUT_STREET","Hang Out")
,new Scenario("WORLD_HUMAN_HIKER_STANDING","Hike")
,new Scenario("WORLD_HUMAN_HUMAN_STATUE","Be Human Statue")
,new Scenario("WORLD_HUMAN_JANITOR","Clean Up")
,new Scenario("WORLD_HUMAN_JOG_STANDING","Jog Standing")
,new Scenario("WORLD_HUMAN_LEANING","Lean")
,new Scenario("WORLD_HUMAN_MAID_CLEAN","Clean Up")
,new Scenario("WORLD_HUMAN_MUSCLE_FLEX","Flex")
,new Scenario("WORLD_HUMAN_MUSCLE_FREE_WEIGHTS","Lift Weights")
,new Scenario("WORLD_HUMAN_MUSICIAN","Play Music")
,new Scenario("WORLD_HUMAN_PAPARAZZI","Take Picture")
,new Scenario("WORLD_HUMAN_PARTYING","Party")
,new Scenario("WORLD_HUMAN_PICNIC","Picnic")
,new Scenario("WORLD_HUMAN_PROSTITUTE_HIGH_CLASS","Sell Yourself")
,new Scenario("WORLD_HUMAN_PROSTITUTE_LOW_CLASS","Sell Yourself")
,new Scenario("WORLD_HUMAN_PUSH_UPS","Do Push Ups")
,new Scenario("WORLD_HUMAN_SEAT_LEDGE","Sit on Ledge")
,new Scenario("WORLD_HUMAN_SEAT_LEDGE_EATING","Eat on LEdge")
,new Scenario("WORLD_HUMAN_SEAT_STEPS","Sit on Steps")
,new Scenario("WORLD_HUMAN_SEAT_WALL","Sit on Wall")
,new Scenario("WORLD_HUMAN_SEAT_WALL_EATING","Eat on Wall")
,new Scenario("WORLD_HUMAN_SEAT_WALL_TABLET","Sit With Table")
,new Scenario("WORLD_HUMAN_SECURITY_SHINE_TORCH","Shine Flashlight")
,new Scenario("WORLD_HUMAN_SIT_UPS","Do Sit Ups")
,new Scenario("WORLD_HUMAN_SMOKING","Smoke")
,new Scenario("WORLD_HUMAN_SMOKING_POT","Smoke Pot")
,new Scenario("WORLD_HUMAN_STAND_FIRE","Stand Around Fire")
,new Scenario("WORLD_HUMAN_STAND_FISHING","Fish")
,new Scenario("WORLD_HUMAN_STAND_IMPATIENT","Stand Impatiently")
,new Scenario("WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT","Stand Impatiently")
,new Scenario("WORLD_HUMAN_STAND_MOBILE","Watch Phone")
,new Scenario("WORLD_HUMAN_STAND_MOBILE_UPRIGHT","Watch Phone")
,new Scenario("WORLD_HUMAN_STRIP_WATCH_STAND","Unknown")
,new Scenario("WORLD_HUMAN_STUPOR","Be Stupor")
,new Scenario("WORLD_HUMAN_SUNBATHE","Sunbathe")
,new Scenario("WORLD_HUMAN_SUNBATHE_BACK","Sunbathe")
,new Scenario("WORLD_HUMAN_SUPERHERO","Act Up")
,new Scenario("WORLD_HUMAN_SWIMMING","Swim")
,new Scenario("WORLD_HUMAN_TENNIS_PLAYER","Play Tennis")
,new Scenario("WORLD_HUMAN_TOURIST_MAP","Tourist Map")
,new Scenario("WORLD_HUMAN_TOURIST_MOBILE","Tourist Mobile")
,new Scenario("WORLD_HUMAN_VEHICLE_MECHANIC","Fix Car")
,new Scenario("WORLD_HUMAN_WELDING","Weld")
,new Scenario("WORLD_HUMAN_WINDOW_SHOP_BROWSE","Window Shop")
,new Scenario("WORLD_HUMAN_YOGA","Do Yoga")
,new Scenario("WORLD_BOAR_GRAZING","Graze")
,new Scenario("WORLD_CAT_SLEEPING_GROUND","Animal")
,new Scenario("WORLD_CAT_SLEEPING_LEDGE","Animal")
,new Scenario("WORLD_COW_GRAZING","Animal")
,new Scenario("WORLD_COYOTE_HOWL","Animal")
,new Scenario("WORLD_COYOTE_REST","Animal")
,new Scenario("WORLD_COYOTE_WANDER","Animal")
,new Scenario("WORLD_CHICKENHAWK_FEEDING","Animal")
,new Scenario("WORLD_CHICKENHAWK_STANDING","Animal")
,new Scenario("WORLD_CORMORANT_STANDING","Animal")
,new Scenario("WORLD_CROW_FEEDING","Animal")
,new Scenario("WORLD_CROW_STANDING","Animal")
,new Scenario("WORLD_DEER_GRAZING","Animal")
,new Scenario("WORLD_DOG_BARKING_ROTTWEILER","Animal")
,new Scenario("WORLD_DOG_BARKING_RETRIEVER","Animal")
,new Scenario("WORLD_DOG_BARKING_SHEPHERD","Animal")
,new Scenario("WORLD_DOG_SITTING_ROTTWEILER","Animal")
,new Scenario("WORLD_DOG_SITTING_RETRIEVER","Animal")
,new Scenario("WORLD_DOG_SITTING_SHEPHERD","Animal")
,new Scenario("WORLD_DOG_BARKING_SMALL","Animal")
,new Scenario("WORLD_DOG_SITTING_SMALL","Animal")
,new Scenario("WORLD_FISH_IDLE","Animal")
,new Scenario("WORLD_GULL_FEEDING","Animal")
,new Scenario("WORLD_GULL_STANDING","Animal")
,new Scenario("WORLD_HEN_PECKING","Animal")
,new Scenario("WORLD_HEN_STANDING","Animal")
,new Scenario("WORLD_MOUNTAIN_LION_REST","Animal")
,new Scenario("WORLD_MOUNTAIN_LION_WANDER","Animal")
,new Scenario("WORLD_PIG_GRAZING","Animal")
,new Scenario("WORLD_PIGEON_FEEDING","Animal")
,new Scenario("WORLD_PIGEON_STANDING","Animal")
,new Scenario("WORLD_RABBIT_EATING","Animal")
,new Scenario("WORLD_RATS_EATING","Animal")
,new Scenario("WORLD_SHARK_SWIM","Animal")
,new Scenario("PROP_BIRD_IN_TREE","Animal")
,new Scenario("PROP_BIRD_TELEGRAPH_POLE","Animal")
,new Scenario("PROP_HUMAN_ATM","Use ATM")
,new Scenario("PROP_HUMAN_BBQ","Use BBQ")
,new Scenario("PROP_HUMAN_BUM_BIN","Use Bin ")
,new Scenario("PROP_HUMAN_BUM_SHOPPING_CART","Use Shopping Cart")
,new Scenario("PROP_HUMAN_MUSCLE_CHIN_UPS","Do Chin Up")
,new Scenario("PROP_HUMAN_MUSCLE_CHIN_UPS_ARMY","Do Chin Up")
,new Scenario("PROP_HUMAN_MUSCLE_CHIN_UPS_PRISON","Do Chin Up")
,new Scenario("PROP_HUMAN_PARKING_METER","Pay for Parking")
,new Scenario("PROP_HUMAN_SEAT_ARMCHAIR","Sit in Chair")
,new Scenario("PROP_HUMAN_SEAT_BAR","Sit at Bar")
,new Scenario("PROP_HUMAN_SEAT_BENCH","Sit on Bench")
,new Scenario("PROP_HUMAN_SEAT_BENCH_DRINK","Drink at Bench")
,new Scenario("PROP_HUMAN_SEAT_BENCH_DRINK_BEER","Drink Beer at Bench")
,new Scenario("PROP_HUMAN_SEAT_BENCH_FOOD","Eat at Bench")
,new Scenario("PROP_HUMAN_SEAT_BUS_STOP_WAIT","Wait for Bus")
,new Scenario("PROP_HUMAN_SEAT_CHAIR","Sit in Chair")
,new Scenario("PROP_HUMAN_SEAT_CHAIR_DRINK","Drink in Chair")
,new Scenario("PROP_HUMAN_SEAT_CHAIR_DRINK_BEER","Drink Beer in Chair")
,new Scenario("PROP_HUMAN_SEAT_CHAIR_FOOD","Eat in Chair")
,new Scenario("PROP_HUMAN_SEAT_CHAIR_UPRIGHT","Sit in Chair")
,new Scenario("PROP_HUMAN_SEAT_CHAIR_MP_PLAYER","Sit in Chair")
,new Scenario("PROP_HUMAN_SEAT_COMPUTER","Sit at Computer")
,new Scenario("PROP_HUMAN_SEAT_DECKCHAIR","Sit at Deckchair")
,new Scenario("PROP_HUMAN_SEAT_DECKCHAIR_DRINK","Drink at Deckchair")
,new Scenario("PROP_HUMAN_SEAT_MUSCLE_BENCH_PRESS","Do Bench Press")
,new Scenario("PROP_HUMAN_SEAT_MUSCLE_BENCH_PRESS_PRISON","Do Bench Press")
,new Scenario("PROP_HUMAN_SEAT_SEWING","Sew")
,new Scenario("PROP_HUMAN_SEAT_STRIP_WATCH","Strip Watch")
,new Scenario("PROP_HUMAN_SEAT_SUNLOUNGER","Sit at Sunlounger")
,new Scenario("PROP_HUMAN_STAND_IMPATIENT","Stand Impatiently")
,new Scenario("CODE_HUMAN_COWER","Cower")
,new Scenario("CODE_HUMAN_CROSS_ROAD_WAIT","Wait to Cross")
,new Scenario("CODE_HUMAN_PARK_CAR","Part Car")
,new Scenario("PROP_HUMAN_MOVIE_BULB","Movie Bulb")
,new Scenario("PROP_HUMAN_MOVIE_STUDIO_LIGHT","Studio Light")
,new Scenario("CODE_HUMAN_MEDIC_KNEEL","Kneel to Check ")
,new Scenario("CODE_HUMAN_MEDIC_TEND_TO_DEAD","Tend to Dead")
,new Scenario("CODE_HUMAN_MEDIC_TIME_OF_DEATH","Log Time of Death")
,new Scenario("CODE_HUMAN_POLICE_CROWD_CONTROL","Do Crowd Control")
,new Scenario("CODE_HUMAN_POLICE_INVESTIGATE","Investigate")
,new Scenario("CODE_HUMAN_STAND_COWER","Cower")
,new Scenario("EAR_TO_TEXT","Ear to Text")
,new Scenario("EAR_TO_TEXT_FAT","Ear to Text") };
    }
}