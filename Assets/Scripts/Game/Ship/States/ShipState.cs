using Summoner.Systems.StateMachines;

namespace Summoner.Game.Ship.States
{
    public abstract class ShipState : State
    {
        protected readonly ShipControl ship;
        
        protected ShipState(ShipControl ship)
        {
            this.ship = ship;
        }
    }
}