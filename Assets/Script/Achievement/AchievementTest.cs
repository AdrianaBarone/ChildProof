using UnityEngine;

public class AchievementTest : MonoBehaviour {

    void Start() {
        // Simula un'interazione con l'oggetto "Cubo"
        TestInteraction("Cubo");

        // Chiama il metodo per testare l'interazione con "Sfera" dopo 20 secondi
        Invoke("TestInteractionWithSphere", 20f);
    }

    void TestInteraction(string objectName) {
        Debug.Log($"Testing interaction with: {objectName}");

        // Chiama il metodo IncrementAchievement del manager

        // AchievementManager.Instance.IncrementAchievement(objectName, 1);
    }

    void TestInteractionWithSphere() {
        TestInteraction("Sfera");
    }
}