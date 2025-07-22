using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting; // Nécessaire pour obtenir le rapport de build

public class BuildAndRunEditor
{
    // Crée une nouvelle option de menu dans l'éditeur d'Unity
    [MenuItem("Outils/Lancer Build Android puis Play")]
    public static void BuildAndThenPlay()
    {
        // --- Étape 1: Configuration du Build ---

        // Affichez une boîte de dialogue pour que l'utilisateur confirme
        if (!EditorUtility.DisplayDialog(
            "Lancer le Build et le Play Mode",
            "Ceci va lancer un build pour Android puis démarrer le Play Mode dans l'éditeur. Continuer ?",
            "Oui", "Annuler"))
        {
            return; // L'utilisateur a annulé
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        // Récupère la liste des scènes activées dans les Build Settings
        buildPlayerOptions.scenes = GetEnabledScenes();

        // Définit le chemin et le nom du fichier de sortie (l'APK pour Android)
        // !! MODIFIEZ CECI pour correspondre à votre projet !!
        buildPlayerOptions.locationPathName = "build.apk";

        // Définit la plateforme cible
        buildPlayerOptions.target = BuildTarget.Android;

        // Lance le processus de build
        Debug.Log("Lancement du build Android...");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        // --- Étape 2: Vérification du résultat et lancement du Play Mode ---

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build terminé avec succès ! ({summary.totalSize / 1024 / 1024} Mo)");

            // Si le build a réussi, on lance le Play Mode dans l'éditeur
            Debug.Log("Lancement du Play Mode dans l'éditeur...");
            EditorApplication.EnterPlaymode();
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError($"Le build a échoué ! {summary.totalErrors} erreurs.");
        }
    }

    // Une petite fonction pour récupérer les scènes cochées dans les Build Settings
    private static string[] GetEnabledScenes()
    {
        var scenes = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenes.Add(scene.path);
            }
        }
        return scenes.ToArray();
    }
}