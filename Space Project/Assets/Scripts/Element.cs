using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[System.Serializable]
public class Element
{
    [SerializeField,ReadOnly]
    int atomicNumber;
    public int AtomicNumber { get { return atomicNumber; } }
    [SerializeField, ReadOnly]
    string element;
    public string ElementName { get { return element; } }
    [SerializeField, ReadOnly]
    string symbol;
    public string Symbol { get { return symbol; } }
    [SerializeField, ReadOnly]
    float atomicMass;
    public float AtomicMass { get { return atomicMass; } }
    [SerializeField, ReadOnly]
    int noOfNutrons;
    public int NumberOfNutrons { get { return noOfNutrons; } }
    [SerializeField, ReadOnly]
    int noOfProtons;
    public int NumberOfProtons { get { return noOfProtons; } }
    [SerializeField, ReadOnly]
    int noOfElectrons;
    public int NumberOfElectrons { get { return noOfElectrons; } }
    [SerializeField, ReadOnly]
    int period;
    public int Period { get { return period; } }
    [SerializeField, ReadOnly]
    int group;
    public int Group { get { return group; } }
    [SerializeField, ReadOnly]
    ElementPhase phase;
    public ElementPhase Phase { get { return phase; } }
    [SerializeField, ReadOnly]
    bool radioactive;
    public bool RadioActive { get { return radioactive; } }
    [SerializeField, ReadOnly]
    bool natural;
    public bool Natural { get { return natural; } }
    [SerializeField, ReadOnly]
    bool metal;
    public bool Metal { get { return metal; } }
    [SerializeField, ReadOnly]
    bool nonmetal;
    public bool Nonmetal { get { return nonmetal; } }
    [SerializeField, ReadOnly]
    bool metaloid;
    public bool Metaloid { get { return metaloid; } }
    [SerializeField, ReadOnly]
    string elementType;
    public string ElementType { get { return elementType; } }
    [SerializeField, ReadOnly]
    float atomicRadius;
    public float AtomicRadius { get { return atomicRadius; } }
    [SerializeField, ReadOnly]
    float electronegativity;
    public float Electronegativity { get { return electronegativity; } }
    [SerializeField, ReadOnly]
    float firstIonization;
    public float FirstIonization { get { return firstIonization; } }
    [SerializeField, ReadOnly]
    double density;
    public double Density { get { return density;  } }
    [SerializeField, ReadOnly]
    float meltingPoint;
    public float MeltingPoint { get { return meltingPoint; } }
    [SerializeField, ReadOnly]
    float boilingPoint;
    public float BoilingPoint { get { return boilingPoint; } }
    [SerializeField, ReadOnly]
    int numberOfIsotopes;
    public int NumberOfIsotopes { get { return numberOfIsotopes; } }
    [SerializeField, ReadOnly]
    string discoverer;
    public string Discoverer { get { return discoverer; } }
    [SerializeField, ReadOnly]
    int year;
    public int Year { get { return year; } }
    [SerializeField, ReadOnly]
    float specificHeat;
    public float SpecificHeat { get { return specificHeat; } }
    [SerializeField, ReadOnly]
    int numberofShells;
    public int NumberofShells { get { return numberofShells; } }
    [SerializeField, ReadOnly]
    int numberofValence;
    public int NumberofValence { get { return numberofValence; } }


    

    public Element(int atomicNumber, string element, string symbol, float atomicMass, int noOfNutrons, int noOfProtons, int noOfElectrons, int period, int group, string phase, bool radioactive, bool natural, bool metal, bool nonmetal,bool metaloid, string elementType, float atomicRadius, float electronegativity, float firstIonization, double density, float meltingPoint, float boilingPoint, int numberOfIsotopes, string discoverer, int year, float specificHeat, int numberofShells, int numberofValence)
    {
        this.atomicNumber = atomicNumber;
        this.element = element;
        this.symbol = symbol;
        this.atomicMass = atomicMass;
        this.noOfNutrons = noOfNutrons;
        this.noOfProtons = noOfProtons;
        this.noOfElectrons = noOfElectrons;
        this.period = period;
        this.group = group;
        this.phase = GetPhaseFromString(phase);
        this.radioactive = radioactive;
        this.natural = natural;
        this.metal = metal;
        this.nonmetal = nonmetal;
        this.metaloid = metaloid;
        this.elementType = elementType;
        this.atomicRadius = atomicRadius;
        this.electronegativity = electronegativity;
        this.firstIonization = firstIonization;
        this.density = density;
        this.meltingPoint = meltingPoint;
        this.boilingPoint = boilingPoint;
        this.numberOfIsotopes = numberOfIsotopes;
        this.discoverer = discoverer;
        this.year = year;
        this.specificHeat = specificHeat;
        this.numberofShells = numberofShells;
        this.numberofValence = numberofValence;
    }

    ElementPhase GetPhaseFromString(string phase)
    {
        ElementPhase phaseToReturn = ElementPhase.None;
        switch (phase)
        {
            case "solid":
                phaseToReturn = ElementPhase.Solid;
                break;
            case "liquid":
                phaseToReturn = ElementPhase.Liquid;
                break;
            case "gas":
                phaseToReturn = ElementPhase.Gas;
                break;
            case "artificial":
                phaseToReturn = ElementPhase.Artificial;
                break;

        }

        return phaseToReturn;
    }

}

public enum ElementPhase
{
    None,
    Solid,
    Liquid,
    Gas,
    Artificial
}