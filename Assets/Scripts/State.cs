using System;
using System.Collections.Generic;


public abstract class State
{
   public abstract List<Action> GetTickBehaviours(params object[] parameters);
   public abstract List<Action> GetEnterBehaviours(params object[] parameters);
   public abstract List<Action> GetExitBehaviours(params object[] parameters);
}

public sealed class PatrolState: State
{
   public override List<Action> GetTickBehaviours(params object[] parameters)
   {
      return new List<Action>();
   }

   public override List<Action> GetEnterBehaviours(params object[] parameters)
   {
      return new List<Action>();
   }

   public override List<Action> GetExitBehaviours(params object[] parameters)
   {
      return new List<Action>();
   }
}

public sealed class ExplodeState : State
{
   public override List<Action> GetTickBehaviours(params object[] parameters)
   {
      return new List<Action>();
   }

   public override List<Action> GetEnterBehaviours(params object[] parameters)
   {
      return new List<Action>();
   }

   public override List<Action> GetExitBehaviours(params object[] parameters)
   {
      return new List<Action>();
   }
}

public sealed class ChaseState : State
{
   public override List<Action> GetTickBehaviours(params object[] parameters)
   {
      return new List<Action>();
   }

   public override List<Action> GetEnterBehaviours(params object[] parameters)
   {
      return new List<Action>();
   }

   public override List<Action> GetExitBehaviours(params object[] parameters)
   {
      return new List<Action>();
   }
}


