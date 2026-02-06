# ProSoft EasySave - Instructions GitHub Copilot

## 🏢 Contexte d'entreprise
**Éditeur** : ProSoft  
**Produit** : EasySave (logiciel de sauvegarde professionnel)  
**Tarification** : 200 €HT/unité + maintenance 12%/an  
**Support** : 5/7 jours, 8h-17h  

## 🎯 Philosophie du projet
Ce projet suit un développement **incrémental multi-versions** (1.0 → 1.1 → 2.0 → 3.0).
- **Maintenabilité** : Le code doit être repris par d'autres équipes
- **Évolutivité** : Minimiser les modifications pour les futures versions
- **Réactivité** : Capacité à corriger rapidement les bugs
- **Compatibilité** : EasyLog.dll doit rester compatible entre toutes les versions

## 🛠️ Stack technique OBLIGATOIRE

### Outils et environnement
- **IDE** : Visual Studio 2022 ou supérieur
- **VCS** : GitHub (versioning, suivi modifications, collaboration)
- **UML** : PlantUML ou ArgoUML
- **Langage** : C# (conventions Microsoft)
- **Framework** : .NET 8.0 minimum (actuellement .NET 10.0)
- **Formats** : JSON (prioritaire) et XML (supporté depuis v1.1)

### Interfaces utilisateur (évolution)
- **v1.0-1.1** : Application Console (.NET)
- **v2.0+** : Interface graphique (WPF, Avalonia, ou équivalent) + architecture MVVM
- **Toutes versions** : Support ligne de commande (CLI) obligatoire

## 📐 Conventions de code STRICTES

### Standards C# Microsoft
- **Casing** :
  - Classes, méthodes, propriétés : `PascalCase`
  - Variables locales, paramètres : `camelCase`
  - Champs privés : `_camelCase` (préfixe underscore)
  - Constantes : `PascalCase` ou `UPPER_SNAKE_CASE`
- **Nullable** : Activé (`<Nullable>enable</Nullable>`)
- **Implicit usings** : Activé quand pertinent
- **Documentation XML** : Obligatoire pour toutes les APIs publiques

### Règles de lisibilité
✅ **OBLIGATOIRE** :
- Code et commentaires en **anglais** (équipes anglophones)
- Nombre de lignes par fonction **raisonnable** (<50 lignes idéalement)
- **Zéro duplication** : pas de copier-coller, utiliser des méthodes réutilisables
- Noms descriptifs et explicites

