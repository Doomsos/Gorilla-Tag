using System;
using System.Collections.Generic;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000E96 RID: 3734
	public class StateMachine
	{
		// Token: 0x06005D60 RID: 23904 RVA: 0x001DFD04 File Offset: 0x001DDF04
		public void Tick()
		{
			StateMachine.Transition transition = this.GetTransition();
			if (transition != null)
			{
				this.SetState(transition.To);
			}
			IState currentState = this._currentState;
			if (currentState == null)
			{
				return;
			}
			currentState.Tick();
		}

		// Token: 0x06005D61 RID: 23905 RVA: 0x001DFD38 File Offset: 0x001DDF38
		public void SetState(IState state)
		{
			if (state == this._currentState)
			{
				return;
			}
			IState currentState = this._currentState;
			if (currentState != null)
			{
				currentState.OnExit();
			}
			this._currentState = state;
			this._transitions.TryGetValue(this._currentState.GetType(), ref this._currentTransitions);
			if (this._currentTransitions == null)
			{
				this._currentTransitions = StateMachine.EmptyTransitions;
			}
			this._currentState.OnEnter();
		}

		// Token: 0x06005D62 RID: 23906 RVA: 0x001DFDA2 File Offset: 0x001DDFA2
		public IState GetState()
		{
			return this._currentState;
		}

		// Token: 0x06005D63 RID: 23907 RVA: 0x001DFDAC File Offset: 0x001DDFAC
		public void AddTransition(IState from, IState to, Func<bool> predicate)
		{
			List<StateMachine.Transition> list;
			if (!this._transitions.TryGetValue(from.GetType(), ref list))
			{
				list = new List<StateMachine.Transition>();
				this._transitions[from.GetType()] = list;
			}
			list.Add(new StateMachine.Transition(to, predicate));
		}

		// Token: 0x06005D64 RID: 23908 RVA: 0x001DFDF3 File Offset: 0x001DDFF3
		public void AddAnyTransition(IState state, Func<bool> predicate)
		{
			this._anyTransitions.Add(new StateMachine.Transition(state, predicate));
		}

		// Token: 0x06005D65 RID: 23909 RVA: 0x001DFE08 File Offset: 0x001DE008
		private StateMachine.Transition GetTransition()
		{
			foreach (StateMachine.Transition transition in this._anyTransitions)
			{
				if (transition.Condition.Invoke())
				{
					return transition;
				}
			}
			foreach (StateMachine.Transition transition2 in this._currentTransitions)
			{
				if (transition2.Condition.Invoke())
				{
					return transition2;
				}
			}
			return null;
		}

		// Token: 0x04006B45 RID: 27461
		private IState _currentState;

		// Token: 0x04006B46 RID: 27462
		private Dictionary<Type, List<StateMachine.Transition>> _transitions = new Dictionary<Type, List<StateMachine.Transition>>();

		// Token: 0x04006B47 RID: 27463
		private List<StateMachine.Transition> _currentTransitions = new List<StateMachine.Transition>();

		// Token: 0x04006B48 RID: 27464
		private List<StateMachine.Transition> _anyTransitions = new List<StateMachine.Transition>();

		// Token: 0x04006B49 RID: 27465
		private static List<StateMachine.Transition> EmptyTransitions = new List<StateMachine.Transition>(0);

		// Token: 0x02000E97 RID: 3735
		private class Transition
		{
			// Token: 0x170008A5 RID: 2213
			// (get) Token: 0x06005D68 RID: 23912 RVA: 0x001DFEEA File Offset: 0x001DE0EA
			public Func<bool> Condition { get; }

			// Token: 0x170008A6 RID: 2214
			// (get) Token: 0x06005D69 RID: 23913 RVA: 0x001DFEF2 File Offset: 0x001DE0F2
			public IState To { get; }

			// Token: 0x06005D6A RID: 23914 RVA: 0x001DFEFA File Offset: 0x001DE0FA
			public Transition(IState to, Func<bool> condition)
			{
				this.To = to;
				this.Condition = condition;
			}
		}
	}
}
