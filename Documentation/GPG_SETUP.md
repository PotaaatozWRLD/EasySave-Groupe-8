# Configuration GPG pour commits vérifiés GitHub

## 📦 Installation

### Windows

1. **Télécharger Gpg4win** : <https://www.gpg4win.org/download.html>
2. **Installer** (cocher "GnuPG" minimum)
3. **Vérifier** : Ouvrir PowerShell et taper `gpg --version`

### Alternative (Chocolatey)

```powershell
choco install gnupg
```

## 🔑 Génération de la clé GPG

### 1. Créer une clé GPG

```powershell
gpg --full-generate-key
```

**Répondre aux questions:**

- Type de clé : `1` (RSA and RSA)
- Taille de clé : `4096`
- Validité : `0` (pas d'expiration) ou `1y` (1 an)
- Nom : `Kenan HUREMOVIC`
- Email : `hipixe.potatoz@outlook.fr` (celui de GitHub)
- Commentaire : `EasySave Project`
- Phrase secrète : Choisir un mot de passe fort

### 2. Lister les clés

```powershell
gpg --list-secret-keys --keyid-format=long
```

**Résultat attendu:**

```
sec   rsa4096/XXXXXXXXXXXXXXXX 2026-02-06 [SC]
      YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
uid                 [ultimate] Kenan HUREMOVIC (EasySave Project) <hipixe.potatoz@outlook.fr>
ssb   rsa4096/ZZZZZZZZZZZZZZZZ 2026-02-06 [E]
```

**Note:** `XXXXXXXXXXXXXXXX` est ton **GPG Key ID**

### 3. Exporter la clé publique

```powershell
gpg --armor --export XXXXXXXXXXXXXXXX
```

**Résultat:** Bloc commençant par `-----BEGIN PGP PUBLIC KEY BLOCK-----`

## 🔧 Configuration Git

### 1. Configurer Git pour signer automatiquement

```powershell
# Remplacer XXXXXXXXXXXXXXXX par ton Key ID
git config --global user.signingkey XXXXXXXXXXXXXXXX
git config --global commit.gpgsign true
git config --global tag.gpgsign true

# Configurer le programme GPG
git config --global gpg.program "C:\Program Files (x86)\GnuPG\bin\gpg.exe"
```

### 2. Vérifier la configuration

```powershell
git config --global --get user.signingkey
git config --global --get commit.gpgsign
```

## 🌐 Ajouter la clé à GitHub

### 1. Copier la clé publique

```powershell
gpg --armor --export XXXXXXXXXXXXXXXX | clip
```

### 2. Sur GitHub

1. Va sur <https://github.com/settings/keys>
2. Clique sur **"New GPG key"**
3. **Title:** `EasySave - PC Principal`
4. **Key:** Colle le contenu (commence par `-----BEGIN PGP PUBLIC KEY BLOCK-----`)
5. Clique **"Add GPG key"**

## ✅ Tester la signature

### 1. Faire un commit signé

```powershell
cd "C:\Users\hipix\Desktop\Projet Genie Logiciel"
git add README.md
git commit -S -m "test: GPG signature test"
git push
```

### 2. Vérifier localement

```powershell
git log --show-signature -1
```

**Résultat attendu:**

```
gpg: Signature made ...
gpg: Good signature from "Kenan HUREMOVIC <hipixe.potatoz@outlook.fr>"
```

### 3. Vérifier sur GitHub

- Va sur <https://github.com/PotaaatozWRLD/EasySave-Groupe-8/commits>
- Le commit doit avoir un badge **"Verified"** ✅ vert

## 🔄 Re-signer les commits précédents (optionnel)

**Attention:** Réécriture de l'historique Git !

```powershell
# Re-signer les 5 derniers commits
git rebase --exec 'git commit --amend --no-edit -n -S' -i HEAD~5
git push --force-with-lease
```

## 🚨 Dépannage

### Erreur: "gpg: signing failed: Inappropriate ioctl for device"

```powershell
export GPG_TTY=$(tty)
```

### Erreur: "cannot open display"

```powershell
git config --global gpg.program gpg
```

### GPG demande le mot de passe à chaque commit

```powershell
# Installer gpg-agent (inclus dans Gpg4win)
# Configurer le cache
echo "default-cache-ttl 3600" >> %APPDATA%\gnupg\gpg-agent.conf
echo "max-cache-ttl 86400" >> %APPDATA%\gnupg\gpg-agent.conf
gpg-connect-agent reloadagent /bye
```

## 📝 Résumé des commandes

```powershell
# 1. Générer la clé
gpg --full-generate-key

# 2. Récupérer le Key ID
gpg --list-secret-keys --keyid-format=long

# 3. Exporter la clé publique
gpg --armor --export XXXXXXXXXXXXXXXX | clip

# 4. Configurer Git
git config --global user.signingkey XXXXXXXXXXXXXXXX
git config --global commit.gpgsign true

# 5. Tester
git commit -S -m "test: Signed commit"
git push
```

## 🔗 Liens utiles

- Gpg4win: <https://www.gpg4win.org/>
- GitHub GPG Guide: <https://docs.github.com/en/authentication/managing-commit-signature-verification>
- GPG Keyserver: <https://keys.openpgp.org/>

---

**Après configuration, tous les futurs commits auront le badge "Verified" ✅**
