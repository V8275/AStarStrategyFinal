using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamController
{
    private int TeamTurn = 0;
    private int TeamCount = 1;
    private Fractions[] fractions = null;
    private List<Team> teams;

    public TeamController(Fractions[] fr)
    {
        fractions = fr;
        TeamCount = fr.Length;
    }

    public void Init()
    {
        teams = new List<Team>();

        for (int i = 0; i < TeamCount; i++)
        {
            Team newTeam = new Team(fractions[i]);
            newTeam.units = new List<TeamUnit>();
            teams.Add(newTeam);
        }
    }

    public List<TeamUnit> GetCurrentTeamUnits()
    {
        return teams[TeamTurn].units;
    }

    public List<Team> GetOtherTeams()
    {
        List<Team> otherTeams = teams.Where(a => a != teams[TeamTurn]).ToList();
        return otherTeams;
    }

    public Team GetTeamByUnit(TeamUnit unit)
    {
        Team resultTeam = null;

        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i].units.Contains(unit))
            {
                resultTeam = teams[i];
                break;
            }
        }

        return resultTeam;
    }

    public List<TeamUnit> GetTeamUnits(Team team)
    {
        return team.units;
    }

    public void AddUnitInCurrentTeam(TeamUnit unit)
    {
        teams[TeamTurn].units.Add(unit);
    }

    public void RemoveUnitInTeam(TeamUnit unit)
    {
        teams[TeamTurn].units.Remove(unit);
    }

    public void RemoveUnitInTeam(Team team, TeamUnit unit)
    {
        team.units.Remove(unit);
    }

    public void NextTeamTurn()
    {
        foreach (var unit in teams[TeamTurn].units)
        {
            unit.moveable = false;
            unit.unit.WaitForMove(true);
            unit.unit.DisableUI();
        }

        TeamTurn++;
        if (TeamTurn >= TeamCount) TeamTurn = 0;

        foreach (var unit in teams[TeamTurn].units)
        {
            unit.moveable = true;
            unit.unit.WaitForMove(true);
            unit.unit.EnableUI();
        }
        if (teams[TeamTurn].units.Count > 0)
            teams[TeamTurn].units.First().unit.WaitForMove(false);

        Debug.Log($"Team {TeamTurn} \n" + teams[TeamTurn].GetUnitList());
    }

    public int GetCurrentTeamFraction()
    {
        int fracIdx = (int)teams[TeamTurn].fraction;
        return fracIdx;
    }
}

public class Team
{
    public List<TeamUnit> units;
    public Fractions fraction;

    public Team(Fractions frac)
    {
        fraction = frac;
    }

    public string GetUnitList()
    {
        string unitsData = "";
        foreach (var unit in units)
        {
            unitsData += unit.unit.GetUnitData() + "\n";
        }
        return unitsData;
    }
}

public class TeamUnit
{
    public IUnit unit;
    public bool moveable = true;

    public TeamUnit(IUnit unit, bool moveable)
    {
        this.unit = unit;
        this.moveable = moveable;
    }
}

public enum Fractions
{
    Clones,
    Droids,
    Empire,
    Rebel
}