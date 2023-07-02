using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using chen0040.ExpertSystem;
public class ExpertSystemManager : MonoBehaviour
{
    public static ExpertSystemManager Instance { get; private set; }
    [SerializeField] private int maxSideWindSpeed = 55;
    public enum Weathertypes
    {
        Sun,
        Rain,
        Storm
    }
    [SerializeField] private int windSpeed;
    [SerializeField] private Weathertypes weather;
    [SerializeField] private bool extremeEvent = false;
    RuleInferenceEngine rie = new RuleInferenceEngine();
    Clause conclusion;
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("Buildingsystem has already an Instance");
        }
        Instance = this;
    }
    void Start()
    {
        Rule rule = new Rule("Extremereignis");
        rule.AddAntecedent(new IsClause("extremereignis", "True"));
        rule.setConsequent(new IsClause("anweisung", "warten"));
        rie.AddRule(rule);

        rule = new Rule("Wetter");
        rule.AddAntecedent(new IsClause("wettertyp", Weathertypes.Storm.ToString()));
        rule.setConsequent(new IsClause("anweisung", "warten"));
        rie.AddRule(rule);

        rule = new Rule("Seitenwind");
        rule.AddAntecedent(new GEClause("windgeschwindigkeit", maxSideWindSpeed.ToString()));
        rule.setConsequent(new IsClause("anweisung", "warten"));
        rie.AddRule(rule);

        StartCoroutine(UpdateWeather());
        StartCoroutine(UpdateWind());
        StartCoroutine(UpdateExtremeEvent());
    }

    private IEnumerator UpdateWeather()
    {
        //alle 16 Stunden neues Wetter
        //Chancen sonne 78% regen 20% sturm 2% 
        var delay = new WaitForSeconds(57600);
        while (true)
        {
            int rand = Random.Range(1, 101);
            if (rand <= 78)
            {
                weather = Weathertypes.Sun;
            }
            else if (rand <= 98)
            {
                weather = Weathertypes.Rain;
            }
            else
            {
                weather = Weathertypes.Storm;
            }
            yield return delay;
        }

    }

    private IEnumerator UpdateWind()
    {
        //alle 8 Stunden neuer Wind
        //seitenwind zwischen 0 und 60 km/h
        var delay = new WaitForSeconds(28800);
        while (true)
        {
            maxSideWindSpeed = Random.Range(0, 60);
            yield return delay;
        }
    }

    private IEnumerator UpdateExtremeEvent()
    {
        //extrem event hat eine 0.5% Chance an jedem Tag
        var delay = new WaitForSeconds(86400);
        while (true)
        {
            int rand = Random.Range(1, 201);
            extremeEvent = rand == 200;
            yield return delay;
        }
    }

    public void SetWeather(string str)
    {
        if (str.Equals("Sun"))
        {
            this.weather = Weathertypes.Sun;
        }
        else if (str.Equals("Rain"))
        {
            this.weather = Weathertypes.Rain;
        }
        else
        {
            this.weather = Weathertypes.Storm;
        }
    }

    public void SetWindSpeed(int speed)
    {
        this.windSpeed = Mathf.Min(0, speed);
    }

    public void SetExtremeEvent(bool extremeEvent)
    {
        this.extremeEvent = extremeEvent;
    }

    public string GetWeatherAsString()
    {
        return weather.ToString();
    }

    public int GetWindSpeed()
    {
        return windSpeed;
    }

    public bool GetExtremeEvent()
    {
        return extremeEvent;
    }

    public string GetConclusion()
    {
        return conclusion == null ? "" : conclusion.Value;
    }

    public bool AllowedToStart()
    {
        rie.AddFact(new IsClause("wettertyp", weather.ToString()));
        rie.AddFact(new IsClause("windgeschwindigkeit", windSpeed.ToString()));
        rie.AddFact(new IsClause("extremereignis", extremeEvent.ToString()));
        List<Clause> unproved_conditions = new List<Clause>();
        conclusion = rie.Infer("anweisung", unproved_conditions);
        rie.ClearFacts();
        return conclusion == null;
    }
    public bool update = false;
    private void Update() {
        if(update) {
            print(AllowedToStart());
            Debug.Log(conclusion);
            update = false;
        }
    }
}
