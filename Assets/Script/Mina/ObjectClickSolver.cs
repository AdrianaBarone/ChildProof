using System;
using Unity.Behavior;
using UnityEngine;

public class ObjectClickSolver : MonoBehaviour
{
    private Renderer objectRenderer;
    private bool solved = false;
    public string newTag = "SolvedTarget";
    private Blackboard blackboard; // Riferimento al behavior tree
    public string currentTargetStateKey = "CurrentTargetState"; // Chiave della variabile nel behavior tree
    public Animator animator;
    void Start()
    {
        // Ottieni il Renderer dell'oggetto
        objectRenderer = GetComponent<Renderer>();
    }

    void OnMouseDown()
    {
        if (!solved) // Esegui l'azione solo se non è già risolto
        {
            // Cambia il colore dell'oggetto in verde
            if (objectRenderer != null)
            {
                objectRenderer.material.color = Color.green;
            }

            // Imposta il flag solved su true
            solved = true;
            // Cambia il tag dell'oggetto
            gameObject.tag = newTag;

            // Imposta il valore SolvedTarget alla variabile CurrentTargetState nel behavior tree
            if (blackboard != null)
            {
                blackboard.Variables[4].ObjectValue = newTag;
            }
            if(animator.GetComponent("Clicked")==true){
                animator.SetBool("GoAway", true);
            }
        }
    }
}
