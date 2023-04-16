using System.Collections;
using System.Collections.Generic;
using chen0040.ExpertSystem;
using UnityEngine;

public class Wettertest : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int minHeight = 1000;
    [SerializeField] private int minSight = 50; 
    [SerializeField] private int maxWindSpeed = 50; 
    public enum Weathertypes {
        sun,
        rain, 
        storm
    }
    [SerializeField] private int windSpeed;
    [SerializeField] private int height = 2000;
    [SerializeField] private int sight = 300;
    [SerializeField] private Weathertypes weather;
    [SerializeField] private bool extremereignis = false;
    RuleInferenceEngine rie = new RuleInferenceEngine();
    void Start()
    {
        Rule rule = new Rule("Wolken");
        rule = new Rule("Extremereignis");
        rule.AddAntecedent(new IsClause("extremereignis", "True"));
        rule.setConsequent(new IsClause("anweisung", "verschieben"));
        rie.AddRule(rule);

        rule.AddAntecedent(new LEClause("wolkenuntergrenze", minHeight.ToString()));
        rule.setConsequent(new IsClause("anweisung", "warten"));
        rie.AddRule(rule);

        rule = new Rule("Sicht");
        rule.AddAntecedent(new LEClause("sichtweite", minSight.ToString()));
        rule.setConsequent(new IsClause("anweisung", "warten"));
        rie.AddRule(rule);

        rule = new Rule("Wetter");
        rule.AddAntecedent(new IsClause("wettertyp", Weathertypes.storm.ToString()));
        rule.setConsequent(new IsClause("anweisung", "warten"));
        rie.AddRule(rule);

        rule = new Rule("Wind");
        rule.AddAntecedent(new GEClause("windgeschwindigkeit", maxWindSpeed.ToString()));
        rule.setConsequent(new IsClause("anweisung", "warten"));
        rie.AddRule(rule);
        /* IF Wolkenuntergrenze < Mindesthöhe AND/OR Sichtweite < Mindestsichtweite THEN Flug absagen/verschieben.

            IF Gewitter in der Nähe des Flughafens oder auf der geplanten Flugroute THEN Flug absagen/verschieben.

            IF Windgeschwindigkeit > Grenzwerte für Starts und Landungen THEN Flug absagen/verschieben.

            IF Vulkanasche in der Nähe des Flughafens oder auf der geplanten Flugroute OR Hochwasser/Überschwemmungen OR Waldbrand THEN Flug absagen/verschieben. */

        StartCoroutine(Check());
    }

    private IEnumerator Check() {
        var delay = new WaitForSeconds(10);
        while(true) {
            rie.AddFact(new IsClause("wolkenuntergrenze", height.ToString()));
            rie.AddFact(new IsClause("sichtweite", sight.ToString()));
            rie.AddFact(new IsClause("wettertyp", weather.ToString()));
            rie.AddFact(new IsClause("windgeschwindigkeit", windSpeed.ToString()));
            rie.AddFact(new IsClause("extremereignis", extremereignis.ToString()));
            List<Clause> unproved_conditions = new List<Clause>();
            Clause conclusion = rie.Infer("anweisung", unproved_conditions);
            print(unproved_conditions.Count);
            Debug.Log(conclusion);
            rie.ClearFacts();
            yield return delay;
        }
    }
}
