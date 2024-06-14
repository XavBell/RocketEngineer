using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes", menuName = "ScriptableObjects/Material/Recipe")]
public class Recipes : ScriptableObject
{
    public string Name;

    [Header("Recipe Reactants")]
    public List<Substance> Reactants;
    public List<float> reactantRatios;

    [Header("Recipe Products")]
    public List<Substance> Products;
    public List<float> productRatios;

}
