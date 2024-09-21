using System;
using Abilities;

namespace GwentEngine.Abilities
{
    public static class CardAbilityFactory
    {
        private class NoAbility : CardAbility { };

        public static CardAbility Create(Ability ability)
        {
            switch (ability)
            {
                //TODO
                
                case Ability.Berserker: return new BerserkerAbility();
                case Ability.Mardroeme: return new MardroemeAbility();
                case Ability.Muster: return new MusterAbility(); // faire les cas spéciaux

                case Ability.Leader: return new LeaderAbility();
                

                //DONE
                case Ability.MoralBoost: return new MoralBoostAbility();
                case Ability.Frost: return new FrostAbility();
                case Ability.Fog: return new FogAbility();
                case Ability.Rain: return new RainAbility();
                case Ability.CommandersHorn: return new CommandersHornAbility();
                case Ability.Decoy: return new DecoyAbility();
                case Ability.Scorch: return new ScorchAbility();
                case Ability.SpecialScorch: return new SpecialScorchAbility();
                case Ability.Agile: return new AgileAbility();
                case Ability.None: return new NoAbility();
                case Ability.Spy: return new SpyAbility();
                case Ability.ClearWeather: return new ClearWeatherAbility();
                case Ability.TightBond: return new TightBondAbility();
                case Ability.Medic: return new MedicAbility();
                case Ability.Emhyr1: return new Emhyr1Ability();
                case Ability.Emhyr2: return new Emhyr2Ability();
                case Ability.Emhyr3: return new Emhyr3Ability();
                case Ability.Emhyr4: return new Emhyr1Ability();
                case Ability.Emhyr5: return new Emhyr1Ability();
                

                default: throw new Exception($"Unkown ability: {ability}");
            }
        }
    }
}
