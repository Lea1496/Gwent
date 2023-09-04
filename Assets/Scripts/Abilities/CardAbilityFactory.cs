using System;

namespace GwentEngine.Abilities
{
    public static class CardAbilityFactory
    {
        private class NoAbility : CardAbility { };

        public static CardAbility Create(GamePhase ability)
        {
            switch (ability)
            {
                //TODO
                case GamePhase.Agile: return new AgileAbility();
                case GamePhase.Berserker: return new BerserkerAbility();
                case GamePhase.Mardroeme: return new MardroemeAbility();
                case GamePhase.Medic: return new MedicAbility();
                case GamePhase.Muster: return new MusterAbility();
                case GamePhase.Spy: return new SpyAbility();
                case GamePhase.TightBond: return new TightBondAbility();
                case GamePhase.ClearWeather: return new ClearWeatherAbility();
                case GamePhase.Decoy: return new DecoyAbility();
                case GamePhase.Scorch: return new ScorchAbility();
                case GamePhase.Leader: return new LeaderAbility();

                //DONE
                case GamePhase.MoralBoost: return new MoralBoostAbility();
                case GamePhase.Frost: return new FrostAbility();
                case GamePhase.Fog: return new FogAbility();
                case GamePhase.Rain: return new RainAbility();
                case GamePhase.CommandersHorn: return new CommandersHornAbility();

                case GamePhase.None: return new NoAbility();

                default: throw new Exception($"Unkown ability: {ability}");
            }
        }
    }
}