❌ **INTERDIT** :
- Chemins absolus hardcodés (ex: `C:\temp\`)
- Code redondant
- Commentaires en français
- Fonctions monolithiques (>100 lignes)

## 🏗️ Architecture et organisation

### Structure modulaire (toutes versions)
```
EasySave/
├── EasySave.Console/       # v1.0-1.1 : Interface Console
├── EasySave.GUI/           # v2.0+ : Interface graphique (WPF/Avalonia)
├── EasySave.Core/          # Logique métier (partagée entre versions)
├── EasyLog/                # DLL de logging (COMPATIBLE ENTRE VERSIONS)
├── EasySave.Shared/        # Modèles et utilitaires partagés
└── CryptoSoft/             # v2.0+ : Logiciel de cryptage (mono-instance v3.0)
```

### EasyLog.dll - Règle d'OR 🔒
**CRITIQUE** : Cette bibliothèque doit maintenir la **rétrocompatibilité absolue**.
- ✅ Ajouter de nouvelles méthodes : OK
- ✅ Ajouter des propriétés optionnelles avec valeurs par défaut : OK
- ❌ Modifier les signatures existantes : INTERDIT
- ❌ Supprimer des méthodes/propriétés : INTERDIT
- ❌ Changer le format de sérialisation : INTERDIT (sauf ajout de formats)

### Patterns et principes
- **Singleton** : Pour managers globaux (ex: `LanguageManager`)
- **MVVM** : v2.0+ pour l'interface graphique
- **Dependency Injection** : Privilégier pour la testabilité
- **Repository Pattern** : Pour l'accès aux données (jobs, config)
- **Factory Pattern** : Pour la création de loggers (JSON/XML)
- **Strategy Pattern** : Pour les types de sauvegarde (Full/Differential)

## 📂 Gestion des fichiers et chemins

### Emplacements standards Windows
- **Configuration** : `%AppData%\ProSoft\EasySave\`
- **Logs** : `%AppData%\ProSoft\EasySave\Logs\`
- **Jobs** : `%AppData%\ProSoft\EasySave\jobs.json`
- **State** : `%AppData%\ProSoft\EasySave\state.json`
- **Config** : `%AppData%\ProSoft\EasySave\config.json`

⚠️ **Jamais** de chemins hardcodés comme `C:\temp\` ou `D:\data\`

### Formats de fichiers
- **JSON** : Format par défaut avec `WriteIndented: true` (lisible dans Notepad)
- **XML** : Supporté depuis v1.1 (choix utilisateur)
- **Logs journaliers** : Nommage `YYYY-MM-DD.json` ou `.xml`
- **UNC paths** : Support obligatoire (`\\server\share\path`)

## 📝 Logging et état

### LogEntry (fichier journalier)
Informations **minimales** :
- `Timestamp` : Horodatage (ISO 8601)
- `JobName` : Nom du travail de sauvegarde
- `SourcePath` : Chemin source complet (format UNC)
- `TargetPath` : Chemin destination complet (format UNC)
- `FileName` : Nom du fichier transféré
- `FileSize` : Taille en octets
- `TransferTime` : Temps de transfert en ms (**négatif si erreur**)
- `EncryptionTime` : (v2.0+) Temps de cryptage en ms (0 si pas de cryptage, <0 si erreur)
- `ErrorMessage` : Message d'erreur si applicable

### StateEntry (fichier d'état temps réel)
Informations **minimales** :
- `Name` : Appellation du travail
- `LastActionTimestamp` : Horodatage de la dernière action
- `State` : État (ACTIVE, END, PAUSED, STOPPED)
- Si actif :
  - `TotalFiles` : Nombre total de fichiers
  - `TotalSize` : Taille totale en octets
  - `Progression` : Pourcentage (0-100)
  - `NbFilesLeftToDo` : Fichiers restants
  - `NbFilesLeftToDoSize` : Taille restante
  - `CurrentSourceFilePath` : Fichier source en cours
  - `CurrentTargetFilePath` : Fichier destination en cours

## 🔄 Évolutions entre versions

### Gestion des travaux de sauvegarde
- **v1.0-1.1** : Maximum **5 travaux**
- **v2.0+** : Nombre **illimité**

### Mode d'exécution
- **v1.0-2.0** : Mono-travail ou **séquentiel**
- **v3.0+** : Exécution **parallèle** avec gestion de priorités

### Détection logiciel métier
- **v1.0-1.1** : Non supporté
- **v2.0** : Interdiction de lancer un travail si détecté
- **v3.0+** : Pause automatique de tous les travaux

### Cryptage (CryptoSoft)
- **v1.0-1.1** : Non supporté
- **v2.0** : Cryptage conditionnel (extensions paramétrables)
- **v3.0+** : CryptoSoft **mono-instance** (gestion mutex)

### Centralisation des logs
- **v1.0-2.0** : Logs locaux uniquement
- **v3.0+** : Centralisation via **Docker** (local, centralisé, ou les deux)

## 🌍 Internationalisation

### Support linguistique
- **Minimum** : Anglais (EN) et Français (FR)
- **Format** : Fichiers `lang.{culture}.json`
- **Fallback** : Toujours vers l'anglais si traduction manquante
- **Clés** : Format dot notation (ex: `"menu.create_job"`)

### Gestion dans le code
```csharp
// Utiliser un gestionnaire centralisé
string text = LanguageManager.Instance.GetString("key");

// Toujours fournir un fallback
string text = GetString("key") ?? "[MISSING_KEY]";
```

## 🚀 Ligne de commande (CLI)

### Format standardisé (toutes versions)
```bash
EasySave.exe <job_specification>

# Exemples valides :
EasySave.exe 1          # Job unique
EasySave.exe 1-3        # Range (jobs 1, 2, 3)
EasySave.exe 1;3;5      # Liste séparée par point-virgule
```

⚠️ En mode CLI : **aucune interaction utilisateur** (exécution silencieuse)

## 🧪 Tests et validation

### Scénarios de test obligatoires
- ✅ Sauvegardes sur disques locaux, externes, et réseaux (UNC)
- ✅ Traitement récursif des sous-répertoires
- ✅ Gestion des erreurs réseau et permissions
- ✅ Calcul de progression en temps réel
- ✅ Validation des formats JSON/XML générés
- ✅ Compatibilité ascendante (charger config v1.0 dans v2.0)
- ✅ Support multi-langues (EN/FR minimum)

### Performance
- Calculer `TotalFiles` et `TotalSize` **avant** le début du transfert
- Mettre à jour `state.json` après chaque fichier (pas plus fréquent)
- Mesurer `TransferTime` avec précision (millisecondes)
- (v3.0+) Respecter les limites de taille pour transferts parallèles

## 📦 Livrables à chaque version

### Documentation technique
1. **User Manual** : 1 page maximum (EN/FR)
2. **Technical Support** : Emplacements fichiers, config minimale, troubleshooting
3. **Release Notes** : Obligatoires pour chaque version
4. **UML Diagrams** : À livrer 24h avant chaque livrable
   - Class diagram
   - Sequence diagrams (opérations clés)
   - Component diagram
   - Activity diagram (workflow sauvegarde)
   - Use case diagram

### Code et versioning
- Tous les documents et codes sur **GitHub**
- Commits fréquents avec messages descriptifs en anglais
- Branches feature pour nouvelles fonctionnalités
- Tags de version pour chaque livrable (v1.0, v1.1, v2.0, v3.0)

## 💡 Directives pour Copilot

### Lors de la génération de code
1. **Toujours vérifier** la version cible (v1.0, v2.0, v3.0)
2. **Respecter** les limitations de la version (ex: 5 jobs max en v1.0)
3. **Anticiper** les évolutions (code modulaire et extensible)
4. **Documenter** avec XML comments (APIs publiques)
5. **Éviter** la duplication : privilégier les méthodes réutilisables

### Suggestions de refactoring
- Identifier et éliminer les copier-coller
- Proposer des abstractions pour faciliter les futures versions
- Signaler les violations des conventions de nommage
- Suggérer des patterns adaptés au contexte

### Compatibilité entre versions
- **JAMAIS** modifier les contrats de `EasyLog.dll`
- Utiliser des propriétés optionnelles pour les nouvelles données
- Versionner les formats de configuration si nécessaire
- Tester la rétrocompatibilité lors des migrations

## 🚫 Anti-patterns à éviter

❌ **Code smells critiques** :
- Copier-coller de blocs de code
- Méthodes de plus de 100 lignes
- Chemins hardcodés (`C:\temp\`, `D:\backup\`)
- Couplage fort entre couches (Console ↔ Core)
- Logique métier dans l'interface utilisateur
- Sérialisation sans gestion d'erreurs
- Pas de validation des chemins utilisateur
- Ignorer les erreurs de transfert

❌ **Violations d'architecture** :
- Accès direct aux fichiers depuis l'UI
- Logique de sauvegarde dans `EasyLog.dll`
- Références circulaires entre projets
- État global mutable partagé (sauf Singleton justifié)

## 🎯 Optimisations futures (prévoir)

### Version 4.0 potentielle
- Interface web pour gestion centralisée
- Cloud backup support (Azure, AWS, Google Drive)
- Compression des sauvegardes
- Déduplication des fichiers
- Scheduler intégré (sauvegardes planifiées)
- Notifications par email/webhook
- Dashboard de monitoring en temps réel
- API REST pour intégration externe

### Questions d'architecture à considérer
- **Parallélisme** : Bénéfice réel vs complexité ?
- **Compression** : Impact sur CPU vs gain d'espace ?
- **Déduplication** : Coût de calcul vs réduction du volume ?
- **Batch size** : Optimiser selon la latence réseau ?

## 📊 Métriques de qualité

### Code
- **Duplication** : <5% de lignes dupliquées
- **Complexité cyclomatique** : <10 par méthode
- **Couverture de tests** : >70% (idéalement >80%)
- **Taille des méthodes** : <50 lignes (moyenne)

### Git
- **Fréquence de commits** : Au moins 3-5/jour par développeur
- **Taille des commits** : Atomiques et focalisés
- **Messages de commit** : Format conventionnel (feat, fix, refactor, docs)
- **Branches** : Feature branches + merge via pull requests

## 🔐 Sécurité et robustesse

### Validation des entrées
- Toujours valider les chemins utilisateur (éviter path traversal)
- Vérifier les permissions avant transfert
- Gérer les cas de fichiers verrouillés
- Timeout sur les opérations réseau

### Gestion des erreurs
- Logger toutes les erreurs avec contexte
- Ne jamais exposer les détails techniques à l'utilisateur
- Proposer des actions correctives (retry, skip, abort)
- Nettoyer les ressources en cas d'erreur (IDisposable, using)

---

## 📌 Résumé des règles d'OR

1. **EasyLog.dll** : Compatibilité absolue entre versions ⚠️
2. **Pas de copier-coller** : Réutilisation via méthodes
3. **Code en anglais** : Équipes anglophones
4. **Chemins dynamiques** : Utiliser `%AppData%`
5. **JSON indented** : Lisibilité dans Notepad
6. **CLI support** : Toutes versions
7. **Documentation XML** : APIs publiques
8. **UML à jour** : Livrer 24h avant chaque livrable
9. **Release notes** : Obligatoires
10. **GitHub** : Environnement de travail unique

---

**Version de ce prompt** : 2026-02-06  
**Compatible avec** : EasySave v1.0 à v3.0+  
**Prochaine révision** : À chaque nouveau cahier des charges