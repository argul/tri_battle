using System;
using System.Collections;
using System.Collections.Generic;

public class FSMState
{
	public FSMState(int code, object param, FSMMachine machine)
	{
		this.code = code;
		this.param = param;
		this.machine = machine;
	}
	public int Code { get { return code; } }
	public Object Param { get { return param; } }
	public FSMMachine Machine { get { return machine; } }
	int code;
	object param;
	FSMMachine machine;
	
	public virtual void Enter() {}
	public virtual void Exit() {}
	public virtual void Update(float dt) {}
	public virtual int CheckTransitions(float dt) { return Code; }
}

public class FSMMachine
{
	public void AddState(FSMState state)
	{
		stateDict.Add(state.Code, state);
	}
	
	public void SetDefaultState(int code)
	{
		defaultState = GetState(code);
	}
	
	public virtual bool TransitionState(int code)
	{
		return true;
	}
	
	public FSMState CurrentState
	{
		get { return curState; }
	}
	
	private FSMState GetState(int code)
	{
		return stateDict[code];
	}
	
	public void Reset()
	{
		if (curState != defaultState)
		{
			SwitchTo(defaultState.Code);
		}
	}
	
	public void UpdateMachine(float dt)
	{
		if (stateDict.Count <= 0) return;
		
		if (null == curState) curState = defaultState;
		
		if (null == curState) return;
		
		targetStateCode = curState.CheckTransitions(dt);
		if (targetStateCode != curState.Code 
		    && TransitionState(targetStateCode))
		{
			SwitchTo(targetStateCode);
		}
		
		curState.Update(dt);
	}
	
	Dictionary<int, FSMState> stateDict = new Dictionary<int, FSMState>();
	FSMState 	curState;
	FSMState 	defaultState;
	int 		targetStateCode;
	
	public void SwitchTo(int code)
	{
		if (null != curState && curState.Code == code) return;
		if (null != curState)
		{
			curState.Exit();
		}
		
		curState = GetState(code);
		
		if (null != curState)
		{
			curState.Enter();
		}
	}
}