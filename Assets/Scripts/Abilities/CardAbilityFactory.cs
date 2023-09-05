using System;

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
                case Ability.Agile: return new AgileAbility();
                case Ability.Berserker: return new BerserkerAbility();
                case Ability.Mardroeme: return new MardroemeAbility();
                case Ability.Medic: return new MedicAbility();
                case Ability.Muster: return new MusterAbility();
                case Ability.Spy: return new SpyAbility();
                case Ability.TightBond: return new TightBondAbility();
                case Ability.ClearWeather: return new ClearWeatherAbility();
                case Ability.Decoy: return new DecoyAbility();
                case Ability.Scorch: return new ScorchAbility();
                case Ability.Leader: return new LeaderAbility();

                //DONE
                case Ability.MoralBoost: return new MoralBoostAbility();
                case Ability.Frost: return new FrostAbility();
                case Ability.Fog: return new FogAbility();
                case Ability.Rain: return new RainAbility();
                case Ability.CommandersHorn: return new CommandersHornAbility();

                case Ability.None: return new NoAbility();

                default: throw new Exception($"Unkown ability: {ability}");
            }
        }
    }
}
